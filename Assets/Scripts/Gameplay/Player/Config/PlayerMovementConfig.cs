using System;
using UnityEngine;

namespace TowerOfEternity.Gameplay.Player.Config
{
    /// <summary>
    /// [Gameplay/Player/Config] Chứa thông số cấu hình di chuyển của nhân vật.
    /// Dữ liệu thuần túy (POCO), tách biệt hoàn toàn khỏi logic runtime.
    /// </summary>
    [Serializable]
    public class PlayerMovementConfig
    {
        [Header("--- TỐC ĐỘ DI CHUYỂN ---")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8.5f;

        [Header("--- CƠ CHẾ DASH (LƯỚT GIÓ) ---")]
        [SerializeField] private float dashSpeed = 18f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 0.8f;

        [Header("--- CƠ CHẾ NHẢY & TRỌNG LỰC ---")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float doubleJumpForce = 10f;
        [SerializeField] private float gravity = 25f;

        public float WalkSpeed => walkSpeed;
        public float SprintSpeed => sprintSpeed;
        public float DashSpeed => dashSpeed;
        public float DashDuration => dashDuration;
        public float DashCooldown => dashCooldown;
        public float JumpForce => jumpForce;
        public float DoubleJumpForce => doubleJumpForce;
        public float Gravity => gravity;
    }
}
