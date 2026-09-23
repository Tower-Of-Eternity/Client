using TowerOfEternity.Gameplay.Player.StateMachine;

namespace TowerOfEternity.Gameplay.Player.States
{
    /// <summary>
    /// [Gameplay/Player/States] Lớp cơ sở cho toàn bộ các State của Player.
    /// </summary>
    public abstract class PlayerBaseState : IPlayerState
    {
        protected readonly PlayerController player;
        protected readonly PlayerStateMachine stateMachine;

        protected PlayerBaseState(PlayerController player, PlayerStateMachine stateMachine)
        {
            this.player = player;
            this.stateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void HandleInput() { }
        public virtual void LogicUpdate() { }
        public virtual void PhysicsUpdate() { }
        public virtual void Exit() { }
        public virtual void OnJump() { }
        public virtual void OnDash() { }
    }
}
