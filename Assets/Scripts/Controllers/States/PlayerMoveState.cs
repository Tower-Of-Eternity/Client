using UnityEngine;
using TowerOfEternity.Controllers.StateMachine;

namespace TowerOfEternity.Controllers.States
{
    /// <summary>
    /// Trạng thái Di chuyển trên mặt đất (Move):
    /// - Bao gồm Đi bộ (Walk) và Chạy nhanh (Sprint khi đè Shift).
    /// - Chuyển sang Idle khi thả phím, sang Air khi nhảy, sang Dash khi lướt.
    /// </summary>
    public class PlayerMoveState : PlayerBaseState
    {
        public PlayerMoveState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            // Debug.Log("[FSM] -> Chuyển sang: MOVE");
        }

        public override void HandleInput()
        {
            Vector2 moveDir = player.InputReader.MoveDirection;

            // Người chơi buông phím -> Quay về Idle
            if (moveDir == Vector2.zero)
            {
                stateMachine.ChangeState(player.IdleState);
                return;
            }

            // Lật hướng Sprite sang trái / phải
            player.Visual.UpdateFacing(moveDir.x);
        }

        public override void PhysicsUpdate()
        {
            Vector2 moveDir = player.InputReader.MoveDirection;
            if (moveDir == Vector2.zero) return;

            // Kiểm tra tốc độ: Chạy nhanh (Sprint) hay Đi bộ (Walk)
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
