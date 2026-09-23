using UnityEngine;
using TowerOfEternity.Gameplay.Player.StateMachine;

namespace TowerOfEternity.Gameplay.Player.States
{
    /// <summary>
    /// [Gameplay/Player/States] Trạng thái di chuyển (Đi bộ / Chạy nhanh).
    /// </summary>
    public class PlayerMoveState : PlayerBaseState
    {
        public PlayerMoveState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            // Debug.Log("[FSM] -> MOVE");
        }

        public override void HandleInput()
        {
            Vector2 moveDir = player.InputReader.MoveDirection;

            if (moveDir == Vector2.zero)
            {
                stateMachine.ChangeState(player.IdleState);
                return;
            }

            player.Visual.UpdateFacing(moveDir.x);
        }

        public override void PhysicsUpdate()
        {
            Vector2 moveDir = player.InputReader.MoveDirection;
            if (moveDir == Vector2.zero) return;

            float speed = player.InputReader.IsSprintHeld 
                ? player.Config.SprintSpeed 
                : player.Config.WalkSpeed;

            player.Motor.MoveGround(moveDir, speed, Time.fixedDeltaTime);
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
