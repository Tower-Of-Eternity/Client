using System;
using UnityEngine;

namespace TowerOfEternity.Models
{
    /// <summary>
    /// Chứa toàn bộ thông số cấu hình di chuyển của nhân vật.
    /// Giúp tách biệt dữ liệu (Data) khỏi logic xử lý (Behavior).
    /// </summary>
    [Serializable]
    public class PlayerMovementConfig
    {
        [Header("--- TỐC ĐỘ DI CHUYỂN ---")]
        [Tooltip("Tốc độ đi bộ thông thường (m/s)")]
        [SerializeField] private float walkSpeed = 5f;

        [Tooltip("Tốc độ chạy nước rút khi đè Shift (m/s)")]
        [SerializeField] private float sprintSpeed = 8.5f;

        [Header("--- CƠ CHẾ DASH (LƯỚT GIÓ) ---")]
        [Tooltip("Tốc độ lướt tức thời")]
        [SerializeField] private float dashSpeed = 18f;

        [Tooltip("Thời gian duy trì một cú lướt (giây)")]
        [SerializeField] private float dashDuration = 0.2f;

        [Tooltip("Thời gian hồi chiêu giữa 2 lần lướt (giây)")]
        [SerializeField] private float dashCooldown = 0.8f;

        [Header("--- CƠ CHẾ NHẢY & TRỌNG LỰC ---")]
        [Tooltip("Lực bật nhảy lần 1 từ mặt đất")]
        [SerializeField] private float jumpForce = 8f;

        [Tooltip("Lực bật nhảy lần 2 trên không trung (Double Jump)")]
        [SerializeField] private float doubleJumpForce = 10f;

        [Tooltip("Gia tốc trọng lực kéo nhân vật rơi xuống đất")]
        [SerializeField] private float gravity = 25f;

        // Getters công khai
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
