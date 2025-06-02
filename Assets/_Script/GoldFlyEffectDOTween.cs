using UnityEngine;
using DG.Tweening;  // Import DOTween namespace
using UnityEngine.UI;

public class GoldFlyEffectDOTween : MonoBehaviour
{
    // Target là RectTransform của Gold Text trên UI
    public RectTransform target;
    public float duration = 1f; // Thời gian di chuyển từ enemy đến target

    private RectTransform rectTransform;

    void Awake()
    {
        // Lấy RectTransform của chính hiệu ứng này
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        // Nếu bạn instantiate prefab dưới dạng con của Canvas, vị trí của nó đã ở hệ tọa độ screen space.
        // Sử dụng DOTween để di chuyển từ vị trí hiện tại đến target.position.
        rectTransform.DOMove(target.position, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Khi hoàn thành tween, xoá gameobject hiệu ứng.
                Destroy(gameObject);
            });
    }
}
