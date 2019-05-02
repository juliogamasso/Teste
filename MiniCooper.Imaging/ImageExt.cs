using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace ESCPOS.PrinterC
{
    public static class ImageExt
    {
        /// <summary>
        /// Converte em uma matriz de bytes de linha principal.Se não houver dados de imagem
        /// ou a imagem está vazia, a matriz resultante estará vazia (comprimento zero).
        /// </summary>
        /// <returns>byte[]</returns>
        public static byte[] ToBuffer(this Bitmap bitmap)
        {
            if (bitmap == null || bitmap.Size.IsEmpty)
            {
                return new byte[0];
            }

            BitmapData bitmapData = null;

            // Esse retângulo seleciona a totalidade do bitmap de origem para bloquear bits na memória
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            try
            {
                // Adquira um bloqueio nos dados da imagem para que possamos extrair nosso próprio fluxo de bytes
                // Nota: atualmente suporta apenas dados como 32 bits, cor de 4 canais de 8 bits
                bitmapData = bitmap.LockBits(
                    rect,
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppPArgb);

                // Cria o buffer de saída
                int length = Math.Abs(bitmapData.Stride) * bitmapData.Height;
                byte[] results = new byte[length];

                // Copiar de não gerenciado para memória gerenciada
                IntPtr ptr = bitmapData.Scan0;
                Marshal.Copy(ptr, results, 0, length);

                return results;

            }
            finally
            {
                if (bitmapData != null)
                    bitmap.UnlockBits(bitmapData);
            }
        }

        /// <summary>
        ///  Converte esse buffer em um bitmap de 32GB ARGB. 
        ///  Os parâmetros width e height devem ser a soma do produto do comprimento imageData. 
        ///  Você especificar um buffer de comprimento 1000 e uma largura de 10, a altura deve ser 100.
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="width">Largura do bitmap resultante em bytes</param>
        /// <param name="height">Altura do bitmap resultante em bytes</param>
        /// <returns>Instância de bitmap. Não se esqueça de descartá-lo quando terminar.</returns>
        public static Bitmap AsBitmap(this byte[] imageData, int width, int height)
        {

            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            // Bloqueia o bitmap inteiro
            BitmapData bitmapData = result.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);

            // Como nossos dados são fornecidos no modo pixel expandido em bytes, ajuste a largura para 4x
            width = result.Width << 2;

#if SAFE

#error Safe mode not implemented

#else
            unsafe
            {
                // Obter um ponteiro para o início da região de dados de pixel
                // O canto superior esquerdo
                int* pixelPtr = (int*)bitmapData.Scan0;

                // Iterar através de linhas e colunas
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col += 4)
                    {
                        int index = row * width + col;
                        int color = imageData[index++] |
                                    imageData[index++] << 8 |
                                    imageData[index++] << 16 |
                                    imageData[index++] << 24;

                        // Definir o pixel (rápido!)
                        *pixelPtr = color;

                        // Atualiza o ponteiro
                        pixelPtr++;
                    }
                }
            }
