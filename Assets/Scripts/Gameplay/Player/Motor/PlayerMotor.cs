using UnityEngine;

namespace TowerOfEternity.Gameplay.Player.Motor
{
    /// <summary>
    /// [Gameplay/Player/Motor] Tính toán tọa độ và chuyển động vật lý 2.5D:
    /// - GroundPosition: Tọa độ mặt phẳng 2D.
    /// - JumpHeight: Độ cao trên không (ảo ảnh 2.5D).
    /// </summary>
    public class PlayerMotor : MonoBehaviour
    {
        public Vector3 GroundPosition { get; private set; }
        public float JumpHeight { get; private set; } = 0f;
        public float VerticalVelocity { get; private set; } = 0f;
        public int JumpsRemaining { get; private set; } = 2;
        public bool IsGrounded => JumpHeight <= 0.001f;

        private void Awake()
        {
            GroundPosition = transform.position;
        }

        public void MoveGround(Vector2 direction, float speed, float deltaTime)
        {
            GroundPosition += (Vector3)(direction * speed * deltaTime);
        }

        public void Jump(float force)
        {
            VerticalVelocity = force;
            JumpsRemaining = Mathf.Max(0, JumpsRemaining - 1);
        }

        public bool UpdateAirPhysics(float gravity, float deltaTime)
        {
            VerticalVelocity -= gravity * deltaTime;
            JumpHeight += VerticalVelocity * deltaTime;

            if (JumpHeight <= 0f)
            {
                JumpHeight = 0f;
                VerticalVelocity = 0f;
                JumpsRemaining = 2;
                return true;
            }

            return false;
        }

        public void ApplyPosition()
        {
            transform.position = GroundPosition + new Vector3(0, JumpHeight, 0);
        }

        public void SetPositionFromServer(Vector3 newGroundPos)
        {
            GroundPosition = newGroundPos;
        }
    }
}
