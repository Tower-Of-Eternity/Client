using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TowerOfEternity.Core.Input
{
    /// <summary>
    /// Component chịu trách nhiệm duy nhất: Đọc tín hiệu phần cứng bàn phím từ Unity 6 Input System.
    /// Tuân thủ Single Responsibility Principle (SRP) - Chỉ đọc input, KHÔNG đụng vào transform hay physics.
    /// </summary>
    public class PlayerInputReader : MonoBehaviour, IInputReader
    {
        public Vector2 MoveDirection { get; private set; }
        public bool IsSprintHeld { get; private set; }

        public event Action OnJumpTriggered;
        public event Action OnDashTriggered;

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            // 1. Đọc hướng di chuyển 8 hướng (WASD & Phím Mũi tên)
            Vector2 rawInput = Vector2.zero;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) rawInput.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) rawInput.x += 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) rawInput.y += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) rawInput.y -= 1f;

            MoveDirection = rawInput.normalized;

            // 2. Kiểm tra giữ Shift để chạy nhanh (Sprint)
            IsSprintHeld = keyboard.shiftKey.isPressed;

            // 3. Phím Space: Nhảy hoặc Nhảy kép (Chỉ bắt frame đầu tiên nhấn xuống)
            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                OnJumpTriggered?.Invoke();
            }

            // 4. Phím Shift: Nhấp để Lướt gió (Dash)
            if (keyboard.shiftKey.wasPressedThisFrame && MoveDirection != Vector2.zero)
            {
                OnDashTriggered?.Invoke();
            }
        }
    }
}