#endif

            // Desbloqueie o bitmap
            result.UnlockBits(bitmapData);

            return result;

        }

        /// <summary>
        /// Converter em um bitmap na memória
        /// </summary>
        /// <param name="data">BitmapImage</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this BitmapImage data)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(data));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        /// <summary>
        /// Inverte os pixels deste bitmap no lugar.Ignora o canal alfa.
        /// </summary>
        /// <param name="bitmapImage"></param>
        public static void InvertColorChannels(this Bitmap bitmapImage)
        {
            var rect = new Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height);

            var bmpRO = bitmapImage.LockBits(
                rect,
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppPArgb);

            var bmpLen = bmpRO.Stride * bmpRO.Height;
            var bitmapBGRA = new byte[bmpLen];
            Marshal.Copy(bmpRO.Scan0, bitmapBGRA, 0, bmpLen);
            bitmapImage.UnlockBits(bmpRO);

            // Copie SOMENTE os canais de cor e inverta - preto -> branco, branco -> preto
            for (int i = 0; i < bmpLen; i += 4)
            {
                bitmapBGRA[i] = (byte)(255 - bitmapBGRA[i]);
                bitmapBGRA[i + 1] = (byte)(255 - bitmapBGRA[i + 1]);
                bitmapBGRA[i + 2] = (byte)(255 - bitmapBGRA[i + 2]);
            }

            var bmpWO = bitmapImage.LockBits(
                rect,
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppPArgb);

            Marshal.Copy(bitmapBGRA, 0, bmpWO.Scan0, bmpLen);
            bitmapImage.UnlockBits(bmpWO);
        }


        private const string LogoDelimiter = "||||";

        /// <summary>
        /// Converte esta imagem em uma string codificada em base64.
        /// </summary>
        /// <param name="bitmapImage">Imagem de origem</param>
        /// <returns>string</returns>
        public static string ToBase64String(this Bitmap bitmap)
        {
            // Não codifique imagem nula ou vazia
            if (bitmap == null || (bitmap.Width == 0 && bitmap.Height == 0))
            {
                return string.Empty;
            }

            // Extrair imagem de bitmap em bitmap e salvar na memória
            using (MemoryStream m = new MemoryStream())
            {
                bitmap.Save(m, ImageFormat.Bmp);
                var imageBytes = m.ToArray();

                return Convert.ToBase64String(imageBytes);
            }
        }

        /// <summary>
        /// Converte string codificada base64 em bitmap.
        /// </summary>
        /// <param name="content">conteudo para converter</param>
        /// <returns>Bitmap ou null se erro</returns>
        public static Bitmap FromBase64String(string content)
        {
            try
            {
                var raw = Convert.FromBase64String(content);

                // Não descarte o stream. Bitmap agora possui e irá fechá-lo quando descartado.
                var ms = new MemoryStream(raw, 0, raw.Length);
                return Bitmap.FromStream(ms, true) as Bitmap;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///  Cria um buffer ordenado MSB, com bit reverso, dos dados do logotipo localizados neste bitmap. 
        ///  Os pixels dos dados de entrada são reduzidos a uma imagem de 8bpp. 
        ///  Isso significa que 8 pixels de bitmap do PC são reduzidos em 8 bits no buffer resultante. 
        ///  Esses bits são entregues na ordem inversa (0x80: 0b10000000 -&gt; 0x00000001).
        ///  Se QUALQUER um dos valores RGB do pixel for diferente de zero, 
        ///  o índice de bits correspondente será definido no buffer de saída. O canal alfa não tem efeito no buffer de saída. 
        ///  O bitmap é lido do canto superior esquerdo do bitmap para o canto inferior direito.
        /// </summary>
        /// <param name="bitmapImage">Bitmap</param>
        /// <returns>MSB ordered, bit-reversed Buffer</returns>
        public static byte[] Rasterize(this Bitmap bitmapImage)
        {
            // Definir uma área na qual os bits de bitmap serão bloqueados na memória gerenciada
            var rect = new Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height);
            var bmpReadOnly = bitmapImage.LockBits(
                rect,
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppPArgb);


            var bmpLen = bmpReadOnly.Stride * bmpReadOnly.Height;
            var bmpChannels = new byte[bmpLen];
            Marshal.Copy(bmpReadOnly.Scan0, bmpChannels, 0, bmpLen);
            bitmapImage.UnlockBits(bmpReadOnly);

            // Dividir em bitmap em N número de linhas onde cada linha é
            // tão grande quanto a contagem de pixels do bitmap de entrada.
            var rowWidth = bmpReadOnly.Width;
            var pixels = bmpChannels.Split(bmpReadOnly.Stride);
            var byteWidth = (int)Math.Ceiling((double)rowWidth / 8);
            var tmpBuff = new List<Pixel>();

            // Buffer de resultado - Use array porque usamos | operador em reversão de bytes
            var outBuffer = new byte[byteWidth * bmpReadOnly.Height];
            var outIndex = 0;

            // Lê 1 linha (stride aka) ou pixels de 4 bytes de cada vez
            foreach (var row in pixels)
            {
                // Lê 1º de cada 4 bytes da fonte [colorIndex] em ordem no buffer temporário
                foreach (var pix in row.Split(4))
                {
                    tmpBuff.Add(new Pixel(pix));
                }

                // Inverte o byte de pixel, 0b10000010 -> 0x01000001
                for (var set = 0; set < byteWidth; set++)
                {
                    // Max bit tells us what bit to start shifting from
                    var maxBit = Math.Min(7, rowWidth - (set * 8) - 1);

                    // Leia até 8 bytes por vez em LSB-> MSB para que eles sejam transmitidos MSB-> LSB para impressora
                    // define grupos de deslocamento em bytes
                    for (int b = maxBit, bb = 0; b >= 0; b--, bb++)
                    {
                        // Lê linhas da direita para a esquerda
                        var px = tmpBuff[b + (set * 8)];

                        // Firmware preto == 1, branco == 0
                        outBuffer[outIndex] |= (byte)((px.IsNotWhite() ? 1 : 0) << bb);
                    }

                    // Incrementos após cada byte
                    outIndex++;
                }

                tmpBuff.Clear();
            }


            return outBuffer;
        }
    }
}
