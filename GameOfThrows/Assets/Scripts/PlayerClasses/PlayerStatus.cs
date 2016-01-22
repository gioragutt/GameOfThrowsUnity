using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PlayerClasses
{
    public class PlayerStatus : MonoBehaviour
    {
        private PlayerInformation playerInfo;
        private Text nameLabel;
        private Text healthLabel;
        private Image healthBar;

        private void Awake()
        {
            playerInfo = GetComponentInParent<PlayerInformation>();
            healthLabel = transform.FindChild("HealthLabel").GetComponent<Text>();
            healthBar = transform.FindChild("HealthBar").GetComponent<Image>();
            nameLabel = transform.FindChild("NameLabel").GetComponent<Text>();

            if (healthLabel == null)
                Debug.LogError("Missing HealthBar Text component in children! Can't show player health!");
            if (nameLabel == null)
                Debug.LogError("Missing NameLabel Text component in children! Can't show player name!");
            if (healthBar == null)
                Debug.LogError("Missing Image component in children! Can't show player health!");
        }

        private void OnGUI()
        {
            // Name Label
            nameLabel.text = playerInfo.stats.playerName.ToUpper();

            // Health Bar
            var healthBarScale = healthBar.rectTransform.localScale;
            healthBarScale.x = playerInfo.stats.CurrentHealth / playerInfo.stats.maxHealth;
            healthBar.rectTransform.localScale = healthBarScale;

            // Health Label
            var curr = Mathf.CeilToInt(playerInfo.stats.CurrentHealth);
            var max = Mathf.CeilToInt(playerInfo.stats.maxHealth);
            var perc = curr * 100 / max;
            healthLabel.text = string.Format("{0}/{1} {2}%", curr, max, perc);
        }
    }
}