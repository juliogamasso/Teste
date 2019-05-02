using System.Collections.Generic;
using System.Text;

namespace ESCPOS.Printer.Barcodes
{
    public class QrCode : BaseBarcode
    {
        private int _size;
        private string _data;


        public QrCode(int size, string data)
        {
            _size = size;
            _data = data;
        }

        public override byte[] Build()
        {
            if (string.IsNullOrEmpty(_data))
            {
                return new byte[0];
            }

            int l = _data.Length + 3;
            List<byte> b = new List<byte>();
            //function 65 - mode
            b.AddRange(new byte[] { 0x1d, 0x28, 0x6b, 0x04, 0x00, 0x31, 0x41, 0x32, 0x00 });
            //function 67 - size
            b.AddRange(new byte[] { 0x1d, 0x28, 0x6b, 0x03, 0x00, 0x31, 0x43 });
            b.Add((byte)_size);
            //function 80 - save data        
            b.Add(0x1d);
            b.Add(0x28);
            b.Add(0x6b);
            b.Add((byte)(l % 256));
            b.Add((byte)(l / 256));
            b.Add(0x31);
            b.Add(0x50);
            b.Add(0x30);
            b.AddRange(Encoding.ASCII.GetBytes(_data));
            //function 81 - print data
            b.AddRange(new byte[] { 0x1d, 0x28, 0x6b, 0x03, 0x00, 0x31, 0x51, 0x30 });
            return b.ToArray();
        }

    }
}
