using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ESCPOS.Printer.Helpers
{
    /// <summary>
    /// Conversor PInvoke para funções do driver winspool
    /// </summary>
    public class RawPrinterHelper
    {
        // no ctor
        private RawPrinterHelper() { }

        /// <summary>
        ///  Envie dados não gerenciados para a impressora de destino. 
        ///  Quando a função recebe um nome de impressora e uma matriz não gerenciada de bytes, 
        ///  a função envia esses bytes para a fila de impressão. Retorna verdadeiro em sucesso, falso em falha.
        /// </summary>
        /// <param name="szPrinterName">Nome da impressora</param>
        /// <param name="pBytes">Pointer to data</param>
        /// <param name="dwCount">Comprimento de dados em bytes</param>
        /// <returns>bool</returns>
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            NativeMethods.DOCINFOA di = new NativeMethods.DOCINFOA();

            bool bSuccess = false; // Suponha falha, a menos que você tenha sucesso especificamente.

            di.pDocName = "ESCPOSTTester";
            di.pDataType = "RAW";

            // Abra a impressora.
            if (NativeMethods.OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Iniciar um documento.
                if (NativeMethods.StartDocPrinter(hPrinter, 1, di))
                {

                    // Inicia uma página.
                    if (NativeMethods.StartPagePrinter(hPrinter))
                    {
                        bSuccess = NativeMethods.WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        NativeMethods.EndPagePrinter(hPrinter);
                    }
                    NativeMethods.EndDocPrinter(hPrinter);
                }
                NativeMethods.ClosePrinter(hPrinter);
            }

            // Se você não teve sucesso, GetLastError pode fornecer mais informações
            // sobre por que não.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        /// <summary>Enviar arquivo para impressora como um trabalho de impressão. 
        /// Se o caminho não existir, uma exceção será lançada</summary>
        /// <param name="szPrinterName">Nome da impressora</param>
        /// <param name="szFileName">Caminho para o arquivo a ser impresso</param>
        /// <returns>bool</returns>
        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Abra o arquivo
            // MS por qualquer motivo, faz o leitor apropriar-se deste fluxo
            // então NÃO use o padrão de uso aqui. Isso poderia levar a um ObjectDisposed
            // exception. Deixe o BinaryReader (br) limpar o fs para nós.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            using (BinaryReader br = new BinaryReader(fs))
            {
                // Declara uma matriz de bytes grande o suficiente para conter o conteúdo do arquivo.
                Byte[] bytes = new Byte[fs.Length];
                bool bSuccess = false;
                // Ponteiro não gerenciado.
                IntPtr pUnmanagedBytes = new IntPtr(0);
                int nLength;

                nLength = Convert.ToInt32(fs.Length);

                // Leia o conteúdo do arquivo no array.
                bytes = br.ReadBytes(nLength);

                // Aloca alguma memória não gerenciada para esses bytes.
                pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);

                // Copie a matriz de bytes gerenciados na matriz não gerenciada.
                Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);

                // Envia os bytes não gerenciados para a impressora.
                bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);

                // Libera a memória não gerenciada que você alocou anteriormente.
                Marshal.FreeCoTaskMem(pUnmanagedBytes);

                return bSuccess;
            }
        }

        /// <summary> Container em torno de SendBytesToPrinter</summary>
        /// <param name="szPrinterName">Nome da impressora</param>
        /// <param name="szString">Texto para enviar para a impressora</param>
        /// <returns></returns>
        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // Quantos caracteres estão na string
            dwCount = szString.Length;
            // Suponha que a impressora esteja esperando texto ANSI e, em seguida, converta
            // a string para o texto ANSI.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Envie a string ANSI convertida para a impressora.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }

        public static bool ReadFromPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            // Leia os dados da impressora.
            Int32 dwError = 0, dwBytesRead = 0;
            IntPtr hPrinter = new IntPtr(0);
            NativeMethods.DOCINFOA di = new NativeMethods.DOCINFOA();
            NativeMethods.PRINTER_DEFAULTS pd = new NativeMethods.PRINTER_DEFAULTS();
            pd.DesiredAccess = 0x00000020;

            bool bSuccess = false;

            // Abra a impressora.
            if (NativeMethods.OpenPrinter2(szPrinterName.Normalize(), out hPrinter, pd))
            {
                // Start a document.
                if (NativeMethods.StartDocPrinter(hPrinter, 1, di))
                {
                    // Iniciar um documento.
                    if (NativeMethods.StartPagePrinter(hPrinter))
                    {
                        // leia seus bytes.
                        bSuccess = NativeMethods.ReadPrinter(hPrinter, out pBytes, dwCount, out dwBytesRead);

                        // Se você não teve sucesso, GetLastError pode fornecer mais informações
                        // sobre por que não.
                        if (bSuccess == false)
                        {
                            dwError = Marshal.GetLastWin32Error();
                        }

                        NativeMethods.EndPagePrinter(hPrinter);
                    }
                    NativeMethods.EndDocPrinter(hPrinter);
                }
                NativeMethods.ClosePrinter(hPrinter);
            }

            return bSuccess;
        }
    }
}
