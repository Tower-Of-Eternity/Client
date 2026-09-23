using UnityEngine;
using TowerOfEternity.Gameplay.Player.StateMachine;

namespace TowerOfEternity.Gameplay.Player.States
{
    /// <summary>
    /// [Gameplay/Player/States] Trạng thái trên không, rơi tự do và Double Jump.
    /// </summary>
    public class PlayerAirState : PlayerBaseState
    {
        public PlayerAirState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            // Debug.Log("[FSM] -> AIR");
        }

        public override void HandleInput()
        {
            Vector2 moveDir = player.InputReader.MoveDirection;
            player.Visual.UpdateFacing(moveDir.x);
        }

        public override void PhysicsUpdate()
        {
            Vector2 moveDir = player.InputReader.MoveDirection;
            if (moveDir != Vector2.zero)
            {
                player.Motor.MoveGround(moveDir, player.Config.WalkSpeed, Time.fixedDeltaTime);
            }

            bool hasLanded = player.Motor.UpdateAirPhysics(player.Config.Gravity, Time.fixedDeltaTime);

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
            if (player.Motor.JumpsRemaining > 0)
            {
                player.Motor.Jump(player.Config.DoubleJumpForce);
                Debug.Log("<color=yellow>[FSM] -> DOUBLE JUMP! Nhảy đập gió lần 2 cao hơn!</color>");
            }
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
