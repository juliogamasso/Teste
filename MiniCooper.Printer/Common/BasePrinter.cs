using ESCPOS.Printer.Common.Enums;
using ESCPOS.Printer.Helpers;
using ESCPOS.Printer.Interfaces;
using ESCPOS.PrinterC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ESCPOS.Printer.Common
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class BasePrinter : IPrinter
    {
        protected BasePrinter()
        {
            Alignment = FontAlignment.Left;
            SetScalarCommand = new byte[0];
            InitPrinterCommand = new byte[0];
            FormFeedCommand = new byte[0];
            NewLineCommand = new byte[0];
        }

        /// <summary>
        /// Destructor - Feche e descarte a porta serial, se necessário
        /// </summary>
        ~BasePrinter()
        {
            Dispose(false);
        }

        /// <summary>
        /// Obtém a conexão serial para este dispositivo
        /// </summary>
        protected ISerialConnection Connection { get; set; }

        /// <summary>
        /// Comando para aplicar escala. Adicione 0 byte extra para manter o valor de configuração
        /// Deixe em branco se não for suportado.
        /// </summary>
        protected byte[] SetScalarCommand { get; set; }

        /// <summary>
        /// Comando enviado para inicializar a impressora.
        /// Deixe em branco se não for suportado.
        /// </summary>
        protected byte[] InitPrinterCommand { get; set; }

        /// <summary>
        /// Comando enviado para executar uma nova linha e um trabalho de impressão
        /// Deixe em branco se não for suportado.
        /// </summary>
        protected byte[] FormFeedCommand { get; set; }

        /// <summary>
        /// Comando enviado para executar uma nova linha
        /// Deixe em branco se não for suportado.
        /// </summary>
        protected byte[] NewLineCommand { get; set; }

        /// <summary>
        /// Comando para cortar o papel
        /// 0 - Corte Total, 1 - Corte parcial
        /// </summary>
        protected byte[] CutCommand { get; set; }

        /// <summary>
        /// Ativar / desativar o modo de impressão inversa branco / preto
        /// 0 - Inversão desativada, 1 - Inversão ativada
        /// </summary>
        protected byte[] ReverseCommand { get; set; }

        /// <summary>
        /// Mapa de efeitos de fonte e o comando específico de byte para aplicá-los
        /// </summary>
        protected Dictionary<FontEffectsEnum, byte[]> EnableCommands { get; set; }

        /// <summary>
        /// Mapa de efeitos de fonte e o comando específico de byte para desativá-los
        /// </summary>
        protected Dictionary<FontEffectsEnum, byte[]> DisableCommands { get; set; }

        /// <summary>
        /// Mapa de comandos justifcation e o comando byte específico para aplicá-los
        /// </summary>
        protected Dictionary<FontAlignment, byte[]> JustificationCommands { get; set; }

        /// <summary>
        /// Obtém ou define o tempo limite de leitura em milissegundos
        /// </summary>
        public int PrintSerialReadTimeout { get; set; }

        /// <summary>
        /// Obtém ou define a taxa de transmissão da porta serial
        /// </summary>
        public int PrintSerialBaudRate { get; set; }

        /// <summary>
        /// Obtém ou define a altura de escala da fonte
        /// </summary>
        public FontHeighScalarEnum Height { get; private set; }

        /// <summary>
        /// Obtém ou define a largura de escala da fonte
        /// </summary>
        public FontWidthScalarEnum Width { get; private set; }

        /// <summary>
        /// Obtém os efeitos de fonte ativos      
        /// </summary>
        public FontEffectsEnum Effects { get; private set; }

        /// <summary>
        /// Obtém ou define o alinhamento ativo
        /// </summary>
        public FontAlignment Alignment { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// Obtém o tipo de fonte ativo
        /// </summary>
        public ThermalFontsEnum Font { get; private set; }

        /// <summary>
        /// Codifica a string especificada como um código de barras 2D justificado pelo centro. 
        /// Esse código de barras 2D é compatível com a especificação do QR Code® e pode ser lido por todos os 
        /// leitores de código de barras 2D.
        /// </summary>
        /// <param name="encodeThis">String para codificar</param>
        public abstract void Print2DBarcode(string encodeThis);

        /// <inheritdoc />
        /// <summary>
        /// Define uma fonte
        /// </summary>
        /// <param name="font">Font to use</param>
        public abstract void SetFont(ThermalFontsEnum font);

        /// <inheritdoc />
        /// <summary>
        /// Retorna o relatório de status especifico para esta impressora
        /// </summary>
        /// <param name="type">Tipo de consulta de status</param>
        /// <returns>Relatório de Status</returns>
        public abstract StatusReport GetStatus(StatusTypesEnum type);

        /// <inheritdoc />
        /// <summary>
        /// Envie o comando reinicializar ESC/POS, que restaura todas
        /// opções padrão, configuráveis, etc.
        /// </summary>
        public virtual void Reinitialize()
        {
            internalSend(InitPrinterCommand);
        }

        /// <inheritdoc />
        /// <summary>
        /// Aplica as escalas especificadas
        /// </summary>
        /// <param name="w">Escala de largura - Width</param>
        /// <param name="h">Escalda de altura - Height</param>
        public virtual void SetScalars(FontWidthScalarEnum w, FontHeighScalarEnum h)
        {
            // If both scalars are set to "keep current" then do nothing
            if (w == FontWidthScalarEnum.NOP && h == FontHeighScalarEnum.NOP)
            {
                return;
            }

            // Do not alter the scalars if param is set to x0 which means
            // "keep the current scalar"
            Width = w == FontWidthScalarEnum.NOP ? Width : w;
            Height = h == FontHeighScalarEnum.NOP ? Height : h;

            byte wb = (byte)w;
            byte hb = (byte)h;

            byte[] cmd = (byte[])SetScalarCommand.Clone();

            cmd[2] = (byte)(wb | hb);
            internalSend(cmd);
        }

        /// <inheritdoc />
        /// <summary>
        /// Aplica o alinhamento especificado
        /// </summary>
        /// <param name="alignment">Alinhamento para ser aplicado</param>
        public virtual void SetAlignment(FontAlignment alignment)
        {
            // If "keep current" justification is set, do nothing
            if (alignment == FontAlignment.NOP)
            {
                return;
            }

            Alignment = alignment;

            if (JustificationCommands.ContainsKey(alignment))
            {
                byte[] cmd = JustificationCommands[alignment];
                if (cmd != null)
                {
                    internalSend(cmd);
                }
            }
        }

        /// <inheritdoc />
        public virtual void AddEffect(FontEffectsEnum effect)
        {
            foreach (var flag in effect.GetFlags())
            {
                //habilita o comando e envia se não for vazio
                if (!EnableCommands.ContainsKey(flag))
                {
                    continue;
                }
                var cmd = EnableCommands[flag];
                if (cmd.Length > 0)
                {
                    internalSend(cmd);
                }
            }

            Effects |= effect;
        }

        public virtual void RemoveEffect(FontEffectsEnum effect)
        {
            foreach (var flag in effect.GetFlags())
            {
                // Lookup enable command and send if non-empty
                if (DisableCommands.ContainsKey(flag))
                {
                    var cmd = DisableCommands[flag];
                    if (cmd.Length > 0)
                    {
                        internalSend(cmd);
                    }
                }
            }
            Effects &= ~effect;
        }

        public virtual void ClearAllEffects()
        {
            foreach (var cmd in DisableCommands.Values)
            {
                if (cmd.Length > 0)
                {
                    internalSend(cmd);
                }
            }
            Effects = FontEffectsEnum.None;
        }

        public virtual void PrintASCIIString(string str)
        {
            internalSend(Encoding.ASCII.GetBytes(str));
        }

        public virtual void PrintDocument(IDocument doc)
        {
            //// Keep track of current settings so we can restore
            //var oldJustification = Justification;
            //var oldWidth = Width;
            //var oldHeight = Height;
            //var oldFont = Font;

            foreach (var sec in doc.Sections)
            {

                // First apply all effects. The firwmare decides if any there
                // are any conflicts and there is nothing we can do about that.
                // Apply the rest of the settings before we send string
                AddEffect(sec.Effects);
                SetAlignment(sec.Justification);
                SetFont(sec.Font);
                SetScalars(sec.WidthScalar, sec.HeightScalar);


                // Send the actual content
                internalSend(sec.GetContentBuffer(doc.CodePage));

                if (sec.AutoNewline)
                {
                    PrintNewline();
                }

                //// Remove effects for this section
                //RemoveEffect(sec.Effects);
            }

            //// Undo all the settings we just set
            //SetJustification(oldJustification);
            //SetScalars(oldWidth, oldHeight);
            //SetFont(oldFont);
            Reinitialize();
        }

        public abstract void SetImage(PrinterImage image, IDocument doc, int index);

        public abstract void SetImage(PrinterImage image, IDocument doc, int index, FontAlignment justification);

        public virtual void PrintNewline()
        {
            internalSend(NewLineCommand);
        }

        /// <summary>
        /// Pula linhas no impresso.
        /// </summary>
        /// <param name="lineCount">Opcional: quantiade de linhas | Default: 5</param>
        public virtual void FormFeed(int lineCount = 5)
        {
            FormFeedCommand[2] = (byte)(lineCount);
            internalSend(FormFeedCommand);
        }

        public virtual void SendRaw(byte[] raw)
        {
            internalSend(raw);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Fechar a porta serial
        /// </summary>
        /// <param name="diposing">True para fechar a conexão</param>
        protected virtual void Dispose(bool diposing)
        {
            if (Connection != null)
            {
                Connection.Dispose();
            }
        }

        #region Protected
        /// <summary>Envia payload para a porta serial. 
        /// Se a porta não estiver aberta, isso abrirá a porta antes de gravar. 
        /// A porta será fechada quando a gravação for concluída ou falhar.</summary>
        /// <param name="payload"></param>
        protected void internalSend(byte[] payload)
        {
            // Do not send empty packets
            if (payload.Length == 0)
            {
                return;
            }

            try
            {
                Connection.Open();

                Connection.Write(payload);
            }
            catch { /* Do nothing */ }
            finally
            {
                Connection.Close();
            }
        }

        public void Cut(CutModeEnum cutMode)
        {
            CutCommand[2] = (byte)cutMode;
            internalSend(CutCommand);
        }

        public void Reverse()
        {
            internalSend(ReverseCommand);
        }

        /// <summary>
        ///   <para>Imprime um texto utilizando as formatações abaixo:</para>
        ///   <para>{FT:N} - Tipo de fonte N=A, B ou C    <br />{EX} - Fonte expandida    <br />{CE} - Centralizado   <br /> {AD} - Alinhado a direita    <br />{AE} - Alinhado a esquerda<br />    {RE} - Reiniciar parametros  <br />{FE:N} - Feed N = quantidade de linhas   <br />{BP} - Beep (Não implementado)</para>
        ///   <para>{CO} - Cortar papel parcial</para>
        /// </summary>
        /// <param name="texto">Texo a ser impresso (Cada linha deve conter um comando Enviroment.NewLine)</param>
        public void PrintGenericFormat(string texto)
        {
            RemoveAccentuation(ref texto);

            ThermalFontsEnum fonteAtual = ThermalFontsEnum.A;

            foreach (var linha in texto.Split(Environment.NewLine.ToCharArray()))
            {
                switch (linha.Trim())
                {
                    case "{EX}": //Expandido
                        SetScalars(FontWidthScalarEnum.w2, FontHeighScalarEnum.h1);
                        break;
                    case "{CE}": //Centralizado
                        SetAlignment(FontAlignment.Center);
                        break;
                    case "{AE}": //Alinhamento esquerda
                        SetAlignment(FontAlignment.Left);
                        break;
                    case "{AD}": //alinhamento direita
                        SetAlignment(FontAlignment.Right);
                        break;
                    case "{RE}": //Reiniciar configuração
                        Reinitialize();
                        SetFont(fonteAtual);
                        break;
                    case "{RV}": //Reverso
                        Reverse();
                        break;
                    case "{BP}": //Beep
                        //Beep
                        break;
                    case "{CO}": //Cortar
                        Cut(CutModeEnum.Parcial);
                        break;
                    default:
                        if (linha.Contains("{FE:"))
                        {
                            int feed;

                            if (int.TryParse(linha.Split(':').ToString(), out feed))
                                FormFeed(feed);
                            else
                                FormFeed();

                            break;
                        }

                        if (linha.Contains("{FT:"))
                        {
                            if (linha.Split(':')[0].Equals("A"))
                            { SetFont(ThermalFontsEnum.A); fonteAtual = ThermalFontsEnum.A; }

                            if (linha.Split(':')[1][0].ToString().Equals("B"))
                            { SetFont(ThermalFontsEnum.B); fonteAtual = ThermalFontsEnum.B; }

                            if (linha.Split(':')[0].Equals("C"))
                            { SetFont(ThermalFontsEnum.C); fonteAtual = ThermalFontsEnum.C; }

                            break;
                        }

                        if (linha.Contains("{ES:")) //Escala
                        {
                            var regex = new Regex(@"[^\d]");

                            string n = regex.Replace(linha, "");

                            if (int.TryParse(n, out int esc))
                            {
                                switch (esc)
                                {
                                    case 0:
                                        SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h1);
                                        break;
                                    case 1:
                                        SetScalars(FontWidthScalarEnum.w2, FontHeighScalarEnum.h1);
                                        break;
                                    case 2:
                                        SetScalars(FontWidthScalarEnum.w3, FontHeighScalarEnum.h2);
                                        break;
                                    case 3:
                                        SetScalars(FontWidthScalarEnum.w4, FontHeighScalarEnum.h3);
                                        break;
                                    case 4:
                                        SetScalars(FontWidthScalarEnum.w5, FontHeighScalarEnum.h4);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        }

                        PrintASCIIString(linha + Environment.NewLine);

                        break;
                }
            }
        }

        private void RemoveAccentuation(ref string texto)
        {
            //Remove toda a acentuação
            byte[] tempBytes;
            tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(texto);
            texto = Encoding.UTF8.GetString(tempBytes);
        }

        #endregion
    }
}
