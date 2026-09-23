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
    /// 1. Gửi lệnh Intent (MoveCommand) kèm Sequence Number lên Server với tần số 20Hz (50ms).
    /// 2. Lưu trữ hàng đợi unacknowledgedInputs phục vụ Client Prediction.
    /// 3. Nhận WorldSnapshot và thực hiện Server Reconciliation khi phát hiện sai lệch.
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
                    false // Dash được kích hoạt riêng nếu có
                );

                // Lưu lại input vào hàng đợi Client Prediction
                unacknowledgedInputs.Enqueue(cmd);

                _ = NetworkManager.Instance.SendMessageAsync(cmd.ToJson());
                lastSentDir = currentDir;
            }
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

                            // 2.2. Server Reconciliation: So sánh vị trí dự đoán với vị trí thật của Server
                            Vector2 serverPos = new Vector2(p.x, p.y);
                            Vector2 localPos = new Vector2(motor.GroundPosition.x, motor.GroundPosition.y);
                            float errorDistance = Vector2.Distance(localPos, serverPos);

                            if (errorDistance > reconciliationThreshold)
                            {
                                // Phát hiện lệch vị trí -> Hòa giải kéo về vị trí Server chỉ định
                                motor.SetPositionFromServer(new Vector3(serverPos.x, serverPos.y, motor.GroundPosition.z));
                                Debug.LogWarning($"<color=orange>[Reconciliation] Lệch {errorDistance:F2}m! Hòa giải về vị trí Server: ({serverPos.x:F2}, {serverPos.y:F2}) [ACK #{p.ackSequence}]</color>");
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
