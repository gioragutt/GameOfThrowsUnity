using System;
using System.Runtime.Serialization;

namespace GotLib
{
    /// <summary>
    /// Represents the player data that should be transfered to the server
    /// </summary>
    [Serializable]
    public class PlayerData : ISerializable
    {
        public float xPos;
        public float yPos;

        /// <summary>
        /// Constructor to deserialize player data from the serialized string
        /// </summary>
        /// <param name="info">serialized player data info</param>
        /// <param name="context">context</param>
        public PlayerData(SerializationInfo info, StreamingContext context)
        {
            // Reset the property value using the GetValue method.
            xPos = (float)info.GetValue("xpos", typeof(float));
            yPos = (float)info.GetValue("ypos", typeof(float));
        }

        public PlayerData()
        {
            
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("xpos", xPos, typeof(float));
            info.AddValue("ypos", yPos, typeof(float));
        }

        public override string ToString()
        {
            return string.Format("{{{0:##.00}, {1:##.00}}}", xPos, yPos);
        }
    }
}
