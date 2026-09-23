using System;
using UnityEngine;

namespace TowerOfEternity.Network.Snapshots
{
    /// <summary>
    /// [Network/Snapshots] Dữ liệu trạng thái của một người chơi do Server trả về.
    /// </summary>
    [Serializable]
    public class PlayerSnapshotData
    {
        public string playerId;
        public float x;
        public float y;
        public string state;
        public long ackSequence; // Số thứ tự input cuối cùng Server đã xử lý của Player này
    }

    /// <summary>
    /// [Network/Snapshots] Bức ảnh toàn cảnh thế giới tại một Tick (World Snapshot).
    /// Client nhận dữ liệu này từ Server để đồng bộ và hòa giải (Reconciliation).
    /// </summary>
    [Serializable]
    public class WorldSnapshot
    {
        public string type;
        public long tick;
        public PlayerSnapshotData[] players;

        public static WorldSnapshot FromJson(string json)
        {
            return JsonUtility.FromJson<WorldSnapshot>(json);
        }
    }
}
