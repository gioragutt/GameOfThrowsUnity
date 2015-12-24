using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts
{
    public static class GotExtentions
    {
        /// <summary>
        /// Returns the Max of the x and y values of a UnityEngine.Vector2
        /// </summary>
        /// <param name="vector">this</param>
        /// <returns>float that is the maximum and x and y</returns>
        public static float MaxOfXandY(this Vector2 vector)
        {
            return Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }

        /// <summary>
        /// Zerofies vector values when they are below the epsilon
        /// </summary>
        /// <param name="vector">this</param>
        /// <param name="epsilon">lowest value before zerofication</param>
        public static Vector2 ZerofiyTinyValues(this Vector2 vector, float epsilon)
        {
            float absEpsilon = Mathf.Abs(epsilon);

            if (Mathf.Abs(vector.x) < absEpsilon)
                vector.x = 0;
            if (Mathf.Abs(vector.y) < absEpsilon)
                vector.y = 0;

            return vector;
        }
    }
}
