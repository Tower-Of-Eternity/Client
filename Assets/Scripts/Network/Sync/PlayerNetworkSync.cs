using System;
using System.Collections.Generic;
using UnityEngine;
using TowerOfEternity.Input;
using TowerOfEternity.Network.Commands;
using TowerOfEternity.Network.Connection;
using TowerOfEternity.Network.Snapshots;
using TowerOfEternity.Gameplay.Player.Motor;

namespace TowerOfEternity.Network.Sync
{
    /// <summary>
    /// [Network/Sync] Điều phối đồng bộ mạng cho người chơi:
    /// 1. Gửi lệnh Intent (MoveCommand, DashCommand) kèm Sequence Number lên Server với tần số 20Hz (50ms).
    /// 2. Lưu trữ hàng đợi unacknowledgedInputs phục vụ Client Prediction.
    /// 3. Nhận WorldSnapshot và thực hiện Server Reconciliation REPLAY khi phát hiện sai lệch.
    /// </summary>
    [RequireComponent(typeof(IInputReader))]
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerNetworkSync : MonoBehaviour
    {
        [Header("--- CẤU HÌNH ĐỒNG BỘ MẠNG ---")]
        [Tooltip("Tần số gửi lệnh lên server (0.05s = 20 lần/giây)")]
        [SerializeField] private float sendInterval = 0.05f;

        [Tooltip("Khoảng cách lệch tối đa cho phép trước khi kích hoạt hòa giải Server Reconciliation (mét)")]
        [SerializeField] private float reconciliationThreshold = 0.5f;

        private IInputReader inputReader;
        private PlayerMotor motor;
        private float timer = 0f;
        private Vector2 lastSentDir = Vector2.zero;
        private string myPlayerId = string.Empty;

        // Quản lý Sequence Number & Hàng đợi input chưa được Server ACK
        private long sequenceNumber = 0;
        private readonly Queue<MoveCommand> unacknowledgedInputs = new Queue<MoveCommand>();

        private void Awake()
        {
            inputReader = GetComponent<IInputReader>();
            motor = GetComponent<PlayerMotor>();
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.OnDashTriggered += SendDashCommand;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.OnDashTriggered -= SendDashCommand;
            }
        }

