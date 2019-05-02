using ESCPOS.Printer.Common.Enums;

namespace ESCPOS.Printer.Barcodes.Enums
{
    public interface IBarcode
    {
        /// <summary>
        /// String para codificar
        /// </summary>
        string EncodeThis { get; set; }

        /// <summary>
        /// A forma de código de barras. pode ser o formulário 1 ou o formulário 2
        /// Default: Form 1
        /// </summary>
        byte Form { get; set; }

        /// <summary>
        /// Parâmetro de altura do código de barras em pontos. 1 ponto = 1/8 mm
        /// </summary>
        byte BarcodeDotHeight { get; set; }

        /// <summary>
        /// Multiplicador de largura de código de barras, isso multiplica toda a largura do código de barras. 
        /// Um código de barras não calibrado é 1 ponto de largura (1 / 8mm). 
        /// Código128 com uma escala de 1 pode não ser legível por alguns leitores.
        /// </summary>
        byte BarcodeWidthMultiplier { get; set; }

        /// <summary>
        /// Onde colocar a string HRI
        /// </summary>
        HRIPositions HriPosition { get; set; }

        /// <summary>
        /// Qual fonte utilizar para a fonte HRI. Apenas opções A
        /// e B podem ser usados.
        /// </summary>
        ThermalFontsEnum BarcodeFont { get; set; }

        /// <summary>
        /// Constrói um payload do código de barras.Se houver algum problema,
        /// uma payload vazia será retornada.
        /// </summary>
        /// <returns>byte[] payload ou vazio em erro</returns>
        byte[] Build();
    }
}
