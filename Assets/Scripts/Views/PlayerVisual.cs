using UnityEngine;

namespace TowerOfEternity.Views
{
    /// <summary>
    /// Chịu trách nhiệm hiển thị (View):
    /// - Lật hướng nhìn của Sprite (quay trái / quay phải).
    /// - Tương tác animation (Idle, Run, Sprint, Dash, Jump, Fall).
    /// - Ảo ảnh quang học chiều sâu (Shadow, độ phóng đại khi ở trên không).
    /// </summary>
    public class PlayerVisual : MonoBehaviour
    {
        [Header("--- THÀNH PHẦN HIỂN THỊ ---")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Vector3 initialScale;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
            initialScale = transform.localScale;
        }

        /// <summary>
        /// Lật hướng nhìn của nhân vật dựa theo chiều di chuyển trục X.
        /// </summary>
        public void UpdateFacing(float xDirection)
        {
            if (Mathf.Abs(xDirection) < 0.01f) return;

            if (spriteRenderer != null)
            {
                // Cách 1: Dùng flipX của SpriteRenderer (nhẹ và an toàn)
                spriteRenderer.flipX = xDirection < 0f;
            }
            else
            {
                // Cách 2: Lật localScale nếu không có SpriteRenderer trực tiếp
                float sign = xDirection < 0f ? -1f : 1f;
                transform.localScale = new Vector3(Mathf.Abs(initialScale.x) * sign, initialScale.y, initialScale.z);
            }
        }
    }
}
