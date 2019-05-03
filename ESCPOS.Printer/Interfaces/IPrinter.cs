using ESCPOS.Printer.Common;
using ESCPOS.Printer.Common.Enums;

namespace ESCPOS.Printer.Interfaces
{
    public interface IPrinter : System.IDisposable
    {
        /// <summary>
        /// Obtém os efeitos de fonte ativos
        /// </summary>
        FontEffectsEnum Effects { get; }

        /// <summary>
        /// Obtém ou define o alinhamento ativo
        /// </summary>
        FontAlignment Alignment { get; }

        /// <summary>
        /// Obtém ou define a escala de altura da fonte
        /// </summary>
        FontHeighScalarEnum Height { get; }

        /// <summary>
        /// Obtém ou define a escala de largura da fonte
        /// </summary>
        FontWidthScalarEnum Width { get; }

        /// <summary>
        /// Obtém a fonte ativa
        /// </summary>
        ThermalFontsEnum Font { get; }

        /// <summary>
        /// Retorna o relatório de status especificado para esta impressora
        /// </summary>
        /// <param name="type">Tipo de consulta de status</param>
        /// <returns>Status report</returns>
        StatusReport GetStatus(StatusTypesEnum type);

        /// <summary>
        /// Define a fonte ativa.
        /// </summary>
        /// <param name="font">Tipo da fonte</param>
        void SetFont(ThermalFontsEnum font);

        /// <summary>
        /// Aplica as escalas especificadas
        /// </summary>
        /// <param name="w">Escala de largura - Width</param>
        /// <param name="h">Escala de algura - Height</param>
        void SetScalars(FontWidthScalarEnum w, FontHeighScalarEnum h);

        /// <summary>
        /// Aplica o alinhamento especificado
        /// </summary>
        /// <param name="justification">Tipo de alinhamento</param>
        void SetAlignment(FontAlignment justification);

        /// <summary>
        /// Ativa o efeito para a próxima impressão. 
        /// Esse efeito pode ser bit a bit OU usar vários efeitos de uma só vez. 
        /// Se houver algum efeito conflitante, a impressora terá a palavra final sobre o comportamento definido.
        /// </summary>
        /// <param name="effect">Efeito de fonte para aplicar</param>
        void AddEffect(FontEffectsEnum effect);

        /// <summary> Remover efeito da lista de efeitos ativos. 
        /// Se o efeito não estiver atualmente na lista de efeitos ativos, nada acontecerá.</summary>
        /// <param name="effect">Efeito a ser removido</param>
        void RemoveEffect(FontEffectsEnum effect);

        /// <summary>
        /// Remova todos os efeitos imediatamente. Aplica-se apenas
        /// aos dados que ainda não foram transmitidos.
        /// </summary>
        void ClearAllEffects();

        /// <summary>
        /// Define todas as opções ESC / POS como padrão
        /// </summary>
        void Reinitialize();

        /// <summary>
        /// Imprimir string como texto ASCII. Quaisquer efeitos que estão atualmente
        /// ativo será aplicado a essa string.
        /// </summary>
        /// <param name="str">ASCII para ser impresso</param>
        void PrintASCIIString(string str);

        /// <summary>
        /// Imprime o documento especificado
        /// </summary>
        /// <param name="doc">Documento para imprimir</param>
        void PrintDocument(IDocument doc);

        /// <summary>Define a imagem para uma posição dentro do documento especificado pelo índice.</summary>
        /// <param name="image">Imagem a ser adicionada</param>
        /// <param name="doc">Documento para adicionar a imagem</param>
        /// <param name="index">Índice para inserir. Se esse índice exceder o espaço atual, 
        /// os espaços reservados serão inseridos até que o índice seja atingido.</param>
        /// <example>
        /// var header = new StandardSection()
        /// {
        ///   Justification = FontJustification.JustifyCenter,
        ///   HeightScalar = FontHeighScalar.h2,
        ///   WidthScalar = FontWidthScalar.w2,
        ///   AutoNewline = true,
        /// };
        /// var document = new StandardDocument();
        /// document.Sections.Add(header);
        /// var someImage = Webcam.GrabPicture()
        /// myPrinter.SetImage(someImage, document, 1);
        /// </example>
        void SetImage(Imaging.PrinterImage image, IDocument doc, int index);

        /// <summary>
        /// Emitir um caractere de nova linha e retornar a impressão
        /// posição para o início da linha.
        /// </summary>
        void PrintNewline();

        /// <summary>
        /// Marcar ingresso como completo e pular linhas
        /// <param name="lineCount">Quantidade de linhas a serem puladas</param>
        /// </summary>
        void FormFeed(int lineCount);

        /// <summary>
        /// Cortar papel 
        /// <param name="cutMode">(0 - Corte total, 1 - Corte parcial)</param>
        /// </summary>
        void Cut(CutModeEnum cutMode);

        /// <summary>
        /// Enviar buffer bruto para a impressora de destino.
        /// </summary>
        /// <param name="raw"></param>
        void SendRaw(byte[] raw);
    }
}
