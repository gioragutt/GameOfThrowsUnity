using System.IO;
using GotLib;
using UnityEngine;

namespace Assets.Scripts.PlayerClasses
{
    public abstract class Player : MonoBehaviour
    {
        public PlayerData Data { get; protected set; }
        public abstract void Read(BinaryReader reader);
    }
}
