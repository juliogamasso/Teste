namespace ESCPOS.Printer.Common.Enums
{
    /// <summary>
    /// Efeitos de fontes ESC/POS
    /// </summary>
    [System.Flags]
    public enum FontEffectsEnum
    {
        None = 0,
        Bold = 1 << 0,
        Italic = 1 << 1,
        Underline = 1 << 2,
        Rotated = 1 << 3,
        Reversed = 1 << 4,
        UpsideDown = 1 << 5,
    }
}
