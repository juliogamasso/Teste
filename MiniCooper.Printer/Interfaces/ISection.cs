using ESCPOS.Printer.Common.Enums;

namespace ESCPOS.Printer.Interfaces
{
    public interface ISection
    {
        /// <summary>
        /// Conteúdo de texto para esta seção
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// Todos os efeitos para aplicar ao conteúdo. 
        /// Isso pode ser mascarado juntos para aplicar mais de um efeito. 
        /// Estes efeitos só estarão ativos durante a impressão deste documento e serão apagados.
        /// </summary>
        FontEffectsEnum Effects { get; set; }

        /// <summary>
        /// Obtém ou define alinhamento para este documento
        /// </summary>
        FontAlignment Justification { get; set; }

        /// <summary>
        /// Obtém ou define a escala de largura para este documento
        /// </summary>
        FontWidthScalarEnum WidthScalar { get; set; }

        /// <summary>
        /// Obtém ou define a escala de altura para este documento
        /// </summary>
        FontHeighScalarEnum HeightScalar { get; set; }

        /// <summary>
        /// Obtém ou define a fonte a ser usada para esta seção
        /// </summary>
        ThermalFontsEnum Font { get; set; }

        /// <summary>
        /// Aplicar automaticamente uma nova linha após este documento
        /// </summary>
        bool AutoNewline { get; set; }

        /// <summary>
        /// Retorna a parte de dados do conteúdo como matriz de bytes
        /// </summary>
        /// <param name="codepage">Página de código para codificar texto</param>
        /// <returns></returns>
        byte[] GetContentBuffer(CodePagesEnum codepage);
    }
}
