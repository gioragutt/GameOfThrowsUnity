using System.IO;

namespace Assets.Scripts.PlayerClasses
{
    public class NetworkPlayer : Player
    {
        public override void Read(BinaryReader reader)
        {
            Data.Read(reader);
        }
    }
}
