using GotLib;
using UnityEngine;

namespace Assets.Scripts
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

            return new PlayerData
            {
                name = informationComponent.playerName,
                xPos = position.x,
                yPos = position.y
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
