using System;
using UnityEngine;

namespace TowerOfEternity.Network.Commands
{
    /// <summary>
    /// [Network/Commands] Lệnh di chuyển đại diện cho Ý đồ (Intent) của người chơi.
    /// Client CHỈ gửi hướng muốn đi và trạng thái phím bấm, KHÔNG gửi tọa độ tự tính.
    /// Server là người thẩm định và quyết định vị trí thật (Authoritative).
    /// </summary>
    [Serializable]
    public class MoveCommand
    {
        public string type = "MOVE_CMD";
        public long sequenceNumber; // Số thứ tự gói tin để đối chiếu ACK từ Server
        public float dirX;
        public float dirY;
        public bool isSprint;
        public bool isDash;
        public long clientTime;

        public MoveCommand(long seq, Vector2 dir, bool sprint, bool dash)
        {
            this.sequenceNumber = seq;
            this.dirX = dir.x;
            this.dirY = dir.y;
            this.isSprint = sprint;
            this.isDash = dash;
            this.clientTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
