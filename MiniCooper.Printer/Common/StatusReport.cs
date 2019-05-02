using ESCPOS.Printer.Helpers;

namespace ESCPOS.Printer.Common
{
    /// <summary>
    /// Encapsula todos os campos de resposta do status ESC/POS
    /// </summary>
    public class StatusReport
    {
        /// <summary>
        /// Obter ou Define um sinalizador indicando que este é um relatório inválido. 
        /// Um relatório inválido será gerado se uma resposta incompleta, 
        /// malformada ou ausente for processada a partir de um comando GetStatus.
        /// </summary>
        public bool IsInvalidReport { get; set; }

        /// <summary>
        /// Impressora está relatando on-line se o valor for verdadeiro
        /// </summary>
        public IsOnlineVal IsOnline { get; set; }

        /// <summary>
        /// Há algum papel presente se esse valor for verdadeiro. Note, o nível do papel 
        /// pode ser baixo, mas ainda é considerado presente.
        /// </summary>
        public IsPaperPresentVal IsPaperPresent { get; set; }

        /// <summary>
        /// O nível do papel está no limite ou acima do limite mínimo de papel se o valor for verdadeiro
        /// </summary>
        //TODO: Implementar leitura quantidade papel
        public IsPaperLevelOkayVal IsPaperLevelOkay { get; set; }

        /// <summary>
        /// O papel está na posição atual se esse valor for verdadeiro
        /// </summary>
        public IsTicketPresentAtOutputVal IsTicketPresentAtOutput { get; set; }

        /// <summary>
        /// A tampa da impressora está fechada se o valor for verdadeiro
        /// </summary>
        //TODO: Implementar status da tampa da impressora
        public IsCoverClosedVal IsCoverClosed { get; set; }

        /// <summary>
        /// O tracionador (motor) de papel está atualmente desligado se esse valor for verdadeiro
        /// </summary>
        public IsPaperMotorOffVal IsPaperMotorOff { get; set; }

        /// <summary>
        /// O botão de diagnóstico NÃO está sendo pressionado se esse valor for verdadeiro
        /// </summary>
        //TODO: Implementar status do botão de painel
        public IsDiagButtonReleasedVal IsDiagButtonReleased { get; set; }

        /// <summary>
        /// A temperatura de impressão está ok se esse valor for verdadeiro
        /// </summary>
        public IsHeadTemperatureOkayVal IsHeadTemperatureOkay { get; set; }

        /// <summary>
        /// Comms estão ok, sem erros, se esse valor for verdadeiro
        /// </summary>
        public IsCommsOkayVal IsCommsOkay { get; set; }

        /// <summary>
        /// A tensão da fonte de alimentação está dentro da tolerância se esse valor for verdadeiro
        /// </summary>
        //TODO: Implementar leitura da tensão
        public IsPowerSupplyVoltageOkayVal IsPowerSupplyVoltageOkay { get; set; }

        /// <summary>
        /// O caminho do papel está desobstruído
        /// </summary>
        //TODO: Implementar papel obstruido
        public IsPaperPathClearVal IsPaperPathClear { get; set; }

        /// <summary>
        /// A guilhotina esta ok se esse valor for verdadeiro
        /// </summary>
        //Implementar status guilhotina
        public IsCutterOkayVal IsCutterOkay { get; set; }

        /// <summary>
        /// A última alimentação de papel NÃO foi devido ao botão de diagnóstico se o valor for verdadeiro
        /// </summary>
        //TODO: Alimentação de papel
        public IsNormalFeedVal IsNormalFeed { get; set; }

        /// <summary>
        /// Se a impressora estiver relatando algum tipo de erro, esse valor será verdadeiro
        /// </summary>
        public HasErrorVal HasError { get; set; }

        /// <summary>
        /// Existe um estado de erro não recuperável se esse valor for verdadeiro
        /// </summary>
        //TODO: Ajustar HasFatalError
        public HasFatalErrorVal HasFatalError { get; set; }

        /// <summary>
        /// Existe um estado de erro recuperável se esse valor for verdadeiro
        /// </summary>
        //TODO: Ajustar HasRecoverableError
        public HasRecoverableErrorVal HasRecoverableError { get; set; }

        /// <summary>Retorna esse objeto como uma string JSON e, opcionalmente, um formato de tabulação.</summary>
        /// <param name="prettyPrint">True para impressão formatada, padrão falso</param>
        /// <returns>JSON string</returns>
        public string ToJSON(bool prettyPrint = false)
        {
            return Json.Serialize(this, prettyPrint);
        }

        /// <summary>
        /// Retorna um relatório de status com o sinalizador IsValidReport definido como false
        /// </summary>
        /// <returns></returns>
        public static StatusReport Invalid()
        {
            return new StatusReport
            {
                IsInvalidReport = true,
            };
        }
    }
}
