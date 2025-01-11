using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Utils
    {
        public static T Choose<T>(this System.Random random, IEnumerable<T> enumerable)
        {
            int len = enumerable.Count();
            return enumerable.ElementAt(random.Next(len));
        }

        public static T Choose<T>(this System.Random random, params T[] array) => array[random.Next(array.Length)];
        public static T ChooseRandom<T>(IEnumerable<T> enumerable) => new System.Random().Choose(enumerable);
        public static T ChooseRandom<T>(params T[] array) => new System.Random().Choose(array);

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

        public static T ChooseWeighted<T>(System.Random random, params (int Weight, T Obj)[] weightedResults)
        {
            int totalWeight = weightedResults.Sum(a => a.Weight);
            int selection = random.Next(totalWeight);
            foreach(var (weight, obj) in weightedResults)
            {
                selection -= weight;
                if(selection < 0) return obj;
            }
            throw new Exception("Unreachable");
        }

        public static T ChooseWeightedRandom<T>(params (int Weight, T Obj)[] weightedResults) => ChooseWeighted(new System.Random(), weightedResults);

        public static Vector2 DistributeBetweenCentered(Vector2 start, Vector2 end, int count, int index, float maxDistance = Mathf.Infinity)
        {
            if(count <= 0) throw new ArgumentException(nameof(count) + " must be greater than 0.", nameof(count));

            if(count == 1) return (start + end) / 2f; // Center

            float maxLength = maxDistance * (count - 1); // Only between points
            float length = Vector2.Distance(start, end);

            if(length > maxLength)
            {
                float reduce = (length - maxLength) / 2f; // Reduce on both sides
                start = Vector2.MoveTowards(start, end, reduce);
                end = Vector2.MoveTowards(end, start, reduce);
            }

            float t = ((float) index) / (count - 1);
            return Vector2.LerpUnclamped(start, end, t);
        }

        public static string TrimRTF(this string str)
        {
            bool skip = false;

            return new string(str.Where(c =>
            {
                if(c == '<') skip = true;
                else if(c == '>')
                {
                    skip = false;
                    return false;
                }
                return !skip;
            }).ToArray());
        }
    }
}
