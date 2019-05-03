namespace ESCPOS.Printer.Common.Enums
{
    [System.Flags]
    public enum ReturnCodeEnum
    {
        Success = 0,

        // Problemas de conexão
        ConnectionAlreadyOpen,
        ConnectionNotFound,

        // Problemas de sintaxe
        UnsupportedCommand,
        InvalidArgument,

        // Problemas gerais
        ExecutionFailure,
    }
}