        private void Start()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnMessageReceived += HandleServerMessage;
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnMessageReceived -= HandleServerMessage;
            }
        }

        private void Update()
        {
            if (NetworkManager.Instance == null || !NetworkManager.Instance.IsConnected) return;

            timer += Time.deltaTime;
            if (timer >= sendInterval)
            {
                timer = 0f;
                SendMoveCommandIfChanged();
            }
        }

        private void SendMoveCommandIfChanged()
        {
            Vector2 currentDir = inputReader.MoveDirection;

            // Gửi khi đang di chuyển, hoặc vừa buông phím về (0,0)
            if (currentDir != Vector2.zero || lastSentDir != Vector2.zero)
            {
                sequenceNumber++;

                MoveCommand cmd = new MoveCommand(
                    sequenceNumber,
                    currentDir,
                    inputReader.IsSprintHeld,
                    false
                );

                // Lưu lại input vào hàng đợi Client Prediction
                unacknowledgedInputs.Enqueue(cmd);

                _ = NetworkManager.Instance.SendMessageAsync(cmd.ToJson());
                lastSentDir = currentDir;
            }
        }

        private void SendDashCommand()
        {
            if (NetworkManager.Instance == null || !NetworkManager.Instance.IsConnected) return;

            sequenceNumber++;

            MoveCommand dashCmd = new MoveCommand(
                sequenceNumber,
                inputReader.MoveDirection,
                false,
                true // isDash = true! Đã kết nối Dash từ Client lên Server!
            );

            unacknowledgedInputs.Enqueue(dashCmd);
            _ = NetworkManager.Instance.SendMessageAsync(dashCmd.ToJson());
            Debug.Log($"<color=cyan>[NetworkSync] Đã gửi lệnh DASH (Seq #{sequenceNumber}) lên Server!</color>");
        }

        private void HandleServerMessage(string rawJson)
        {
            if (string.IsNullOrEmpty(rawJson)) return;

            // 1. Nhận gói chào mừng khởi tạo Player ID
            if (rawJson.Contains("\"WELCOME\""))
            {
                try
                {
                    var welcome = JsonUtility.FromJson<WelcomePacket>(rawJson);
                    myPlayerId = welcome.playerId;
                    Debug.Log($"<color=green>[NetworkSync] Đã nhận Player ID từ Server: {myPlayerId}</color>");
                }
                catch { }
                return;
            }

            // 2. Nhận gói WorldSnapshot định kỳ từ 20Hz GameLoop
            if (rawJson.Contains("\"WORLD_SNAPSHOT\""))
            {
                try
                {
                    var snapshot = WorldSnapshot.FromJson(rawJson);
                    if (snapshot == null || snapshot.players == null) return;

                    foreach (var p in snapshot.players)
                    {
                        if (p.playerId == myPlayerId)
                        {
                            // 2.1. Dọn dẹp các input đã được Server xử lý (ACK)
                            while (unacknowledgedInputs.Count > 0 && unacknowledgedInputs.Peek().sequenceNumber <= p.ackSequence)
                            {
                                unacknowledgedInputs.Dequeue();
                            }

                            // 2.2. Kiểm tra độ lệch giữa vị trí Client hiện tại và vị trí Server chốt
                            Vector2 serverPos = new Vector2(p.x, p.y);
                            Vector2 localPos = new Vector2(motor.GroundPosition.x, motor.GroundPosition.y);
                            float errorDistance = Vector2.Distance(localPos, serverPos);

                            if (errorDistance > reconciliationThreshold)
                            {
                                // 2.3. FULL SERVER RECONCILIATION WITH REPLAY:
                                // Bước A: Reset về vị trí Server chỉ định
                                Vector3 replayedPos = new Vector3(serverPos.x, serverPos.y, motor.GroundPosition.z);

                                // Bước B: Tua nhanh (Replay) lại toàn bộ các input chưa được ACK trong hàng đợi
                                // Đảm bảo đồng nhất 100% với mô hình mô phỏng của Server (0.2s Dash, khóa hướng)
                                float replayDashTimer = 0f;
                                Vector2 replayDashDir = Vector2.zero;

                                // Nếu Snapshot từ Server ghi nhận nhân vật đang trong trạng thái DASH
                                if (p.state == "DASH")
                                {
                                    replayDashTimer = 0.15f; // Tiếp tục thời lượng lướt còn lại
                                    replayDashDir = (inputReader.MoveDirection != Vector2.zero) ? inputReader.MoveDirection.normalized : Vector2.right;
                                }

                                foreach (var unackedCmd in unacknowledgedInputs)
                                {
                                    // 1. Kích hoạt Dash nếu gặp lệnh Dash mới và không đang trong thời gian lướt cũ
                                    if (unackedCmd.isDash && replayDashTimer <= 0f)
                                    {
                                        replayDashTimer = 0.2f; // DASH_DURATION = 0.2s chuẩn Server
                                        replayDashDir = new Vector2(unackedCmd.dirX, unackedCmd.dirY);
                                        if (replayDashDir == Vector2.zero) replayDashDir = Vector2.right;
                                        replayDashDir = replayDashDir.normalized;
                                    }

                                    // 2. Nếu đang trong thời gian lướt: Bay cố định theo hướng đã khóa với tốc độ 18m/s
                                    if (replayDashTimer > 0f)
                                    {
                                        replayedPos += (Vector3)(replayDashDir * 18f * sendInterval);
                                        replayDashTimer -= sendInterval;
                                    }
                                    else
                                    {
                                        // 3. Di chuyển thông thường
                                        Vector2 dir = new Vector2(unackedCmd.dirX, unackedCmd.dirY);
                                        if (dir != Vector2.zero)
                                        {
                                            dir = dir.normalized;
                                            float speed = unackedCmd.isSprint ? 8.5f : 5.0f;
                                            replayedPos += (Vector3)(dir * speed * sendInterval);
                                        }
                                    }
                                }

                                // Bước C: Áp dụng vị trí sau khi đã replay mượt mà
                                motor.SetPositionFromServer(replayedPos);
                                Debug.LogWarning($"<color=orange>[Reconciliation] Lệch {errorDistance:F2}m! Đã hòa giải và Replay {unacknowledgedInputs.Count} inputs về: ({replayedPos.x:F2}, {replayedPos.y:F2}) [ACK #{p.ackSequence}]</color>");
                            }
                            break;
                        }
                    }
                }
                catch { }
            }
        }

        [Serializable]
        private class WelcomePacket
        {
            public string type;
            public string playerId;
        }
    }
}
