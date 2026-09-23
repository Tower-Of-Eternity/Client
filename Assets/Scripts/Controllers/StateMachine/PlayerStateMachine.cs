using System;
using UnityEngine;

namespace TowerOfEternity.Controllers.StateMachine
{
    /// <summary>
    /// Bộ máy điều phối trạng thái (Finite State Machine).
    /// Quản lý trạng thái hiện tại và kiểm soát luồng chuyển đổi giữa các trạng thái.
    /// </summary>
    public class PlayerStateMachine
    {
        public IPlayerState CurrentState { get; private set; }

        public event Action<IPlayerState> OnStateChanged;

        /// <summary>
        /// Khởi tạo trạng thái ban đầu của nhân vật (thường là IdleState).
        /// </summary>
        public void Initialize(IPlayerState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
            OnStateChanged?.Invoke(CurrentState);
        }

        /// <summary>
        /// Chuyển đổi từ trạng thái hiện tại sang trạng thái mới một cách an toàn.
        /// </summary>
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
