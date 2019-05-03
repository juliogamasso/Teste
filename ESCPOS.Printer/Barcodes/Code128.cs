namespace ESCPOS.Printer.Barcodes
{
    /// <summary>O código 128 possui modos estendidos, cada um suportando um subconjunto de caracteres.</summary>
    /// <inheritdoc />
    public class Code128 : BaseBarcode
    {
        /// <summary>
        /// Submodos do Código 128
        /// <see href="https://en.wikipedia.org/wiki/Code_128#Subtypes"/>
        /// </summary>
        public enum Modes
        {
            /// <summary>
            /// ASCII 0-95, caracteres especiais e FNC 1-4
            /// </summary>
            A,
            /// <summary>
            /// ASCII 32-127, caracteres especiais e FNC-1-4
            /// </summary>
            B,
            /// <summary>
            /// 00-99 codifica dois dígitos com um único ponto de código
            /// </summary>
            C
        }

        /// <summary>
        /// Obtém ou define o modo de codificação
        /// </summary>
        public Modes Mode { get; set; }

        /// <summary>
        /// Codifica como Código 128. Qualquer caractere não suportado em sua configuração será omitido do código de barras final. Se alguma configuração ilegal for encontrada, uma payload vazia será retornada.
        /// </summary>
        /// <returns>Payload ou vazio em erro</returns>
        /// <inheritdoc />
        public override byte[] Build()
        {
            if (string.IsNullOrEmpty(EncodeThis))
            {
                return new byte[0];
            }

            // Se o usuário solicitar explicitamente o formulário 2, caso contrário, assuma 1
            var m = (byte)(Form == 2 ? 73 : 8);

            var payload = Preamble();
            payload.AddRange(new byte[] { 0x1D, 0x6B, m });

            // modo 2 requer comprimento
            if (m == 73)
            {
                payload.Add((byte)EncodeThis.Length);
            }

            // Adicionar bytes de modo
            var mode = Mode == Modes.A ? 0x41 : Mode == Modes.B ? 0x42 : 0x43;
            payload.AddRange(new byte[] { 0x7B, (byte)mode });

            var bytes = System.Text.Encoding.ASCII.GetBytes(EncodeThis);
            payload.AddRange(bytes);

            // Forçar sequência terminando com nulo
            if (!EncodeThis.EndsWith("\0"))
            {
                payload.Add(0);
            }

            return payload.ToArray();
        }

    }
}
