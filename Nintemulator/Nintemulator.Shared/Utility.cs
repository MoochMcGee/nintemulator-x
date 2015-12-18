using System;

namespace Nintemulator.Shared
{
    public static class Utility
    {
        public static T[][] CreateArray<T>(int length1, int length2)
        {
            T[][] result = new T[length1][];

            for (int i = 0; i < length1; i++)
            {
                result[i] = new T[length2];
            }

            return result;
        }

        public static void Initialize<T>(this T[] array)
            where T : new()
        {
            array.Initialize(() => new T());
        }
        public static void Initialize<T>(this T[] array, T value)
        {
            array.Initialize(() => value);
        }
        public static void Initialize<T>(this T[] array, Func<T> construct)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = construct();
            }
        }

        public static uint Decode(string pattern, uint mustbe0, uint mustbe1, uint dontcare)
        {
            uint value = 0u;

            for (int i = 0; i < pattern.Length; i++)
            {
                switch (pattern[i])
                {
                default : value = (value << 1) | dontcare; break;
                case '0': value = (value << 1) | mustbe0; break;
                case '1': value = (value << 1) | mustbe1; break;
                case ' ': break;
                }
            }

            return value;
        }

        public static uint Flat(string pattern) { return Decode(pattern, 0u, 1u, 0u); }
        public static uint Full(string pattern) { return Decode(pattern, 0u, 1u, 1u); }
        public static uint Mask(string pattern) { return Decode(pattern, 1u, 1u, 0u); }
    }
}