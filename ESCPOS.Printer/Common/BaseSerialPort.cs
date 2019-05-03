using ESCPOS.Printer.Common.Enums;
using ESCPOS.Printer.Helpers;
using ESCPOS.Printer.Interfaces;
using System;
using System.IO.Ports;
using System.Threading;

namespace ESCPOS.Printer.Common
{

    abstract class BaseSerialPort : ISerialConnection
    {
        #region Fields
        protected readonly SerialPort _mPort;
        protected int _mReadTimeout;
        protected int _mWriteTimeout;
        /// <summary>Definido para um tamanho que funciona para a implementação da impressora. 
        /// Isso evita o envio excessivo de dados de uma só vez e a saturação do buffer de destino.</summary>
        //TODO: fazer essa propriedade configuravel
        protected int _mChunkSize;
        #endregion

        #region Constructor
        protected BaseSerialPort(string portName,
            int baud,
            int databits,
            Parity parity,
            StopBits stopbits,
            Handshake handshake)
        {
            Name = portName;

            _mPort = new SerialPort(portName, baud, parity, databits, stopbits);
            _mPort.Handshake = handshake;
            _mPort.WriteTimeout = _mWriteTimeout = 500;
            _mPort.ReadTimeout = _mReadTimeout = 500;
            _mPort.Handshake = Handshake.None;
            _mPort.WriteBufferSize = 4 * 1024;
            _mPort.ReadBufferSize = 4 * 1024;
            _mPort.Encoding = System.Text.Encoding.GetEncoding("Windows-1252");
            _mPort.DtrEnable = true;
            _mPort.RtsEnable = true;
            _mPort.DiscardNull = false;


            // As portas de comunicação virtuais usam um ponto de extremidade de transferência em massa que é
            // tipicamente 64 bytes de tamanho. Alta velocidade pode usar 512 bytes, mas é dificil de ser encontrado porta que suporte
            if (SerialPortUtils.IsVirtualComPort(portName))
            {
                _mChunkSize = 64;
            }
            else
            {
                // 256 é um valor razoável. RS-232 pode lidar com um pouco mais, mas não é recomendado.
                // o dispositivo de destino pode ter um buffer pequeno, manter valor baixo
                _mChunkSize = 256;
            }
        }

        ~BaseSerialPort()
        {
            Dispose(false);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Obtém o nome da porta serial para esta conexão
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Obtém ou define o tempo limite de leitura em milissegundos
        /// </summary>
        public int ReadTimeoutMS
        {
            get { return _mReadTimeout; }
            set
            {
                _mReadTimeout = value;
                _mPort.ReadTimeout = value;
            }
        }

        /// <summary>
        /// Obtém ou define o tempo limite de gravação em milissegundos
        /// </summary>
        public int WriteTimeoutMS
        {
            get { return _mWriteTimeout; }
            set
            {
                _mWriteTimeout = value;
                _mPort.WriteTimeout = value;
            }
        }

        #endregion

        public ReturnCodeEnum Open()
        {
            if (_mPort.IsOpen) return ReturnCodeEnum.Success;

            try
            {
                _mPort.Open();
                return ReturnCodeEnum.Success;
            }
            catch (System.IO.IOException)
            {
                return ReturnCodeEnum.ConnectionNotFound;
            }
            catch (System.AccessViolationException)
            {
                return ReturnCodeEnum.ConnectionAlreadyOpen;
            }
        }

        public ReturnCodeEnum Close()
        {
            if (!_mPort.IsOpen) return ReturnCodeEnum.Success;

            try
            {
                _mPort.Close();
                return ReturnCodeEnum.Success;
            }
            catch
            {
                return ReturnCodeEnum.ExecutionFailure;
            }
        }


        public int Write(byte[] payload)
        {
            return WritePort(payload);
        }

        public byte[] Read(int n)
        {
            return ReadPort(n);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_mPort != null)
                {
                    Close();
                    _mPort.Dispose();
                }
            }
        }

        #region Protected
        /// <summary>
        /// Grava dados na porta serial da impressora
        /// </summary>
        /// <param name="data">Dados para enviar</param>
        /// <returns>Número de bytes escritos</returns>
        protected int WritePort(byte[] data)
        {
            try
            {
                // Divida em partes para evitar o overrunning do buffer de destino
                foreach (var s in data.Split(_mChunkSize))
                {
                    _mPort.Write(s, 0, s.Length);
                    Thread.Sleep(10);
                }

                return data.Length;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Lê os bytes de contagem da porta serial. 
        /// Se os bytes de contagem estiverem indisponíveis, esta função bloqueará até a leitura expirar. 
        /// Se houver uma exceção ou não todos os dados esperados forem recebidos, um buffer vazio será retornado
        /// </summary>
        /// <param name="count">Número de bytes para ler da porta.</param>
        /// <returns>Bytes lidos da porta.</returns>
        protected byte[] ReadPort(int count)
        {

            byte[] buff = new byte[count];

            try
            {

                // Tentativa de ler os bytes
                var read = _mPort.Read(buff, 0, count);

                // Se não conseguirmos o suficiente, restaure o buffer para o tamanho 0
                if (read != count)
                {
                    buff = new byte[0];
                }

            }
            catch
            {
                buff = new byte[0];
            }

            return buff;
        }
        #endregion
    }
}
