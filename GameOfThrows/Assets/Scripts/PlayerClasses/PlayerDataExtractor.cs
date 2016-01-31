using GotLib;
using UnityEngine;

namespace Assets.Scripts.PlayerClasses
{
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerInformation))]
    public class PlayerDataExtractor : MonoBehaviour, IPlayerDataGetter
    {
        #region Public Members

        public PlayerMovement movementComponent;
        public PlayerInformation informationComponent;

        #endregion

        #region Game Loop Methods

        private void Awake()
        {
        }

        #endregion

        private PlayerData ExtractPlayerData()
        {
            var position = movementComponent.Position;
            var stats = informationComponent.stats;

            return new PlayerData
            {
                name = informationComponent.stats.playerName,
                xPos = position.x,
                yPos = position.y,
                maxHealth = stats.maxHealth,
                currentHealth = stats.CurrentHealth
            };
        }

        #region IPlayerDataGetter Methods

        public PlayerData GetPlayerData()
        {
            return ExtractPlayerData();
        }

        #endregion
    }
}
