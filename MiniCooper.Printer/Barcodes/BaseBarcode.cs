using ESCPOS.Printer.Barcodes.Enums;
using ESCPOS.Printer.Common.Enums;
using System.Collections.Generic;

namespace ESCPOS.Printer.Barcodes
{
    /// <summary>Interface que define o que um gerador de código de barras deve executar e fornecer</summary>
    public abstract class BaseBarcode : IBarcode
    {
        /// <summary>
        /// Cria um código de barras padrão
        /// </summary>
        protected BaseBarcode()
        {
            Form = 1;
            BarcodeDotHeight = 100;
            BarcodeWidthMultiplier = 2;
            HriPosition = HRIPositions.NotPrinted;
            BarcodeFont = ThermalFontsEnum.A;
        }

        public string EncodeThis { get; set; }

        public byte Form { get; set; }

        public byte BarcodeDotHeight { get; set; }

        public byte BarcodeWidthMultiplier { get; set; }

        public HRIPositions HriPosition { get; set; }

        public ThermalFontsEnum BarcodeFont { get; set; }

        public abstract byte[] Build();

        /// <summary>Criar args de pré-configuração de código de barras, se houver</summary>
        /// <returns>byte[] payload ou vazio se não precisar de configuração</returns>
        protected List<byte> Preamble()
        {
            var payload = new List<byte>();

            if (BarcodeDotHeight > 0)
            {
                payload.AddRange(new byte[] { 0x1D, 0x68, BarcodeDotHeight });
            }

            if (BarcodeWidthMultiplier >= 1 && BarcodeWidthMultiplier <= 6)
            {
                payload.AddRange(new byte[] { 0x1D, 0x77, BarcodeWidthMultiplier });
            }

            payload.AddRange(new byte[] { 0x1D, 0x48, (byte)HriPosition });

            payload.AddRange(new byte[] { 0x1D, 0x66, (byte)(BarcodeFont == ThermalFontsEnum.A ? 0 : 1) });

            return payload;
        }
    }
}
