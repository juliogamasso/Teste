using ESCPOS.Printer.Barcodes.Enums;
using ESCPOS.Printer.Common;
using ESCPOS.Printer.Common.Enums;
using ESCPOS.Printer.Interfaces;
using ESCPOS.Printer.Templates;
using ESCPOS.PrinterC;
using System.Collections.Generic;
using System.Text;

namespace ESCPOS.Printer.Printers.GenericPrinter
{
    public class GenericPrinter : BasePrinter
    {
        private const int DefaultReadTimeout = 1500; // ms
        private const int DefaultBaudRate = 9600;

        private readonly byte[] FontACmd = { 0x1B, 0x21, 0x0 };
        private readonly byte[] FontBCmd = { 0x1B, 0x21, 0x1 };
        private readonly byte[] FontCCmd = { 0x1B, 0x21, 0x2 };

        public GenericPrinter(string serialPortName)
        {
            EnableCommands = new Dictionary<FontEffectsEnum, byte[]>()
            { //TODO: Revisar hexadecimal dos comandos
                { FontEffectsEnum.None, new byte[0]},
                { FontEffectsEnum.Bold, new byte[] { 0x1B, 0x45, 0x1 }},
                { FontEffectsEnum.Italic, new byte[] { 0x1B, 0x34, 0x1 }},
                { FontEffectsEnum.Underline, new byte[] { 0x1B, 0x2D, 0x1 }},
                { FontEffectsEnum.Rotated, new byte[] { 0x1B, 0x56, 0x1 }},
                { FontEffectsEnum.Reversed, new byte[] { 0x1D, 0x42, 0x1 }},
                { FontEffectsEnum.UpsideDown, new byte[] { 0x1B, 0x7B, 0x1 }},
            };

            DisableCommands = new Dictionary<FontEffectsEnum, byte[]>()
            {
                { FontEffectsEnum.None, new byte[0]},
                { FontEffectsEnum.Bold, new byte[] { 0x1B, 0x45, 0x0 }},
                { FontEffectsEnum.Italic, new byte[] { 0x1B, 0x34, 0x0 }},
                { FontEffectsEnum.Underline, new byte[] { 0x1B, 0x2D, 0x0 }},
                { FontEffectsEnum.Rotated, new byte[] { 0x1B, 0x56, 0x0 }},
                { FontEffectsEnum.Reversed, new byte[] { 0x1D, 0x42, 0x0 }},
                { FontEffectsEnum.UpsideDown, new byte[] { 0x1B, 0x7B, 0x0 }},
            };

            JustificationCommands = new Dictionary<FontAlignment, byte[]>()
            {
                { FontAlignment.NOP, new byte[0]},
                { FontAlignment.Left, new byte[] { 0x1B, 0x21, 0x20 }},
                { FontAlignment.Center, new byte[] { 0x1B, 0x61, 0x01 }},
                { FontAlignment.Right, new byte[] { 0x1B, 0x61, 0x02 }},
            };

            SetScalarCommand = new byte[] { 0x1D, 0x21, 0x00 };  //29-33-n last byte set by tx func
            FormFeedCommand = new byte[] { 0x1B, 0x64, 0x00 }; //27-100-n(paper cut)27-109
            NewLineCommand = new byte[] { 0x0A }; //10
            InitPrinterCommand = new byte[] { 0x1B, 0x40 }; //27-64
            CutCommand = new byte[] { 0x1D, 0x56, 0x00 }; //29-86-n(tipo de corte)
            ReverseCommand = new byte[] { 0x1D, 0x42, 0x1 };

            PrintSerialReadTimeout = DefaultReadTimeout;
            PrintSerialBaudRate = DefaultBaudRate;

            // O usuário quer uma porta serial
            if (string.IsNullOrEmpty(serialPortName))
            {
                return;
            }

            Connection = new GenericSerialPort(serialPortName, PrintSerialBaudRate)
            {
                ReadTimeoutMS = DefaultReadTimeout
            };
        }

        public override void SetFont(ThermalFontsEnum font)
        {
            if (font == ThermalFontsEnum.NOP)
            {
                return;
            }

            switch (font)
            {
                case ThermalFontsEnum.A:
                    internalSend(FontACmd);
                    break;
                case ThermalFontsEnum.B:
                    internalSend(FontBCmd);
                    break;
                case ThermalFontsEnum.C:
                    internalSend(FontCCmd);
                    break;
            }
        }

