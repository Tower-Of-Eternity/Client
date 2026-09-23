namespace TowerOfEternity.Controllers.StateMachine
{
    /// <summary>
    /// Giao diện chuẩn mực cho một trạng thái trong Finite State Machine (FSM).
    /// </summary>
    public interface IPlayerState
    {
        /// <summary>
        /// Được gọi 1 lần duy nhất ngay khi bước vào trạng thái này.
        /// </summary>
        void Enter();

        /// <summary>
        /// Xử lý tín hiệu điều khiển của người chơi trong frame (gọi trong Update).
        /// </summary>
        void HandleInput();

        /// <summary>
        /// Cập nhật logic của trạng thái (đếm timer, kiểm tra điều kiện chuyển đổi).
        /// </summary>
        void LogicUpdate();

        /// <summary>
        /// Cập nhật chuyển động vật lý (gọi trong FixedUpdate).
        /// </summary>
        void PhysicsUpdate();

        /// <summary>
        /// Được gọi 1 lần duy nhất ngay trước khi rời khỏi trạng thái này.
        /// </summary>
        void Exit();

        /// <summary>
        /// Kích hoạt khi có tín hiệu Nhảy (Space).
        /// </summary>
        void OnJump();

        /// <summary>
        /// Kích hoạt khi có tín hiệu Lướt né chiêu (Shift).
        /// </summary>
        void OnDash();
    }
}
