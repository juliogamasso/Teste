using ESCPOS.Printer.Common;
using System.IO.Ports;

namespace ESCPOS.Printer.Printers.GenericPrinter
{
    /// <inheritdoc />
    class GenericSerialPort : BaseSerialPort
    {
        #region Default SerialPort Params
        private const int DefaultBaudRate = 9600;
        private const int DefaultDatabits = 8;
        private const Parity DefaultParity = Parity.None;
        private const StopBits DefaultStopbits = StopBits.One;
        private const Handshake DefaultHandshake = Handshake.None;
        #endregion

        #region Constructor

        public GenericSerialPort(string portName)
            : this(portName, DefaultBaudRate)
        { }

        public GenericSerialPort(string portName, int baud)
            : base(portName, baud, DefaultDatabits, DefaultParity, DefaultStopbits, DefaultHandshake)
        { }

        #endregion
    }
}
