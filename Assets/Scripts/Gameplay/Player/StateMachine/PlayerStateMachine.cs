using System;

namespace TowerOfEternity.Gameplay.Player.StateMachine
{
    /// <summary>
    /// [Gameplay/Player/StateMachine] Bộ máy hữu hạn quản lý trạng thái hiện tại.
    /// Pure C# Class - Có thể chạy unit test độc lập.
    /// </summary>
    public class PlayerStateMachine
    {
        public IPlayerState CurrentState { get; private set; }
        public event Action<IPlayerState> OnStateChanged;

        public void Initialize(IPlayerState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
            OnStateChanged?.Invoke(CurrentState);
        }

        public void ChangeState(IPlayerState newState)
        {
            if (newState == null || newState == CurrentState) return;

            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
            OnStateChanged?.Invoke(CurrentState);
        }
    }
}
