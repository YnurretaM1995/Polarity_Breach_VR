using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PolarityBreach.Player
{
    public class XPBarUI : MonoBehaviour
    {
        [SerializeField] private Slider xpSlider;
        [SerializeField] private TextMeshProUGUI levelText;

        private PlayerXP playerXP;

        private void Start()
        {
            playerXP = PlayerXP.Instance;

            if (playerXP == null)
            {
                Debug.LogWarning("XPBarUI: PlayerXP.Instance not found.");
                return;
            }

            playerXP.OnXPChanged += UpdateXPBar;
            playerXP.OnLevelUp += UpdateLevelText;

            // initialize on scene load
            UpdateXPBar(playerXP.CurrentXP, playerXP.XPToNextLevel);
            UpdateLevelText(playerXP.CurrentLevel);
        }

        private void OnDestroy()
        {
            if (playerXP == null) return;
            playerXP.OnXPChanged -= UpdateXPBar;
            playerXP.OnLevelUp -= UpdateLevelText;
        }

        private void UpdateXPBar(int currentXP, int xpToNextLevel)
        {
            xpSlider.maxValue = xpToNextLevel;
            xpSlider.value = currentXP;
        }

        private void UpdateLevelText(int newLevel)
        {
            if (levelText != null)
                levelText.text = $"Lv. {newLevel}";
        }
    }
}