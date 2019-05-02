using ESCPOS.Printer.Common.Enums;
using ESCPOS.Printer.Interfaces;
using System.Text;

namespace ESCPOS.Printer.Templates
{
    /// <summary>
    /// Implementação de documento padrão
    /// </summary>
    public class StandardSection : ISection
    {
        public virtual string Content { get; set; }

        public virtual FontEffectsEnum Effects { get; set; }

        public virtual FontAlignment Justification { get; set; }

        public virtual FontWidthScalarEnum WidthScalar { get; set; }

        public virtual FontHeighScalarEnum HeightScalar { get; set; }

        public virtual ThermalFontsEnum Font { get; set; }

        public virtual bool AutoNewline { get; set; }

        public virtual byte[] GetContentBuffer(CodePagesEnum codepage)
        {

            if (string.IsNullOrEmpty(Content))
            {
                return new byte[0];
            }

            Encoding encoder;
            switch (codepage)
            {
                case CodePagesEnum.CP771:
                    // This is the most similar to 771
                    encoder = Encoding.GetEncoding(866);
                    break;

                case CodePagesEnum.CP437:
                    encoder = Encoding.GetEncoding(437);
                    break;

                case CodePagesEnum.ASCII:
                    Content = System.Text.RegularExpressions.Regex.Replace(Content,
                        @"[^\u0020-\u007E]", string.Empty);
                    encoder = System.Text.ASCIIEncoding.ASCII;
                    break;

                default:
                    encoder = Encoding.GetEncoding(866);
                    break;
            }

            return encoder.GetBytes(Content);
        }
    }
}
