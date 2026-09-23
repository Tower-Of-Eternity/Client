using UnityEngine;
using TowerOfEternity.Gameplay.Player.StateMachine;

namespace TowerOfEternity.Gameplay.Player.States
{
    /// <summary>
    /// [Gameplay/Player/States] Trạng thái Đứng yên.
    /// </summary>
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            // Debug.Log("[FSM] -> IDLE");
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
