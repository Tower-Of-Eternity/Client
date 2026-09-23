namespace TowerOfEternity.Gameplay.Player.StateMachine
{
    /// <summary>
    /// [Gameplay/Player/StateMachine] Hợp đồng chuẩn cho vòng đời của một State.
    /// Pure C# Interface - hoàn toàn không phụ thuộc Unity Engine.
    /// </summary>
    public interface IPlayerState
    {
        void Enter();
        void HandleInput();
        void LogicUpdate();
        void PhysicsUpdate();
        void Exit();
        void OnJump();
        void OnDash();
    }
}
