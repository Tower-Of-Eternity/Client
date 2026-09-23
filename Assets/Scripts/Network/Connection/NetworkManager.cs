using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TowerOfEternity.Network.Connection
{
    /// <summary>
    /// [Network/Connection] Quản lý kết nối WebSocket Client đến Game Server Spring Boot.
    /// Sử dụng ClientWebSocket gốc của .NET để tối ưu hiệu năng và không phụ thuộc thư viện thứ 3.
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }

        [Header("--- THÔNG TIN KẾT NỐI SERVER ---")]
        [SerializeField] private string serverUri = "ws://localhost:8080/game";

        private ClientWebSocket webSocket;
        private CancellationTokenSource cts;

        public event Action<string> OnMessageReceived;
        public bool IsConnected => webSocket != null && webSocket.State == WebSocketState.Open;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async void Start()
        {
            await ConnectToServerAsync();
        }

        public async Task ConnectToServerAsync()
        {
            try
            {
                webSocket = new ClientWebSocket();
                cts = new CancellationTokenSource();

                Debug.Log($"<color=cyan>[Network] Đang kết nối tới Server: {serverUri}...</color>");
                await webSocket.ConnectAsync(new Uri(serverUri), cts.Token);
                Debug.Log("<color=green>[Network] Kết nối Game Server thành công!</color>");

                _ = ReceiveLoopAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Network] Kết nối thất bại: {ex.Message}");
            }
        }

        private async Task ReceiveLoopAsync()
        {
            byte[] buffer = new byte[4096];

            while (webSocket != null && webSocket.State == WebSocketState.Open && !cts.IsCancellationRequested)
            {
                try
                {
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed", CancellationToken.None);
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    OnMessageReceived?.Invoke(message);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Network] Lỗi nhận tin nhắn: {ex.Message}");
                    break;
                }
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (!IsConnected) return;

            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cts.Token);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Network] Lỗi gửi tin nhắn: {ex.Message}");
            }
        }

        private async void OnDestroy()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            if (webSocket != null)
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client shutting down", CancellationToken.None);
                }
                webSocket.Dispose();
            }
        }
    }
}
