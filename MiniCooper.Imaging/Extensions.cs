using System;
using System.Collections.Generic;
using System.Linq;

namespace ESCPOS.PrinterC
{
    public static class Extensions
    {
        /// <summary>Dividir a matriz dada em um número x de matrizes menores, cada um de comprimento len</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayIn"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        internal static T[][] Split<T>(this T[] arrayIn, int len)
        {
            bool even = arrayIn.Length % len == 0;
            int totalLength = arrayIn.Length / len;
            if (!even)
                totalLength++;

            T[][] newArray = new T[totalLength][];
            for (int i = 0; i < totalLength; ++i)
            {
                int allocLength = len;
                if (!even && i == totalLength - 1)
                    allocLength = arrayIn.Length % len;

                newArray[i] = new T[allocLength];
                Array.Copy(arrayIn, i * len, newArray[i], 0, allocLength);
            }

            return newArray;
        }

        /// <summary>Retorna uma nova matriz fatiada a partir desta matriz semelhante a matriz de notação Python / Go [0:4]</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="start">Start index</param>
        /// <param name="len">Tamanho da parte final</param>
        /// <returns>parte do array</returns>
        internal static T[] Slice<T>(this T[] arr, int start, int len)
        {
            var result = new T[len];
            Array.Copy(arr, start, result, 0, len);

            return result;
        }

        /// <summary>Preenche o array com val até que o comprimento do terminal seja igual ao newSize</summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="arr">Arrray to fill</param>
        /// <param name="newSize">Nova contagem total de elementos</param>
        /// <param name="val">Valur to pad with</param>
        /// <returns>Nova matriz com newSize contagem de elementos</returns>
        internal static T[] Pad<T>(this T[] arr, int newSize, T val)
        {
            if (arr.Length < newSize)
            {
                var filler = Repeated<T>(val, newSize - arr.Length).ToArray();
                var result = new T[newSize];
                Array.Copy(arr, result, arr.Length);
                Array.Copy(filler, 0, result, arr.Length - 1, filler.Length);
            }

            return arr;
        }

        /// <summary>
        /// Arredonda este inteiro para o múltiplo positivo mais próximo de N
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int RoundUp(this int i, int N)
        {
            return (int)RoundUp(i, (uint)N);
        }

        /// <summary>
        /// Rounds this integer to the nearest positive multiple of N
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static long RoundUp(this long i, int N)
        {
            return RoundUp(i, (uint)N);
        }

        /// <summary>
        /// Rounds this integer to the nearest positive multiple of N
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static uint RoundUp(this uint i, int N)
        {
            return (uint)RoundUp(i, (uint)N);
        }

        /// <summary>
        /// Retorna uma lista do tipo T com tempos de contagem de repetição de valor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static List<T> Repeated<T>(T value, int count)
        {
            List<T> ret = new List<T>(count);
            ret.AddRange(Enumerable.Repeat(value, count));
            return ret;
        }

        /// <summary>
        /// Arredonda este inteiro para o múltiplo positivo mais próximo de N
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private static long RoundUp(long i, uint N)
        {
            if (N == 0)
            {
                return 0;
            }

            if (i == 0)
            {
                return N;
            }

            return (long)(Math.Ceiling(Math.Abs(i) / (double)N) * N);
        }
    }
}
