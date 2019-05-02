using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace ESCPOS.PrinterC
{
    public interface IDitherable
    {
        /// <summary>
        /// Número de linhas na matriz do algoritmo
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// Números de colunas na matriz de algoritmo
        /// </summary>
        int ColCount { get; }

        /// <summary>
        /// O divisor do algoritmo
        /// </summary>
        int Divisor { get; }

        /// <summary>
        /// Limite de preto ou branco
        /// </summary>
        byte Threshold { get; }

        /// <summary>Gera uma nova versão pontilhada do bitmap de entrada usando os parâmetros do algoritmo configurado.</summary>
        /// <param name="input">Input bitmap</param>
        /// <returns></returns>
        Bitmap GenerateDithered(Bitmap input);
    }

    /// <summary>
    /// Classe de pontilhamento de base
    /// </summary>
    internal class Dither : IDitherable
    {
        // 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member")]
        private readonly byte[,] _mMatrixPattern;
        private readonly bool _mCanShift;
        private readonly int _mMatrixOffset;

        /// <summary>Cria uma instância dessa classe de pontilhamento</summary>
        /// <param name="matrixPattern">algoritmo em forma de matriz</param>
        /// <param name="divisor">divisor de algoritmo</param>
        /// <param name="threshold">limiar no qual um pixel é considerado 'preto'</param>
        /// <param name="shift"></param>
        public Dither(byte[,] matrixPattern, int divisor, byte threshold, bool shift = false)
        {
            if (matrixPattern == null)
                throw new ArgumentNullException("matrixPattern must not be null");

            if (divisor == 0)
                throw new ArgumentException("divisor must be non-zero");

            _mMatrixPattern = matrixPattern;
            RowCount = matrixPattern.GetUpperBound(0) + 1;
            ColCount = matrixPattern.GetUpperBound(1) + 1;

            Divisor = divisor;
            Threshold = threshold;

            _mCanShift = shift;

            // Localiza a primeira coluna de coeficiente não zero na matriz. Este valor deve
            // sempre estará na primeira linha da matriz
            for (int i = 0; i < ColCount; i++)
            {
                if (matrixPattern[0, i] != 0)
                {
                    _mMatrixOffset = (byte)(i - 1);
                    break;
                }
            }
        }

        /// <summary>
        /// Número de linhas na matriz do algoritmo
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// Números de colunas na matriz de algoritmo
        /// </summary>
        public int ColCount { get; private set; }

        /// <summary>
        /// Divisor do algoritmo
        /// </summary>
        public int Divisor { get; private set; }

        /// <summary>
        /// Limite de preto ou branco
        /// </summary>
        public byte Threshold { get; private set; }

        /// <summary>Crie uma cópia desse bitmap com um algoritmo de pontilhamento aplicado</summary>
        /// <param name="bitmap">Input bitmap</param>
        /// <returns>Novo bitmap pontilhado</returns>
        public virtual Bitmap GenerateDithered(Bitmap bitmap)
        {
            var bmpBuff = bitmap.ToBuffer();
            var pixels = new List<Pixel>();

            // Converta todos os bytes em pixels
            foreach (var pix in bmpBuff.Split(4))
            {
                pixels.Add(new Pixel(pix));
            }

            // Dither away
            for (int x = 0; x < bitmap.Height; x++)
            {
                for (int y = 0; y < bitmap.Width; y++)
                {
                    var index = x * bitmap.Width + y;
                    var colored = pixels[index];
                    var grayed = ApplyGrayscale(colored);
                    pixels[index] = grayed;

                    ApplySmoothing(pixels, colored, grayed, y, x, bitmap.Width, bitmap.Height);
                }
            }

            // despejar resultados na saída
            var output = new byte[pixels.Count << 2];
            var j = 0;
            for (var i = 0; i < pixels.Count; i++)
            {
                var p = pixels[i];
                output[j++] = p.B;
                output[j++] = p.G;
                output[j++] = p.R;

                // RT-15 - force alpha para 0xFF porque no modo otimizado,
                // o cliente .NET pode enviar dados de bitmap estranhos.
                output[j++] = 0xFF;
            }

            return output.AsBitmap(bitmap.Width, bitmap.Height);
        }

        /// <summary>Aplique escala de cinza a este pixel e retorne o resultado</summary>
        /// <param name="pix">Pixel para transformar</param>
        /// <returns>pixel reduzido em cores (tons de cinza)</returns>
        protected virtual Pixel ApplyGrayscale(Pixel pix)
        {
            // Números mágicos para converter RGB em espaço monocromático. Estes conseguem uma escala de cinza balanceada
            byte grayPoint = (byte)(0.299 * pix.R + 0.587 * pix.G + 0.114 * pix.B);

            // Não altere o canal alfa, caso contrário, a imagem inteira pode ficar opaca
            Pixel grayed;
            grayed.A = pix.A;

            if (grayPoint < Threshold)
            {
                grayed.R = grayed.G = grayed.B = 0;
            }
            else
            {
                grayed.R = grayed.G = grayed.B = 255;
            }

            return grayed;
        }

        /// <summary>Aplicar algoritmo de pontilhamento</summary>
        /// <param name="imageData">dados da imagem</param>
        /// <param name="colored">Pixel source</param>
        /// <param name="grayed">Pixel source</param>
        /// <param name="x">posição da coluna do Pixel</param>
        /// <param name="y">posição y da linha do Pixel</param>
        /// <param name="width">width of imageData</param>
        /// <param name="height">altura de imageData</param>
        protected virtual void ApplySmoothing(
            IList<Pixel> imageData,
            Pixel colored,
            Pixel grayed,
            int x,
            int y,
            int width,
            int height)
        {
            int redError = colored.R - grayed.R;
            int blueError = colored.G - grayed.G;
            int greenError = colored.B - grayed.B;

            for (int row = 0; row < RowCount; row++)
            {
                // Converte linha para o índice principal da linha
                int ypos = y + row;

                for (int col = 0; col < ColCount; col++)
                {
                    int coefficient = _mMatrixPattern[row, col];

                    // Converte a coluna para o índice principal da linha
                    int xpos = x + (col - _mMatrixOffset);

                    // Não processa fora da imagem, 1ª linha / col ou se o pixel é 0
                    if (coefficient != 0 &&
                        xpos > 0 &&
                        xpos < width &&
                        ypos > 0 &&
                        ypos < height)
                    {
                        int offset = ypos * width + xpos;
                        Pixel dithered = imageData[offset];

                        int newR, newG, newB;

                        // Calcular o efeito dither em cada canal de cor
                        if (_mCanShift)
                        {
                            newR = (redError * coefficient) >> Divisor;
                            newG = (greenError * coefficient) >> Divisor;
                            newB = (blueError * coefficient) >> Divisor;
                        }
                        else
                        {
                            newR = (redError * coefficient) / Divisor;
                            newG = (greenError * coefficient) / Divisor;
                            newB = (blueError * coefficient) / Divisor;
                        }

                        // Certifique-se de não resultar em overflow
                        dithered.R = safe_tobyte(dithered.R + newR);
                        dithered.G = safe_tobyte(dithered.G + newG);
                        dithered.B = safe_tobyte(dithered.B + newB);

                        // Aplicar nova cor
                        imageData[offset] = dithered;
                    }
                }
            }
        }

        /// <summary> Retorna um inteiro como um byte e manipula qualquer over / underflow</summary>
        /// <param name="val">int</param>
        /// <returns>byte</returns>
        private static byte safe_tobyte(int val)
        {
            if (val < 0)
            {
                val = 0;
            }
            else if (val > 255)
            {
                val = 255;
            }
            return (byte)val;
        }
    }

    /// <summary>
    /// OneBPP converte a imagem para uma imagem de 1 bit por pixel
    /// </summary>
    internal class OneBPP : Dither
    {
        public OneBPP(byte threshold)
            : base(new byte[,] { { 0 } }, 1, threshold)
        { }

        /// <summary>Override retorna uma cópia de 1bpp do bitmap de entrada</summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public override Bitmap GenerateDithered(Bitmap bitmap)
        {
            var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            return bitmap.Clone(rectangle, PixelFormat.Format1bppIndexed);
        }
    }
}
