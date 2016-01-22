using System;
using System.IO;

namespace GotLib
{
    /// <summary>
    /// Represents the player data that should be transfered to the server
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public string name;
        public float xPos;
        public float yPos;
        public float maxHealth;
        public float currentHealth;

        public PlayerData()
        {
            name = string.Empty;
            xPos = 0;
            yPos = 0;
            maxHealth = 0;
            currentHealth = 0;
        }

        #region Binary Reader/Writer Methods

        /* Protocol definition (order of reading and writing of data members):
            1) name                 -   string (var)
            2) xPos                 -   float (4 bytes)
            3) yPos                 -   float (4 bytes)
            4) maxHealth            -   float (4 bytes)
            5) currentHealth        -   float (4 bytes)
        */

        public void Read(BinaryReader reader)
        {
            name = reader.ReadString();
            xPos = reader.ReadSingle();
            yPos = reader.ReadSingle();
            maxHealth = reader.ReadSingle();
            currentHealth = reader.ReadSingle();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(name);
            writer.Write(xPos);
            writer.Write(yPos);
            writer.Write(maxHealth);
            writer.Write(currentHealth);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} : [ {1:0.##} , {2:0.##} ] [ {3} / {4} | {5:0.##} ] ", name, xPos, yPos,
                (int)currentHealth, (int)maxHealth, currentHealth / maxHealth);
        }
    }
}
