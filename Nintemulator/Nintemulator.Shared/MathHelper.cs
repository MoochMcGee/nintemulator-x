using System;

namespace Nintemulator.Shared
{
    public static class MathHelper
    {
        public const double Pi = Math.PI;
        public const double Tau = Math.PI * 2;

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
        public static int GreatestCommonFactor(int a, int b)
        {
            int remainder;

            while (b != 0)
            {
                remainder = (a % b);
                a = b;
                b = remainder;
            }

            return a;
        }

        public static void Reduce(ref int a, ref int b)
        {
            var gcf = GreatestCommonFactor(a, b);

            a /= gcf;
            b /= gcf;
        }

        public static uint BitsSet(uint number)
        {
            uint count = 0;

            for (; number != 0; count++)
            {
                number = number & (number - 1);
            }

            return count;
        }
        public static uint SignExtend(uint number, int bits)
        {
            var mask = (1u << (bits)) - 1;
            var sign = (1u << (bits - 1));

            return ((number & mask) ^ sign) - sign;
        }

        public static uint NextPowerOfTwo(uint number)
        {
            number--;
            number |= number >> 1;
            number |= number >> 2;
            number |= number >> 4;
            number |= number >> 8;
            number |= number >> 16;
            number++;

            return number;
        }
    }
}