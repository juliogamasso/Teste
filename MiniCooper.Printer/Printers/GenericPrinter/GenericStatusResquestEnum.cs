namespace ESCPOS.Printer.Printers.GenericPrinter
{
    enum GenericStatusResquestEnum
    {
        /// <summary>
        /// Transmitir o status da impressora
        /// </summary>
        Status = 1,

        /// <summary>
        /// Transmitir o status da impressora off-line
        /// </summary>
        OffLineStatus = 2,

        /// <summary>
        /// Transmitir status de erro
        /// </summary>
        ErrorStatus = 3,

        /// <summary>
        /// Transmitir status do sensor de rolo de papel
        /// </summary>
        PaperRollStatus = 4,

        /// <summary>
        /// Solicitar todos os status
        /// </summary>
        FullStatus = 20,
    }
}
