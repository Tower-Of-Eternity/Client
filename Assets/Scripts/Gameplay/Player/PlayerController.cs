using UnityEngine;
using TowerOfEternity.Input;
using TowerOfEternity.Gameplay.Player.Config;
using TowerOfEternity.Gameplay.Player.Motor;
using TowerOfEternity.Gameplay.Player.StateMachine;
using TowerOfEternity.Gameplay.Player.States;
using TowerOfEternity.Presentation.Player;

namespace TowerOfEternity.Gameplay.Player
{
    /// <summary>
    /// [Gameplay/Player] Bộ điều phối Facade của người chơi:
    /// - Kết nối InputReader, Motor, Visual, Config và StateMachine.
    /// - Quản lý vòng đời và chuyển đổi trạng thái.
    /// </summary>
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerVisual))]
    public class PlayerController : MonoBehaviour
    {
        [Header("--- CẤU HÌNH THÔNG SỐ DI CHUYỂN ---")]
        [SerializeField] private PlayerMovementConfig config = new PlayerMovementConfig();

        public IInputReader InputReader { get; private set; }
        public PlayerMovementConfig Config => config;
        public PlayerMotor Motor { get; private set; }
        public PlayerVisual Visual { get; private set; }
        public PlayerStateMachine StateMachine { get; private set; }

        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerDashState DashState { get; private set; }
        public PlayerAirState AirState { get; private set; }

        private float dashCooldownTimer = 0f;
        public bool CanDash => dashCooldownTimer <= 0f;

        private void Awake()
        {
            InputReader = GetComponent<IInputReader>();
            Motor = GetComponent<PlayerMotor>();
            Visual = GetComponent<PlayerVisual>();

            StateMachine = new PlayerStateMachine();
            IdleState = new PlayerIdleState(this, StateMachine);
            MoveState = new PlayerMoveState(this, StateMachine);
            DashState = new PlayerDashState(this, StateMachine);
            AirState = new PlayerAirState(this, StateMachine);
        }

        private void OnEnable()
        {
            if (InputReader != null)
            {
                InputReader.OnJumpTriggered += OnJumpInput;
                InputReader.OnDashTriggered += OnDashInput;
            }
        }

        private void OnDisable()
        {
            if (InputReader != null)
            {
                InputReader.OnJumpTriggered -= OnJumpInput;
                InputReader.OnDashTriggered -= OnDashInput;
            }
        }

        private void Start()
        {
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            if (dashCooldownTimer > 0f)
            {
                dashCooldownTimer -= Time.deltaTime;
            }

            StateMachine.CurrentState?.HandleInput();
            StateMachine.CurrentState?.LogicUpdate();
        }

        private void FixedUpdate()
        {
            StateMachine.CurrentState?.PhysicsUpdate();
            Motor.ApplyPosition();
        }

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
