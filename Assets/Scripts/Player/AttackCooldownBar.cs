using UnityEngine;
using UnityEngine.UI;

public class AttackCooldownBar : MonoBehaviour
{
    [SerializeField] private PlayerAttackSwipe attackSwipe;
    [SerializeField] private Image fillImage;      // Image Type = Filled
    [SerializeField] private Color readyColor = Color.white;
    [SerializeField] private Color cooldownColor = Color.red;
    [SerializeField] private float smooth = 12f;   // 0 = snap to raw value

    private void Update()
    {
        if (attackSwipe == null || fillImage == null) return;

        float target = attackSwipe.CooldownProgress01;
        fillImage.fillAmount = (smooth <= 0f)
            ? target
            : Mathf.MoveTowards(fillImage.fillAmount, target, Time.deltaTime * smooth);

        fillImage.color = attackSwipe.IsReady ? readyColor : cooldownColor;
    }
}