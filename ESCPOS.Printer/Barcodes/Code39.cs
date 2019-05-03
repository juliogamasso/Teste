namespace ESCPOS.Printer.Barcodes
{
    /// <inheritdoc />
    /// <summary>
    /// A implementação do código de barras Code39 suporta A-Z, a-z, 0-9 e
    /// espaço, $,%, +, -,., / ,: (sem vírgula)
    /// </summary>
    public class Code39 : BaseBarcode
    {
        public override byte[] Build()
        {
            if (string.IsNullOrEmpty(EncodeThis))
            {
                return new byte[0];
            }

            // Se o usuário solicitar explicitamente o formulário 2, caso contrário, suponha 1
            var m = (byte)(Form == 2 ? 69 : 4);

            var payload = Preamble();
            payload.AddRange(new byte[] { 0x1D, 0x6B, m });

            // modo 2 requer comprimento
            if (m == 69)
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
