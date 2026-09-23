using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private ClientWebSocket webSocket;
    private CancellationTokenSource cts;

    // Địa chỉ Game Server Spring Boot
    private readonly string serverUrl = "ws://localhost:8080/game";

    async void Start()
    {
        await ConnectToServer();
    }

    private async Task ConnectToServer()
    {
        webSocket = new ClientWebSocket();
        cts = new CancellationTokenSource();

        try
        {
            Debug.Log($"[Network] Dang ket noi toi Server: {serverUrl}...");
            await webSocket.ConnectAsync(new Uri(serverUrl), cts.Token);
            Debug.Log("[Network] Ket noi WebSocket thanh cong!");

            // Gửi tin nhắn chào mừng lên Server
            await SendMessageAsync("Hello from Unity C# Client!");

            // Bắt đầu vòng lặp lắng nghe tin nhắn từ Server trả về
            _ = ReceiveLoop();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Network] Ket noi that bai: {ex.Message}");
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(bytes);
            await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cts.Token);
            Debug.Log($"[Network] Da gui: {message}");
        }
    }

    private async Task ReceiveLoop()
    {
        byte[] buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open)
        {
            try
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    Debug.Log("[Network] Server da dong ket noi.");
                }
                else
                {
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Debug.Log($"<color=green>[Server Response]</color> {receivedMessage}");
                }
            }
            catch (Exception ex)
            {
                if (webSocket.State != WebSocketState.Open) break;
                Debug.LogError($"[Network] Loi khi nhan tin: {ex.Message}");
            }
        }
    }

    private async void OnDestroy()
    {
        if (webSocket != null)
        {
            cts?.Cancel();
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client Quit", CancellationToken.None);
            }
            webSocket.Dispose();
        }
    }
}
