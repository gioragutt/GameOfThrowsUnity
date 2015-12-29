using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GotLib
{
    public class Serializers
    {
        public static string SerializeObject<T>(T objectToSerialize)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            try
            {
                bf.Serialize(ms, objectToSerialize);
                ms.Position = 0;

                return Convert.ToBase64String(ms.ToArray());
            }
            finally
            {
                ms.Close();
            }
        }

        public static T DeserializeObject<T>(string str)
        {
            BinaryFormatter bf = new BinaryFormatter();
            var binaryData = Convert.FromBase64String(str);
            MemoryStream ms = new MemoryStream(binaryData);

            try
            {
                return (T)bf.Deserialize(ms);
            }
            finally
            {
                ms.Close();
            }
        }
    }
}
