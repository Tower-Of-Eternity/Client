using TowerOfEternity.Controllers.StateMachine;

namespace TowerOfEternity.Controllers.States
{
    /// <summary>
    /// Lớp cơ sở (Abstract Class) cho mọi trạng thái của nhân vật.
    /// Giúp tái sử dụng các thuộc tính chung và cung cấp hàm ảo mặc định.
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
