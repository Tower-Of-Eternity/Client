using System;
using UnityEngine;

namespace TowerOfEternity.Core.Input
{
    /// <summary>
    /// Interface trừu tượng hóa toàn bộ việc nhận tín hiệu điều khiển của nhân vật.
    /// Cho phép dễ dàng hoán đổi giữa Bàn phím (Keyboard), Tay cầm (Gamepad), 
    /// Màn hình cảm ứng (Mobile Joystick) hoặc AI Bot mà không cần sửa code di chuyển.
    /// </summary>
    public interface IInputReader
    {
        /// <summary>
        /// Hướng di chuyển chuẩn hóa (độ dài vector = 1) trên mặt phẳng 2D.
        /// </summary>
        Vector2 MoveDirection { get; }

        /// <summary>
        /// Có đang nhấn giữ phím chạy nhanh (Sprint - mặc định là Shift) hay không.
        /// </summary>
        bool IsSprintHeld { get; }

        /// <summary>
        /// Sự kiện kích hoạt khi người chơi vừa nhấn phím Nhảy (Space).
        /// </summary>
        event Action OnJumpTriggered;

        /// <summary>
        /// Sự kiện kích hoạt khi người chơi vừa nhấn phím Lướt né chiêu (Dash - Shift).
        /// </summary>
        event Action OnDashTriggered;
    }
}
