using ESCPOS.Printer.Common.Enums;
using ESCPOS.PrinterC;

namespace ESCPOS.Printer.Templates
{
    internal abstract class ImageSection : StandardSection
    {
        /// <summary>
        /// Uma imagem não tem conteúdo de texto
        /// </summary>
        public override string Content { get { return string.Empty; } set { } }

        /// <summary>
        /// Imagens não suportam efeitos de fonte
        /// </summary>
        public override FontEffectsEnum Effects { get { return FontEffectsEnum.None; } set { } }

        /// <summary>
        /// Imagens não suportam escala de largura
        /// </summary>
        public override FontWidthScalarEnum WidthScalar { get { return FontWidthScalarEnum.w1; } set { } }

        /// <summary>
        /// Imagens não suportam escalas de altura
        /// </summary>
        public override FontHeighScalarEnum HeightScalar { get { return FontHeighScalarEnum.h1; } set { } }

        /// <summary>
        /// Imagem para colocar dentro do documento
        /// </summary>
        public PrinterImage Image { get; set; }

        public abstract override byte[] GetContentBuffer(CodePagesEnum codepage);
    }
}
