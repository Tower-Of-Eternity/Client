using UnityEngine;
using TowerOfEternity.Controllers.StateMachine;

namespace TowerOfEternity.Controllers.States
{
    /// <summary>
    /// Trạng thái Đứng yên (Idle):
    /// - Không di chuyển, chờ đợi input người chơi.
    /// - Chuyển sang Move khi bấm WASD, sang Air khi bấm Space, sang Dash khi bấm Shift.
    /// </summary>
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            // Debug.Log("[FSM] -> Chuyển sang: IDLE");
        }

        public override void HandleInput()
        {
            if (player.InputReader.MoveDirection != Vector2.zero)
            {
                stateMachine.ChangeState(player.MoveState);
            }
        }

        public override void OnJump()
        {
            player.Motor.Jump(player.Config.JumpForce);
            stateMachine.ChangeState(player.AirState);
        }

        public override void OnDash()
        {
            if (player.CanDash && player.InputReader.MoveDirection != Vector2.zero)
            {
                stateMachine.ChangeState(player.DashState);
            }
        }
    }
}
