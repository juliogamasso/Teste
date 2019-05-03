using ESCPOS.Printer.Common.Enums;
using ESCPOS.Printer.Templates;

namespace ESCPOS.Printer.Printers.GenericPrinter
{
    internal sealed class GenericImageSection : ImageSection
    {
        /// <summary> Selecionar imagem no formato da impressora</summary>
        /// <param name="codepage">Não usado</param>
        /// <returns>byte array</returns>
        public override byte[] GetContentBuffer(CodePagesEnum codepage)
        {
            return base.Image.GetAsRaster();
        }
    }
}
