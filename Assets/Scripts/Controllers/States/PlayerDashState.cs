using UnityEngine;
using TowerOfEternity.Controllers.StateMachine;

namespace TowerOfEternity.Controllers.States
{
    /// <summary>
    /// Trạng thái Lướt gió né chiêu (Dash):
    /// - Khóa input điều hướng thông thường trong thời gian lướt (0.2s).
    /// - Di chuyển cực nhanh theo hướng đã định.
    /// - Tự động quay về Move hoặc Idle khi kết thúc lướt.
    /// </summary>
    public class PlayerDashState : PlayerBaseState
    {
        private Vector2 dashDirection;
        private float dashTimer;

        public PlayerDashState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            dashDirection = player.InputReader.MoveDirection;
            dashTimer = player.Config.DashDuration;

            // Kích hoạt tính thời gian hồi chiêu Dash
            player.TriggerDashCooldown();

            // Lật hướng Sprite theo chiều lướt
            player.Visual.UpdateFacing(dashDirection.x);

            Debug.Log("<color=cyan>[FSM] -> DASH! Lướt gió né chiêu!</color>");
        }

        public override void HandleInput()
        {
            // Cố tình để trống: Trong khi lướt (0.2s), nhân vật bị khóa đổi hướng WASD
        }

        public override void LogicUpdate()
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                // Khi hết thời gian lướt, kiểm tra trạng thái tiếp theo
                if (!player.Motor.IsGrounded)
                {
                    stateMachine.ChangeState(player.AirState);
                }
                else if (player.InputReader.MoveDirection != Vector2.zero)
                {
                    stateMachine.ChangeState(player.MoveState);
                }
                else
                {
                    stateMachine.ChangeState(player.IdleState);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            player.Motor.MoveGround(dashDirection, player.Config.DashSpeed, Time.fixedDeltaTime);
        }

        // Bỏ qua OnJump và OnDash trong lúc đang lướt
        public override void OnJump() { }
        public override void OnDash() { }
    }
}
