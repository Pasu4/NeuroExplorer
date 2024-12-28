using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    internal static class Utils
    {
        public static T Choose<T>(this System.Random random, IEnumerable<T> enumerable)
        {
            int len = enumerable.Count();
            return enumerable.ElementAt(random.Next(len));
        }

        public static IEnumerable<T> ChooseMany<T>(this System.Random random, IEnumerable<T> enumerable, int count)
        {
            int len = enumerable.Count();
            T[] result = new T[count];

            for(int i = 0; i < count; i++)
                result[i] = enumerable.ElementAt(random.Next(len));

            return result;
        }

        public static float Range(this System.Random random, float min, float max)
        {
            return (float) random.NextDouble() * (max - min) + min;
        }
    }
}
