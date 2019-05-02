using ESCPOS.Printer.Common.Enums;
using ESCPOS.Printer.Interfaces;

namespace ESCPOS.Printer.Templates
{
    /// <summary>
    /// Implementação sem nenhum comando de formatação
    /// </summary>
    public class Placeholder : ISection
    {
        public string Content { get { return string.Empty; } set { } }

        public FontEffectsEnum Effects { get { return FontEffectsEnum.None; } set { } }

        public FontAlignment Justification { get { return FontAlignment.NOP; } set { } }

        public FontWidthScalarEnum WidthScalar { get { return FontWidthScalarEnum.NOP; } set { } }

        public FontHeighScalarEnum HeightScalar { get { return FontHeighScalarEnum.NOP; } set { } }

        public ThermalFontsEnum Font { get { return ThermalFontsEnum.NOP; } set { } }

        public bool AutoNewline { get { return false; } set { } }

        /// <summary>
        /// Retorna o buffer vazio
        /// </summary>
        /// <param name="codepage">Não usado</param>
        /// <returns>array de bytes de comprimento zero</returns>
        public byte[] GetContentBuffer(CodePagesEnum codepage)
        {
            return new byte[0];
        }
    }
}
