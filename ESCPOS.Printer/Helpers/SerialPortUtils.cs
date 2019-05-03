using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ESCPOS.Printer.Helpers
{
    internal static class SerialPortUtils
    {
        private const string REG_COM_STRING = @"HARDWARE\DEVICEMAP\SERIALCOMM";

        /// <summary>
        /// Realiza uma tentativa de melhor esforço se a porta especificada for uma VCP. Isso executa uma série de verificações de registro para determinar se esse número de porta está atualmente associado a um driver VCP conhecido.
        /// </summary>
        /// <param name="portName">Nome da porta para verificar</param>
        /// <returns>Verdadeiro se a porta foi associada pela última vez à VCP</returns>
        internal static bool IsVirtualComPort(string portName)
        {
            return GetPortByVIDPID("", "", true).Contains(portName);
        }

        /// <summary>Compilar uma matriz de nomes de porta COM associados a um determinado VID e PID</summary>
        /// <param name="VID">string representando o id do fornecedor do conversor USB / Serial</param>
        /// <param name="PID">string representando o id do produto do conversor USB / Serial</param>
        /// <param name="any">Defina como true para ignorar o VID / PID e retornar todas as portas conhecidas</param>
        /// <returns></returns>
        internal static IEnumerable<string> GetPortByVIDPID(string VID, string PID, bool any = false)
        {
            var pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var comports = new List<string>();

            using (var rk1 = Registry.LocalMachine)
            using (var rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\USB"))
            {
                // Abra o ID do dispositivo de nível superior
                foreach (var s3 in rk2.GetSubKeyNames())
                {
                    // Abra uma instância única do dispositivo.
                    var rk3 = rk2.OpenSubKey(s3);
                    foreach (var s in rk3.GetSubKeyNames())
                    {
                        // Isso corresponde ao VID / PID especificado ou o usuário disse que eu quero todos eles?
                        if (regex.Match(s).Success || any)
                        {
                            // Somente olhe para os destinos com o valor da classe definido como Ports
                            var rk4 = rk3.OpenSubKey(s);
                            var classVal = Array.Find(rk4.GetValueNames(), (x) => x.Equals("Class"));
                            if (string.IsNullOrEmpty(classVal))
                            {
                                continue;
                            }

                            if (!rk4.GetValue(classVal, string.Empty).Equals("Ports"))
                            {
                                continue;
                            }

                            // Certifique-se de que tenha um valor adequado de parâmetros do dispositivo
                            var devParams = Array.Find(rk4.GetSubKeyNames(), (x) => x.Equals("Device Parameters"));
                            if (string.IsNullOrEmpty(devParams))
                            {
                                continue;
                            }

                            // Extrai esse nome de porta
                            var rk5 = rk4.OpenSubKey(devParams);
                            comports.Add((string)rk5.GetValue("PortName", string.Empty));

                            rk5.Close();
                            rk4.Close();
                        }
                    }

                    rk3.Close();
                }

                // Retorna nomes de portas exclusivos e não vazios
                return comports.Distinct().Where(x => !string.IsNullOrEmpty(x));
            }
        }
    }
}
