using System;
using UnityEngine;

namespace TowerOfEternity.Input
{
    /// <summary>
    /// [Input Layer] Interface trừu tượng hóa tín hiệu điều khiển.
    /// Giúp tách biệt phần cứng (Keyboard, Gamepad, Touch, AI) khỏi logic game.
    /// </summary>
    public interface IInputReader
    {
        Vector2 MoveDirection { get; }
        bool IsSprintHeld { get; }

        event Action OnJumpTriggered;
        event Action OnDashTriggered;
    }
}
