using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ESCPOS.Printer.Helpers
{
    public static class Extensions
    {

        /// <summary>Dividir a matriz dada em um número x de matrizes menores, cada um de comprimento len</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayIn"></param>
        /// <param name="len">Tamanho da parte</param>
        /// <returns></returns>
        public static T[][] Split<T>(this T[] arrayIn, int len)
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

        /// <summary>
        /// Concatena todos os arrays em um
        /// </summary>
        /// <param name="args">1 ou mais matrizes de bytes</param>
        /// <returns>byte[]</returns>
        public static byte[] Concat(params byte[][] args)
        {
            using (var buffer = new MemoryStream())
            {
                foreach (var ba in args)
                {
                    buffer.Write(ba, 0, ba.Length);
                }

                var bytes = new byte[buffer.Length];
                buffer.Position = 0;
                buffer.Read(bytes, 0, bytes.Length);

                return bytes;
            }
        }

        /// <summary>
        /// Retorna todos as flags definidas nesse valor em ordem crescente.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static IEnumerable<Enum> GetUniqueFlags(this Enum flags)
        {
            ulong flag = 1;
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits && flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }

        /// <summary>Enumera get flags nesta coleção.</summary>
        ///
        /// <param name="value">The value.
        /// </param>
        ///
        /// <returns>Um enumerador que permite que o foreach seja usado para processar os flags nessa coleção.</returns>
        public static IEnumerable<T> GetFlags<T>(this T value) where T : struct
        {
            return GetFlags(value, Enum.GetValues(value.GetType()).Cast<T>().ToArray());
        }

        /// <summary>Enumera get flags nesta coleção</summary>
        ///
        /// <param name="value"> The value.
        /// </param>
        /// <param name="values">The values.
        /// </param>
        ///
        /// <returns>Um enumerador que permite que o foreach seja usado para processar os flags nessa coleção.</returns>
        private static IEnumerable<T> GetFlags<T>(T value, T[] values) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Type must be an enum.");
            }
            ulong bits = Convert.ToUInt64(value);
            var results = new List<T>();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                ulong mask = Convert.ToUInt64(values[i]);
                if (i == 0 && mask == 0L)
                    break;
                if ((bits & mask) == mask)
                {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }
            if (bits != 0L)
                return Enumerable.Empty<T>();
            if (Convert.ToUInt64(value) != 0L)
                return results.Reverse<T>();
            if (bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L)
                return values.Take(1);
            return Enumerable.Empty<T>();
        }
    }
}
