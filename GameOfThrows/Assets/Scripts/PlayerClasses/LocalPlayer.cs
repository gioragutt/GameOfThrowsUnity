using System.IO;
using GotLib;

namespace Assets.Scripts.PlayerClasses
{
    public class LocalPlayer : Player
    {
        public PlayerDataExtractor dataExtractor;

        public override void Read(BinaryReader reader)
        {
            // Ignore all player data
            new PlayerData().Read(reader);
        }

        #region Game Loop Methods

        public void Awake()
        {
            Data = new PlayerData
            {
                name = "",
                xPos = 0,
                yPos = 0
            };
        }

        public void FixedUpdate()
        {
            UpdateData();
        }

        #endregion

        #region Other Methods

        private void UpdateData()
        {
            Data = dataExtractor.GetPlayerData();
        }

        #endregion
    }
}
