namespace ESCPOS.Printer.Common.Enums
{
    public enum StatusTypesEnum
    {
        /// <summary>
        /// Pronto ou falha
        /// </summary>
        PrinterStatus,

        /// <summary>
        /// Status online
        /// </summary>
        OfflineStatus,

        /// <summary>
        /// Todas as mensagens de erro
        /// </summary>
        ErrorStatus,

        /// <summary>
        /// Papel baixo, papel presente
        /// </summary>
        PaperStatus,

        /// <summary>
        /// Imprimindo ou movendo o motor
        /// </summary>
        MovementStatus,

        /// <summary>
        /// Todos os status
        /// </summary>
        FullStatus,
    }
}
