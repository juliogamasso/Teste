using ESCPOS.Printer.Common.Enums;

namespace ESCPOS.Printer.Interfaces
{
    /// <summary>
    /// Interface para todos os tipos de conexão serial
    /// </summary>
    public interface ISerialConnection : System.IDisposable
    {
        /// <summary>
        /// Tenta abrir o dispositivo serial para comunicação.Retorna
        /// Sucesso somente se o dispositivo for encontrado e aberto com sucesso.
        /// </summary>
        /// <returns></returns>
        ReturnCodeEnum Open();

        /// <summary>
        /// Tenta fechar e liberar a conexão com o dispositivo serial.
        /// Se não houver problemas, sucesso será retornado.
        /// </summary>
        /// <returns></returns>
        ReturnCodeEnum Close();

        /// <summary>
        /// Obtém ou define o nome que identifica exclusivamente esse       
        /// dispositivo serial para o sistema
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Obtém ou define o tempo limite de leitura em milissegundos
        /// </summary>
        int ReadTimeoutMS { get; set; }

        /// <summary>
        /// Obtém ou define o tempo limite de gravação em milissegundos
        /// </summary>
        int WriteTimeoutMS { get; set; }

        /// <summary>
        /// Grava o payload na porta de destino e retorna a contagem de bytes que foram gravados. 
        /// A operação de gravação retornará todos os bytes ou escrita ou se o período definido por WriteTimeoutMS expirar.
        /// </summary>
        /// <param name="payload">Dados a serem enviados</param>
        /// <returns>Quantidade de bytes gravados</returns>
        int Write(byte[] payload);

        /// <summary>Leia e retorne n contagem de bytes. 
        /// Esta função retornará quando n bytes forem recebidos ou ReadTimeoutMS tiver expirado.</summary>
        /// <param name="n">Quantidade de bytes para ler</param>
        /// <returns>bytes lidos</returns>
        byte[] Read(int n);
    }
}
