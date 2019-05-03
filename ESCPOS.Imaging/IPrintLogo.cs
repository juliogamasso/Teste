using System;
using System.Windows.Media.Imaging;

namespace ESCPOS.Imaging
{
    interface IPrintLogo : IDisposable
    {
        /// <summary>
        /// Inverte a paleta de cores neste bitmap, pixel a pixel
        /// </summary>
        void ApplyColorInversion();

        /// <summary>
        /// Aplica o algoritmo de pontilhamento especificado a este logotipo
        /// </summary>
        /// <param name="algorithm">Nome do algoritmo</param>
        /// <param name="threshhold">Limiar de cinza.Valores abaixo disso são considerados brancos</param>
        void ApplyDithering(AlgorithmsEnum algorithm, byte threshhold);

        /// <summary>
        /// Retorna este bitmap, em seu estado atual, como uma string codificada em base64
        /// </summary>
        /// <returns></returns>
        string AsBase64String();

        /// <summary>
        /// Fonte de imagem de bitmap observável que representa o bitmap em seu estado atual
        /// e refletirá todas as alterações devido a redimensionamento, pontilhamento ou inversão.
        /// </summary>
        BitmapImage ImageData { get; }

        /// <summary>
        /// Largura deste bitmap em pixels
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Altura deste bitmap em pixels
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Tamanho total deste bitmap em bytes
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Retorna true se este bitmap estiver em um estado invertido
        /// </summary>
        bool IsInverted { get; }
    }
}
