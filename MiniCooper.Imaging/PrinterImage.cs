using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace ESCPOS.PrinterC
{
    public class PrinterImage : IPrintLogo
    {
        /// <summary>
        /// Construa um novo logotipo a partir da imagem de origem no disco
        /// </summary>
        /// <param name="sourcePath">Caminho da string para o arquivo.Suporta todos os formatos de imagem</param>
        public PrinterImage(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException("sourcePath deve ser uma string não vazia.");
            }

            SetImageData(sourcePath);
        }

        /// <summary>
        /// Construa um novo logotipo a partir da imagem de origem
        /// </summary>
        /// <param name="sourcePath">Source image</param>
        public PrinterImage(Bitmap source)
        {
            SetImageData(source);
        }

        ~PrinterImage()
        {
            Dispose(false);
        }

        #region Properties
        /// <summary>
        /// Retorna uma versão somente leitura dos dados da imagem de apoio
        /// </summary>
        /// <remarks>Acesso privado, use SetImageData</remarks>
        public BitmapImage ImageData { get; private set; }

        /// <summary>
        /// Obtém a largura do bitmap atual em pixels
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Obtém a altura do bitmap atual em pixels
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Retorna o tamanho em bytes para este bitmap
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Retorna verdadeiro se esta imagem estiver invertida
        /// </summary>
        public bool IsInverted { get; private set; }
        #endregion


        public void ApplyDithering(AlgorithmsEnum algorithm, byte threshhold)
        {
            // Create an instance of the specified dithering algorithm
            var algo = algorithm;
            var halftoneProcessor = DitherFactory.GetDitherer(algo, threshhold);

            var bitmap = ImageData.ToBitmap();

            // The big grind
            var dithered = halftoneProcessor.GenerateDithered(bitmap);

            // Update ImageData with dithered result
            SetImageData(dithered);

            // For Phoenix we don't care about size of logo in flash. Everything is static.
            Size = dithered.Rasterize().Length;
        }

        /// <summary>
        ///  Aplique a inversão de cores a esta imagem. Inversão é relativa à sourceimage.
        ///  A imagem começa no estado não invertido. Chamar ApplyColorInversiononce colocará essa imagem no estado revertido.
        ///  Chamar duas vezes retornará ao estado não invertido, etc.
        /// </summary>
        public void ApplyColorInversion()
        {
            IsInverted = !IsInverted;
            var bitmap = ImageData.ToBitmap();
            bitmap.InvertColorChannels();
            SetImageData(bitmap);
        }

        /// <summary>
        /// Salvar o estado atual deste logotipo como um bitmap no caminho especificado
        /// </summary>
        /// <param name="outpath">Output path</param>
        public void ExportLogo(string outpath)
        {
            ImageData.ToBitmap().Save(outpath);
        }

        /// <summary>
        /// Exportar o estado atual deste logotipo como um arquivo binário no caminho específico
        /// </summary>
        /// <param name="outpath">Outpuat path</param>
        public void ExportLogoBin(string outpath)
        {
            // Append the bitmap data as a packed dot logo
            var bmpData = ImageData.ToBitmap().Rasterize();

            // Write to file
            File.WriteAllBytes(outpath, bmpData);
        }

        /// <summary>
        /// Empacote este bitmap como uma imagem de varredura ESC / POS que é a
        /// 1D 76 comando.
        /// </summary>
        /// <returns>Byte buffer</returns>
        public byte[] GetAsRaster()
        {
            // Build up the ESC/POS 1D 76 30 command
            var buffer = new List<byte>();
            buffer.Add(0x1D);
            buffer.Add(0x76);
            buffer.Add(0x30);

            // Normal width for now
            buffer.Add(0x00);

            // Get correct dimensions
            var w = (int)Math.Ceiling((double)Width / 8);
            var h = Height;

            // https://goo.gl/FFdiZl
            // Calculate xL and xH
            var xH = (byte)(w / 256);
            var xL = (byte)(w - (xH * 256));

            // Calculate yL and yH
            var yH = (byte)(h / 256);
            var yL = (byte)(h - (yH * 256));

            // Pack up these dimensions
            buffer.Add(xL);
            buffer.Add(xH);
            buffer.Add(yL);
            buffer.Add(yH);

            // Append the bitmap data as a packed dot logo
            var bmpData = ImageData.ToBitmap().Rasterize();
            buffer.AddRange(bmpData);

            return buffer.ToArray();
        }

        /// <summary>
        /// Exportar o estado atual deste logotipo como um arquivo binário, envolto no 1D 76
        /// Comando bitmap ESC / POS.
        /// </summary>
        /// <param name="outpath"></param>
        public void ExportLogoEscPos(string outpath)
        {
            var buffer = GetAsRaster();

            File.WriteAllBytes(outpath, buffer);
        }

        /// <summary>
        /// Retorna este logotipo codificado como bitmap
        /// </summary>
        /// <returns></returns>
        public string AsBase64String()
        {
            using (var bitmap = ImageData.ToBitmap())
            {
                return bitmap.ToBase64String();
            };
        }

        /// <summary>
        /// Definir os dados de bitmap de uma string base64 codificada
        /// </summary>
        /// <param name="base64">String codificada em Base64</param>
        public void FromBase64String(string base64)
        {
            using (Bitmap bitmap = ImageExt.FromBase64String(base64))
            {
                SetImageData(bitmap);
            }
        }

        /// <summary>
        /// Redimensione o logotipo usando as dimensões específicas. 
        /// Para ajustar uma única dimensão, defina o método ManterReatado como verdadeiro e o primeiro parâmetro diferente de zero. 
        /// A altura final da mão larga, mesmo se você passar a largura e a altura atuais como parâmetros, 
        /// sempre será arredondada para ser uniformemente divisível por 8. Por exemplo, se yourimage for 60x60 e você chamar 
        /// Redimensionar (60,60), o resultado terminará como 64x64due ao arredondamento.
        /// </summary>
        /// <param name="pixelWidth">Largura desejada em pixels, diferente de zero, se maintainAspectRatio for falso</param>
        /// <param name="pixelHeight">Altura desejada em pixels, diferente de zero, se maintainAspectRatio for falso</param>
        /// <param name="maintainAspectRatio">Manter relação atual</param>
        /// <exception cref="ImagingException">Gerado se as dimensões inválidas forem especificadas</exception>
        /// <example>
        /// myLogo.Resize(100, 0, true);  // Scale to 100 pixels wide and maintain ratio
        /// myLogo.Resize(0, 400, true);  // Scale to 400 pixel tall and maintain ratio
        /// myLogo.Resize(100, 200, true);// Scale to 100 pixels wide because maintainAspectRatio is true and width is 1st
        /// myLogo.Resize(640, 480);      // Scale to 640x480 and don't worry about ratio
        /// </example>
        public void Resize(int pixelWidth, int pixelHeight, bool maintainAspectRatio = false)
        {

            if (maintainAspectRatio)
            {
                if (pixelWidth > 0)
                {
                    // Get required height scalar
                    var scalar = Width / (float)pixelWidth;

                    // Use scalar reciprocal
                    scalar = 1 / scalar;

                    Width = pixelWidth;
                    Height = (int)(Height * scalar);
                }
                else if (pixelHeight > 0)
                {
                    // Get required width scalar
                    var scalar = Height / (float)pixelHeight;

                    // Use scalar reciprocal
                    scalar = 1 / scalar;

                    Height = pixelHeight;
                    Width = (int)(Width * scalar);
                }
                else
                {
                    throw new ImagingException("Largura ou altura deve ser diferente de zero");
                }
            }
            else
            {
                if (pixelWidth == 0 || pixelHeight == 0)
                {
                    throw new ImagingException("Largura e Altura devem ser diferentes de zero");
                }

                Width = pixelWidth;
                Height = pixelHeight;
            }

            // Ensure we have byte alignment
            Width = Width.RoundUp(8);
            Height = Height.RoundUp(8);

            using (var bitmap = new Bitmap(ImageData.ToBitmap(), new Size(Width, Height)))
            {
                SetImageData(bitmap);
            }
        }

        /// <summary>
        /// Abre a imagem localizada no sourcepath. Escala a imagem até MaxWidth, se necessário. 
        /// As imagens menores que MaxWidth não serão dimensionadas. O resultado é armazenado no campo ImageData.
        /// O resultado final CRC é calculado e atribuído ao campo CRC32.
        /// </summary>
        /// <param name="sourcePath">Caminho da string para a imagem de origem</param>
        private void SetImageData(string sourcePath)
        {
            using (Bitmap bitmap = (Bitmap)Image.FromFile(sourcePath))
            {
                SetImageData(bitmap);
            }
        }

        /// <summary>
        /// Abre a imagem localizada no sourcepath.Escala a imagem até MaxWidth, se necessário.
        /// Imagens menores que MaxWidth não serão ampliadas.O resultado é armazenado no campo ImageData.
        /// </summary>
        /// <param name="sourcePath">String path to source image</param>>
        private void SetImageData(Bitmap bitmap)
        {
            // extract dimensions
            Width = bitmap.Width;
            Height = bitmap.Height;

            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                ImageData = new BitmapImage();
                ImageData.BeginInit();
                ImageData.StreamSource = memory;
                ImageData.CacheOption = BitmapCacheOption.OnLoad;
                ImageData.EndInit();

                if (ImageData.CanFreeze)
                {
                    ImageData.Freeze();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (ImageData != null)
            {
                ImageData.StreamSource.Close();
            }
        }
    }
}
