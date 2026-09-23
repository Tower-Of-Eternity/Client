using UnityEngine;

namespace TowerOfEternity.Controllers
{
    /// <summary>
    /// Chịu trách nhiệm thuần túy về vật lý và tọa độ:
    /// - Quản lý tọa độ mặt đất (groundPosition).
    /// - Quản lý độ cao trên không (jumpHeight) và vận tốc rơi (verticalVelocity).
    /// - Cập nhật transform.position = groundPosition + (0, jumpHeight, 0) tạo ảo giác 2.5D.
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

        /// <summary>
        /// Di chuyển tọa độ trên mặt đất theo vector hướng và tốc độ.
        /// </summary>
        public void MoveGround(Vector2 direction, float speed, float deltaTime)
        {
            GroundPosition += (Vector3)(direction * speed * deltaTime);
        }

        /// <summary>
        /// Kích hoạt nhảy với lực đẩy theo phương thẳng đứng.
        /// </summary>
        public void Jump(float force)
        {
            VerticalVelocity = force;
            JumpsRemaining = Mathf.Max(0, JumpsRemaining - 1);
        }

        /// <summary>
        /// Cập nhật gia tốc trọng lực và độ cao khi ở trên không.
        /// Trả về true nếu nhân vật vừa hạ cánh chạm đất trong frame này.
        /// </summary>
        public bool UpdateAirPhysics(float gravity, float deltaTime)
        {
            VerticalVelocity -= gravity * deltaTime;
            JumpHeight += VerticalVelocity * deltaTime;

            // Kiểm tra chạm đất
            if (JumpHeight <= 0f)
            {
                JumpHeight = 0f;
                VerticalVelocity = 0f;
                JumpsRemaining = 2; // Hồi phục lại cả 2 lần nhảy
                return true;        // Vừa tiếp đất thành công
            }

            return false;
        }

        /// <summary>
        /// Đặt lại số lần nhảy (ví dụ khi chạm đất).
        /// </summary>
        public void ResetJumps()
        {
            JumpsRemaining = 2;
        }

        /// <summary>
        /// Cập nhật vị trí hiển thị cuối cùng của GameObject.
        /// Gọi trong LateUpdate hoặc sau khi xử lý xong physics.
        /// </summary>
        public void ApplyPosition()
        {
            transform.position = GroundPosition + new Vector3(0, JumpHeight, 0);
        }
    }
}