        public override StatusReport GetStatus(StatusTypesEnum type)
        {
            ReturnCodeEnum ret;

            GenericStatusResquestEnum r;
            switch (type)
            {
                case StatusTypesEnum.PrinterStatus:
                    r = GenericStatusResquestEnum.Status;
                    break;

                case StatusTypesEnum.OfflineStatus:
                    r = GenericStatusResquestEnum.OffLineStatus;
                    break;

                case StatusTypesEnum.ErrorStatus:
                    return StatusReport.Invalid();

                case StatusTypesEnum.PaperStatus:
                    r = GenericStatusResquestEnum.PaperRollStatus;
                    break;

                case StatusTypesEnum.MovementStatus:
                    return StatusReport.Invalid(); ;

                case StatusTypesEnum.FullStatus:
                    r = GenericStatusResquestEnum.FullStatus;
                    break;

                default:
                    // Tipo de status desconhecido
                    return StatusReport.Invalid();
            }

            var rts = new StatusReport();

            if (r == GenericStatusResquestEnum.FullStatus)
            {
                ret = internalGetStatus(GenericStatusResquestEnum.Status, rts);
                ret = ret != ReturnCodeEnum.Success ? ret : internalGetStatus(GenericStatusResquestEnum.PaperRollStatus, rts);
                ret = ret != ReturnCodeEnum.Success ? ret : internalGetStatus(GenericStatusResquestEnum.OffLineStatus, rts);

                // Not supported PP-82
                //ret = ret != ReturnCode.Success ? ret : internalGetStatus(PhoenixStatusRequests.ErrorStatus, rts);
            }
            else
            {
                ret = internalGetStatus(r, rts);
            }

            // Retorna o objeto de status nulo em erro
            return ret == ReturnCodeEnum.Success ? rts : StatusReport.Invalid();
        }

        public override void Print2DBarcode(string encodeThis)
        {
            var len = encodeThis.Length > 154 ? 154 : encodeThis.Length;
            var setup = new byte[] { 0x0A, 0x1C, 0x7D, 0x25, (byte)len };

            var fullCmd = Helpers.Extensions.Concat(setup, Encoding.ASCII.GetBytes(encodeThis), new byte[] { 0x0A });
            internalSend(fullCmd);
        }

        public void PrintBarCode(IBarcode barcode)
        {
            var payload = barcode.Build();
            if (payload.Length > 0)
            {
                internalSend(payload);
            }
        }

        public override void SetImage(PrinterImage image, IDocument doc, int index)
        {
            while (index > doc.Sections.Count)
            {
                doc.Sections.Add(new Placeholder());
            }

            doc.Sections[index] = new GenericImageSection()
            {
                Image = image
            };
        }

        public override void SetImage(PrinterImage image, IDocument doc, int index, FontAlignment justification)
        {
            while (index > doc.Sections.Count)
            {
                doc.Sections.Add(new Placeholder());
            }

            doc.Sections[index] = new GenericImageSection()
            {
                Image = image,
                Justification = justification
            };
        }

        public override void SetScalars(FontWidthScalarEnum w, FontHeighScalarEnum h)
        {
            //// Só aplica a atualização se uma das propriedades mudou
            //if (w != Width || h != Height)
            //{
            base.SetScalars(w, h);
            //}
        }

        private ReturnCodeEnum internalGetStatus(GenericStatusResquestEnum r, StatusReport rts)
        {
            //Verificar se impressora suporta o comando
            if (r == GenericStatusResquestEnum.ErrorStatus)
            {
                return ReturnCodeEnum.UnsupportedCommand;
            }

            // Envie o comando de status em tempo real, r é o argumento
            var command = new byte[] { 0x10, 0x04, (byte)r };
            int respLen = 1;

            var data = new byte[0];

            try
            {
                Connection.Open();

                var written = Connection.Write(command);

                System.Threading.Thread.Sleep(100);

                // Colete a resposta
                data = Connection.Read(respLen);

            }
            catch
            {
                //Fazer nada 
            }
            finally
            {
                Connection.Close();
            }

            // Resposta inválida
            if (data.Length != respLen)
            {
                return ReturnCodeEnum.ExecutionFailure;
            }

            switch (r)
            {  //TODO: REVISAR TODOS OS STATUS => PaperRollStatus OK
                case GenericStatusResquestEnum.Status:
                    // bit 3: 0- online, 1- offline        
                    rts.IsOnline = (data[0] & 0x08) == 0;
                    break;

                case GenericStatusResquestEnum.OffLineStatus:

                    // bit 6: 0- no error, 1- error        
                    rts.HasError = (data[0] & 0x40) == 0;
                    break;

                case GenericStatusResquestEnum.ErrorStatus:
                    // bit 3: 0- okay, 1- Not okay    
                    rts.IsCutterOkay = (data[0] & 8) == 0;

                    // bit 5: 0- No fatal (non-recoverable) error, 1- Fatal error        
                    rts.HasFatalError = (data[0] & 8) == 0;

                    // bit 6: 0- No recoverable error, 1- Recoverable error        
                    rts.HasRecoverableError = (data[0] & 0x40) == 1;
                    break;

                case GenericStatusResquestEnum.PaperRollStatus:
                    //18 - Ok,  0- Error, 30 - No Paper
                    rts.IsPaperPresent = (data[0] & 0x60) == 0;
                    break;

                default:
                    rts.IsInvalidReport = true;
                    break;
            }

            return ReturnCodeEnum.Success;
        }

    }
}
