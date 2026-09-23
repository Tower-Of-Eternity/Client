using System;
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
    /// 1. Gửi lệnh Intent (MoveCommand) lên Server với tần số 20Hz (50ms).
    /// 2. Nhận WorldSnapshot từ Server để kiểm chứng hoặc đồng bộ vị trí.
    /// </summary>
    [RequireComponent(typeof(IInputReader))]
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerNetworkSync : MonoBehaviour
    {
        [Header("--- CẤU HÌNH ĐỒNG BỘ MẠNG ---")]
        [Tooltip("Tần số gửi lệnh lên server (0.05s = 20 lần/giây)")]
        [SerializeField] private float sendInterval = 0.05f;

        private IInputReader inputReader;
        private PlayerMotor motor;
        private float timer = 0f;
        private Vector2 lastSentDir = Vector2.zero;
        private string myPlayerId = string.Empty;

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
                MoveCommand cmd = new MoveCommand(
                    currentDir,
                    inputReader.IsSprintHeld,
                    false // Dash được kích hoạt riêng nếu có
                );

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
                // Sẵn sàng cho bước Interpolation / Reconciliation
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
