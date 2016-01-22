using System.Collections;
using System.Linq;
using Assets.Scripts.PlayerClasses;
using UnityEngine;

namespace Assets.Scripts
{
    internal static class GotExtentions
    {
        /// <summary>
        /// Returns the Max of the x and y values of a UnityEngine.Vector2
        /// </summary>
        /// <param name="vector">this</param>
        /// <returns>float that is the maximum and x and y</returns>
        public static float BiggestDimension(this Vector2 vector)
        {
            float max = vector[0];

            for (int i = 1; i < vector.magnitude; ++i)
                if (vector[i] > max)
                    max = vector[i];

            return max;
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

        public static float GetDimensionByDirection(this Vector2 vector, Directions direction)
        {
            if (direction == Directions.Back || direction == Directions.Front)
                return Mathf.Abs(vector.y);
            if (direction == Directions.Left || direction == Directions.Right)
                return Mathf.Abs(vector.x);
            return 0;
        }

        public static bool IsNotEqualToAny(this object thisObject, params object[] objectsList)
        {
            return !objectsList.Contains(thisObject);
        }

        public static object GetRandom(IList list)
        {
            return list[Random.Range(0, list.Count)];
        }
    }
}