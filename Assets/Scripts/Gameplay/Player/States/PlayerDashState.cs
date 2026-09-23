using UnityEngine;
using TowerOfEternity.Gameplay.Player.StateMachine;

namespace TowerOfEternity.Gameplay.Player.States
{
    /// <summary>
    /// [Gameplay/Player/States] Trạng thái lướt gió tốc độ cao.
    /// Khóa input bẻ lái trong 0.2s.
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

            player.TriggerDashCooldown();
            player.Visual.UpdateFacing(dashDirection.x);

            Debug.Log("<color=cyan>[FSM] -> DASH! Lướt gió né chiêu!</color>");
        }

        public override void HandleInput()
        {
            // Khóa đổi hướng trong lúc lướt
        }

        public override void LogicUpdate()
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
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

        public override void OnJump() { }
        public override void OnDash() { }
    }
}
