using UnityEngine;
using TowerOfEternity.Controllers.StateMachine;

namespace TowerOfEternity.Controllers.States
{
    /// <summary>
    /// Trạng thái Trên không (Air / Jump / Fall):
    /// - Chịu tác động của trọng lực kéo rơi tự do parabol.
    /// - Cho phép lượn hướng (Air Control) trên không.
    /// - Kích hoạt Nhảy kép (Double Jump) khi bấm Space lần 2.
    /// - Hỗ trợ Lướt né chiêu trên không (Air Dash).
    /// - Tự động chuyển về Idle hoặc Move khi tiếp đất (Landing).
    /// </summary>
    public class PlayerAirState : PlayerBaseState
    {
        public PlayerAirState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            // Debug.Log("[FSM] -> AIR: Đang trên không trung!");
        }

        public override void HandleInput()
        {
            Vector2 moveDir = player.InputReader.MoveDirection;
            player.Visual.UpdateFacing(moveDir.x);
        }

        public override void PhysicsUpdate()
        {
            // 1. Cho phép bẻ lái trên không (Air Control)
            Vector2 moveDir = player.InputReader.MoveDirection;
            if (moveDir != Vector2.zero)
            {
                player.Motor.MoveGround(moveDir, player.Config.WalkSpeed, Time.fixedDeltaTime);
            }

            // 2. Cập nhật gia tốc trọng lực kéo rơi
            bool hasLanded = player.Motor.UpdateAirPhysics(player.Config.Gravity, Time.fixedDeltaTime);

            // 3. Xử lý chạm đất (Landing)
            if (hasLanded)
            {
                if (player.InputReader.MoveDirection != Vector2.zero)
                {
                    stateMachine.ChangeState(player.MoveState);
                }
                else
                {
                    stateMachine.ChangeState(player.IdleState);
                }
            }
        }

        public override void OnJump()
        {
            // Bấm Space lần 2 trên không trung -> Kích hoạt Double Jump!
            if (player.Motor.JumpsRemaining > 0)
            {
                player.Motor.Jump(player.Config.DoubleJumpForce);
                Debug.Log("<color=yellow>[FSM] -> DOUBLE JUMP! Nhảy đập gió lần 2 cao hơn!</color>");
            }
        }

        public override void OnDash()
        {
            // Cho phép Air Dash nếu hồi chiêu đã sẵn sàng
            if (player.CanDash && player.InputReader.MoveDirection != Vector2.zero)
            {
                stateMachine.ChangeState(player.DashState);
            }
        }
    }
}
