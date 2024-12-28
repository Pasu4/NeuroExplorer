using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        public static void Shuffle2D(this Array array, System.Random random)
        {
            int len = array.Length;
            int l0 = array.GetLength(0);
            object buffer;
            for(int i = 0; i < len; i++)
            {
                int index = random.Next(i, len);
                buffer = array.GetValue(i % l0, i / l0);
                array.SetValue(array.GetValue(index % l0, index / l0), i % l0, i / l0);
                array.SetValue(buffer, index % l0, index / l0);
            }
        }

        public static bool HasReadPermission(string path)
        {
            try
            {
                _ = Directory.GetDirectories(path);
                _ = Directory.GetFiles(path);
            }
            catch(UnauthorizedAccessException)
            {
                return false;
            }
            return true;
        }

        public static string FileSizeString(long fileSize)
        {
            return fileSize switch
            {
                >= 1_000_000_000_000L => (fileSize / 1_000_000_000_000f).ToString("F2", CultureInfo.InvariantCulture) + " TB",
                >=     1_000_000_000L => (fileSize /     1_000_000_000f).ToString("F2", CultureInfo.InvariantCulture) + " GB",
                >=         1_000_000L => (fileSize /         1_000_000f).ToString("F2", CultureInfo.InvariantCulture) + " MB",
                >=             1_000L => (fileSize /             1_000f).ToString("F2", CultureInfo.InvariantCulture) + " kB",
                _                     => fileSize + " B"
            };
        }

        public static T ChooseWeighted<T>(System.Random random, params (int Weight, T Obj)[] weightedActions)
        {
            int totalWeight = weightedActions.Sum(a => a.Weight);
            int selection = random.Next(totalWeight);
            foreach(var (weight, obj) in weightedActions)
            {
                selection -= weight;
                if(selection < 0) return obj;
            }
            throw new Exception("Unreachable");
        }
    }
}
