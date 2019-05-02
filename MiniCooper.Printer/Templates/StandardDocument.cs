using ESCPOS.Printer.Common.Enums;
using ESCPOS.Printer.Interfaces;
using System.Collections.Generic;

namespace ESCPOS.Printer.Templates
{

    public class StandardDocument : IDocument
    {
        public StandardDocument()
        {
            Sections = new List<ISection>();
        }

        public IList<ISection> Sections { get; set; }

        public virtual CodePagesEnum CodePage { get; set; }
    }
}
