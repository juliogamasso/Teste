using System.Linq;

namespace ESCPOS.Printer.Barcodes
{
    /// <inheritdoc />
    /// <summary>
    /// ITF é um formato de código de barras apenas numérico
    /// </summary>
    public class ITF : BaseBarcode
    {
        public override byte[] Build()
        {
            if (string.IsNullOrEmpty(EncodeThis))
            {
                return new byte[0];
            }

            // deve ser o mesmo comprimento
            if (EncodeThis.Length % 2 != 0)
            {
                return new byte[0];
            }

            // deve ser numérico
            if (EncodeThis.ToCharArray().Any(ch => !char.IsDigit(ch)))
            {
                return new byte[0];
            }

            // Se o usuário solicitar explicitamente o formulário 2, caso contrário, suponha 1
            var m = (byte)(Form == 2 ? 70 : 5);

            var payload = Preamble();
            payload.AddRange(new byte[] { 0x1D, 0x6B, m });

            // modo 2 requer comprimento
            if (m == 70)
            {
                payload.Add((byte)EncodeThis.Length);
            }

            var bytes = System.Text.Encoding.ASCII.GetBytes(EncodeThis);
            payload.AddRange(bytes);

            // Forçar string terminando com nulo
            if (!EncodeThis.EndsWith("\0"))
            {
                payload.Add(0);
            }

            return payload.ToArray();
        }
    }
}
