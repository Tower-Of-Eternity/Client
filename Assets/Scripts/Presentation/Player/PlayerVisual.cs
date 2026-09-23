using UnityEngine;

namespace TowerOfEternity.Presentation.Player
{
    /// <summary>
    /// [Presentation/Player] Xử lý hiển thị (View):
    /// - Lật hướng nhìn của Sprite (quay trái / quay phải).
    /// - Tương tác animation (Idle, Run, Sprint, Dash, Jump, Fall).
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

        public void UpdateFacing(float xDirection)
        {
            if (Mathf.Abs(xDirection) < 0.01f) return;

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = xDirection < 0f;
            }
            else
            {
                float sign = xDirection < 0f ? -1f : 1f;
                transform.localScale = new Vector3(Mathf.Abs(initialScale.x) * sign, initialScale.y, initialScale.z);
            }
        }
    }
}
