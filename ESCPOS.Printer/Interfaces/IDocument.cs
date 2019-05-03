using ESCPOS.Printer.Common.Enums;
using System.Collections.Generic;

namespace ESCPOS.Printer.Interfaces
{
    /// <summary>
    /// Contém uma seqüência ordenada de seções
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// Obtém ou define a lista ordenada de seções neste documento
        /// </summary>
        IList<ISection> Sections { get; set; }

        /// <summary>
        /// Obtém ou define o código de página para este documento
        /// </summary>
        CodePagesEnum CodePage { get; set; }
    }
}
