using UnityEngine;
using TowerOfEternity.Core.Input;
using TowerOfEternity.Models;
using TowerOfEternity.Views;
using TowerOfEternity.Controllers.StateMachine;
using TowerOfEternity.Controllers.States;

namespace TowerOfEternity.Controllers
{
    /// <summary>
    /// PlayerController đóng vai trò Facade / Bộ điều phối trung tâm:
    /// - Kết nối InputReader, Motor, Visual, Config và StateMachine.
    /// - Quản lý vòng đời và chuyển đổi trạng thái của nhân vật.
    /// - Tuân thủ Clean Architecture và State Pattern (FSM).
    /// </summary>
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerVisual))]
    public class PlayerController : MonoBehaviour
    {
        [Header("--- CẤU HÌNH THÔNG SỐ DI CHUYỂN ---")]
        [SerializeField] private PlayerMovementConfig config = new PlayerMovementConfig();

        // Các thành phần phụ thuộc (Dependencies)
        public IInputReader InputReader { get; private set; }
        public PlayerMovementConfig Config => config;
        public PlayerMotor Motor { get; private set; }
        public PlayerVisual Visual { get; private set; }
        public PlayerStateMachine StateMachine { get; private set; }

        // Các trạng thái (States) cụ thể của nhân vật
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerDashState DashState { get; private set; }
        public PlayerAirState AirState { get; private set; }

        // Quản lý hồi chiêu Lướt gió (Dash)
        private float dashCooldownTimer = 0f;
        public bool CanDash => dashCooldownTimer <= 0f;

        private void Awake()
        {
            // 1. Lấy các component phụ thuộc
            InputReader = GetComponent<IInputReader>();
            Motor = GetComponent<PlayerMotor>();
            Visual = GetComponent<PlayerVisual>();

            // 2. Khởi tạo State Machine và các trạng thái
            StateMachine = new PlayerStateMachine();
            IdleState = new PlayerIdleState(this, StateMachine);
            MoveState = new PlayerMoveState(this, StateMachine);
            DashState = new PlayerDashState(this, StateMachine);
            AirState = new PlayerAirState(this, StateMachine);
        }

        private void OnEnable()
        {
            // Đăng ký lắng nghe sự kiện từ InputReader
            if (InputReader != null)
            {
                InputReader.OnJumpTriggered += OnJumpInput;
                InputReader.OnDashTriggered += OnDashInput;
            }
        }

        private void OnDisable()
        {
            // Hủy đăng ký sự kiện để tránh rò rỉ bộ nhớ (Memory Leak)
            if (InputReader != null)
            {
                InputReader.OnJumpTriggered -= OnJumpInput;
                InputReader.OnDashTriggered -= OnDashInput;
            }
        }

        private void Start()
        {
            // Khởi đầu nhân vật ở trạng thái Đứng yên (Idle)
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            // 1. Đếm ngược hồi chiêu Dash
            if (dashCooldownTimer > 0f)
            {
                dashCooldownTimer -= Time.deltaTime;
            }

            // 2. Chuyển quyền xử lý cho trạng thái hiện tại
            StateMachine.CurrentState?.HandleInput();
            StateMachine.CurrentState?.LogicUpdate();
        }

        private void FixedUpdate()
        {
            // 1. Chuyển quyền tính toán vật lý cho trạng thái hiện tại
            StateMachine.CurrentState?.PhysicsUpdate();

            // 2. Áp dụng tọa độ mới lên transform của GameObject
            Motor.ApplyPosition();
        }

        /// <summary>
        /// Kích hoạt hồi chiêu Dash sau khi vừa thực hiện cú lướt.
        /// </summary>
        public void TriggerDashCooldown()
        {
            dashCooldownTimer = config.DashCooldown;
        }

        private void OnJumpInput()
        {
            StateMachine.CurrentState?.OnJump();
        }

        private void OnDashInput()
        {
            StateMachine.CurrentState?.OnDash();
        }
    }
}
