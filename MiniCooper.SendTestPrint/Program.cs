using ESCPOS.Printer.Barcodes;
using ESCPOS.Printer.Common.Enums;
using ESCPOS.Printer.Printers.GenericPrinter;
using ESCPOS.Printer.Templates;
using ESCPOS.PrinterC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ESCPOS.SendTestPrint
{
    class Program
    {
        static string PortaCom = "";
        static int testeAtual;
        static List<string> testCases;

        static void Main(string[] args)
        {
            Titulo();
            InicioTeste();
            TestCase();
        }

        private static void TestCase()
        {

            testCases = new List<string>()
            {
                "Texto",
                "Formatação texto",
                "Formatação genérica",
                "Guilhotina",
                "Escala de largura",
                "Escala de Altura",
                "Códigos de barras",
                "Teste Imagem",
                "Completo",
                "Status",
                "Teste Codigo Barras"
            };
            while (true)
            {
                int i = 0;

                Console.Clear();
                Titulo();

                Console.WriteLine("         Selecione um teste:");
                foreach (var item in testCases)
                {
                    i += 1;
                    Console.WriteLine($"            {i} : {item}");
                }
                Console.WriteLine();
                Console.WriteLine("         99 : Sair");

                Console.WriteLine();
                Console.Write("         Executar teste: ");

                try
                {
                    testeAtual = Convert.ToInt32(Console.ReadLine());
                    if (testeAtual != 99 && (testeAtual < 1 || testeAtual > testCases.Count))
                    {
                        throw new InvalidOperationException();
                    }
                }
                catch
                {
                    Console.WriteLine("Opção inválida, tente novamente.");
                    continue;
                }

                if (testeAtual == 99) return;

                Console.Clear();

                switch (testeAtual)
                {
                    case 1:
                        TesteTexto();
                        break;
                    case 2:
                        TesteFormatacaoTexto();
                        break;
                    case 3:
                        TesteTextoGenerico();
                        break;
                    case 4:
                        TesteGuilhotina();
                        break;
                    case 5:
                        TesteEscalaLargura();
                        break;
                    case 6:
                        TesteEscalaAltura();
                        break;
                    case 7:
                        TesteCodigoBarras();
                        break;
                    case 8:
                        TesteImagem();
                        break;
                    case 9:
                        TesteCompleto();
                        break;
                    case 10:
                        GetStatus();
                        break;
                    case 11:
                        TesteCodigoBarras();
                        break;
                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }
            }
        }


        private static string Alinhamento(string texto, int lenght, AlinhamentoEnum alinhamento)
        {
            switch (alinhamento)
            {
                case AlinhamentoEnum.Esquerda:
                    return $"       {string.Format("{0," + (lenght * -1) + "}", texto)}";

                case AlinhamentoEnum.Centro:

                    return $"       {string.Format("{0," + (lenght * -1) + "}", string.Format("{0," + ((lenght + texto.Length) / 2).ToString() + "}", texto))}";

                case AlinhamentoEnum.Direita:

                    return $"       {string.Format("{0," + (lenght) + "}", texto)}";
            }
            return texto;
        }

        private static string AddCaracter(string texto, string caracter)
        {
            var r = $"{caracter}{texto.Insert(texto.Length, caracter)}";
            return r;
        }

        private enum AlinhamentoEnum
        {
            Esquerda,
            Centro,
            Direita
        }

        private static void Titulo()
        {
            Console.WriteLine();
            Console.WriteLine(Alinhamento("".PadRight(50, '+'), 50, AlinhamentoEnum.Centro));
            Console.WriteLine(Alinhamento("Teste de impressão ESCPOS.Printer", 50, AlinhamentoEnum.Centro));

            if (!string.IsNullOrWhiteSpace(PortaCom))
                Console.WriteLine(Alinhamento($"Porta selecionada: {PortaCom}", 50, AlinhamentoEnum.Centro));

            Console.WriteLine(Alinhamento("".PadRight(50, '+'), 50, AlinhamentoEnum.Centro));
            Console.WriteLine();
        }

        private static void InicioTeste()
        {
            try
            {
                Console.WriteLine();
            getPort: Console.Write("Informe o numero da porta COM:");
                var retorno = Console.ReadLine();

                if (int.TryParse(retorno, out int _))
                    PortaCom = $"COM{retorno}".Trim();
                else
                {
                    Console.WriteLine("Entrada inválida, tente novamente!");
                    goto getPort;
                }

                Console.Clear();

                Titulo();
            }
            catch (Exception)
            {
                Console.WriteLine("Ocorreu um erro, tente novamente!");
            }
        }

        #region Testes

        private static void TesteTexto()
        {
            if (string.IsNullOrWhiteSpace(PortaCom))
                return;

            using (var printer = new GenericPrinter(PortaCom))
            {
                printer.PrintASCIIString("Texto e um conjunto de palavras e frases encadeadas que permitem interpretacao e transmitem uma mensagem. E qualquer obra escrita em versao original e que constitui um livro ou um documento escrito. Um texto e uma unidade linguistica de extensao superior a frase.");
                printer.SetFont(ThermalFontsEnum.B);
                printer.FormFeed();
                printer.Cut(CutModeEnum.Parcial);
            }
        }

        private static void TesteFormatacaoTexto()
        {
            if (string.IsNullOrWhiteSpace(PortaCom))
                return;

            using (var printer = new GenericPrinter(PortaCom))
            {
                List<string> pedido = new List<string>();

                pedido.Add("{FT:" + GetFonte().ToUpper() + "}");
                pedido.Add("{RE}");
                pedido.Add("NORMAL");
                pedido.Add("{EX}");
                pedido.Add("EXPANDIDO");
                pedido.Add("{AD}");
                pedido.Add("EXPANDIDO DIREITA");
                pedido.Add("{CE}");
                pedido.Add("EXPANDIDO CENTRO");
                pedido.Add("{AE}");
                pedido.Add("EXPANDIDO ESQUERDA");
                pedido.Add("{RE}");

                pedido.Add("{EX}");
                pedido.Add("{RV}");
                pedido.Add("SENHA 12324");

                pedido.Add("{CE}");
                pedido.Add("{ES:0}");
                pedido.Add("              SENHA 12324              ");

                pedido.Add("{ES:1}");
                pedido.Add("         SENHA 12324        ");

                pedido.Add("{ES:2}");
                pedido.Add("SENHA 12324");

                pedido.Add("{ES:3}");
                pedido.Add("SENHA 12324");

                pedido.Add("{ES:4}");
                pedido.Add("SENHA 12324");

                pedido.Add("{FE:3}");
                pedido.Add("{CO}");

                var c = string.Join("\n", pedido);

                printer.PrintGenericFormat(c);
            }
        }

        private static void TesteTextoGenerico()
        {
            if (string.IsNullOrWhiteSpace(PortaCom))
                return;

            using (var printer = new GenericPrinter(PortaCom))
            {
                printer.PrintSerialBaudRate = 19200;

                var nome = "TESTANDO NOME PRODUTO 123456 45844848 548481 51313";
                var quantidade = 9999;
                var preco = "150,00";
                var valorTotal = "400,00";

                List<string> pedido = new List<string>();



                pedido.Add("{FT:" + GetFonte().ToUpper() + "}");
                pedido.Add("{RE}");
                pedido.Add("{EX}");
                pedido.Add("{CE}");
                pedido.Add("LOJA TESTE");
                pedido.Add("SENHA: 8952314");
                pedido.Add("");
                pedido.Add("{RE}");
                pedido.Add($"5 itens");
                pedido.Add("".PadRight(40, '-'));

                pedido.Add($"{$"{quantidade} x ",-5}{nome,-32}");
                pedido.Add($"{preco,40}");
                pedido.Add($"{$"{quantidade} x ",-5}{nome,-32}");
                pedido.Add($"{preco,40}");
                pedido.Add($"{$"{quantidade} x ",-5}{nome,-32}");
                pedido.Add($"{preco,40}");
                pedido.Add($"{$"{quantidade} x ",-5}{nome,-32}");
                pedido.Add($"{preco,40}");

                pedido.Add("{EX}");
                pedido.Add("");
                pedido.Add("");
                pedido.Add($"{$"Total: {valorTotal}",20}");
                pedido.Add("{RE}");
                pedido.Add("".PadRight(40, '_'));
                pedido.Add("Volte sempre!");
                pedido.Add($"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToShortTimeString()}");
                pedido.Add("{FE:3}");
                pedido.Add("{CO}");

                //var p = pedido.Aggregate("",
                //                    (a, b) => a + "\n" + b);

                var c = string.Join("\n", pedido);

                printer.PrintGenericFormat(c);
            }
        }

        private static string GetFonte()
        {
            Titulo();

        fonte: Console.Write("Informe o tipo de fonte A, B ou C:");

            var fonte = Console.ReadLine().ToLower();

            if (!(fonte.Equals("a") || fonte.Equals("b") || fonte.Equals("c")))
            {
                Console.WriteLine("Entrada inválida, tente novamente!");
                goto fonte;
            }

            return fonte;
        }

        private static void TesteGuilhotina()
        {
            if (string.IsNullOrWhiteSpace(PortaCom))
                return;

            using (var printer = new GenericPrinter(PortaCom))
            {
                printer.Reinitialize();

                //Corte guilhotina parcial
                printer.PrintASCIIString($"== [ Start {testCases[testeAtual - 1]} ] ==");
                printer.FormFeed();
                printer.PrintASCIIString("  CORTE PARCIAL ");
                printer.FormFeed();
                printer.PrintASCIIString($"== [ End {testCases[testeAtual - 1]} ] ==");
                printer.FormFeed();
                printer.Cut(Printer.Common.Enums.CutModeEnum.Parcial);

                //Corte guilhotina total
                printer.PrintASCIIString($"== [ Start {testCases[testeAtual - 1]} ] ==");
                printer.FormFeed();
                printer.PrintASCIIString("  CORTE TOTAL ");
                printer.FormFeed();
                printer.PrintASCIIString($"== [ End {testCases[testeAtual - 1]} ] ==");
                printer.FormFeed();
                printer.Cut(Printer.Common.Enums.CutModeEnum.Total);
                printer.FormFeed();

                printer.Reinitialize();
            }
        }

        private static void TesteEscalaLargura()
        {
            if (string.IsNullOrWhiteSpace(PortaCom))
                return;

            using (var printer = new GenericPrinter(PortaCom))
            {
                printer.Reinitialize();

                //Font A
                printer.SetFont(ThermalFontsEnum.A);
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Largura 1");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w2, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Largura 2");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w3, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Largura 3");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w4, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Largura 4");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w5, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Largu. 5");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w6, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Lar. 6");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w7, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Lar. 7");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w8, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("La. 8");
                printer.PrintNewline();

                printer.FormFeed();
                printer.Cut(CutModeEnum.Parcial);

                printer.Reinitialize();

                //Font B
                printer.SetFont(ThermalFontsEnum.B);
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Largura 1");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w2, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Largura 2");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w3, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Largura 3");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w4, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Largura 4");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w5, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Largu. 5");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w6, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Lar. 6");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w7, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Lar. 7");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w8, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("La. 8");
                printer.PrintNewline();

                printer.FormFeed();
                printer.Cut(CutModeEnum.Parcial);

                printer.Reinitialize();
            }
        }

        private static void TesteEscalaAltura()
        {
            if (string.IsNullOrWhiteSpace(PortaCom))
                return;

            using (var printer = new GenericPrinter(PortaCom))
            {
                printer.Reinitialize();

                //Font A
                printer.SetFont(ThermalFontsEnum.A);
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Altura 1");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h2);
                printer.PrintASCIIString("Altura 2");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h3);
                printer.PrintASCIIString("Altura 3");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h4);
                printer.PrintASCIIString("Altura 4");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h5);
                printer.PrintASCIIString("Altura 5");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h6);
                printer.PrintASCIIString("Altura 6");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h7);
                printer.PrintASCIIString("Altura 7");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h8);
                printer.PrintASCIIString("Altura 8");
                printer.PrintNewline();

                printer.FormFeed();
                printer.Cut(CutModeEnum.Parcial);

                printer.Reinitialize();

                //Font B
                printer.SetFont(ThermalFontsEnum.B);
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h1);
                printer.PrintASCIIString("Altura 1");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h2);
                printer.PrintASCIIString("Altura 2");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h3);
                printer.PrintASCIIString("Altura 3");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h4);
                printer.PrintASCIIString("Altura 4");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h5);
                printer.PrintASCIIString("Altura 5");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h6);
                printer.PrintASCIIString("Altura 6");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h7);
                printer.PrintASCIIString("Altura 7");
                printer.PrintNewline();
                printer.SetScalars(FontWidthScalarEnum.w1, FontHeighScalarEnum.h8);
                printer.PrintASCIIString("Altura 8");
                printer.PrintNewline();

                printer.FormFeed();
                printer.Cut(CutModeEnum.Parcial);

                printer.Reinitialize();
            }
        }

        private static void TesteCodigoBarras()
        {
            using (var printer = new GenericPrinter(PortaCom))
            {
                //Code128
                Code128 code = new Code128()
                {
                    EncodeThis = "12321321312",
                    BarcodeWidthMultiplier = 2,
                    BarcodeDotHeight = 50, //Autura do codigo
                    HriPosition = Printer.Barcodes.Enums.HRIPositions.Below,
                    Mode = Code128.Modes.A,
                    BarcodeFont = ThermalFontsEnum.A
                };

                printer.PrintASCIIString("Code 128 - Altura 50");
                printer.FormFeed(1);
                printer.PrintBarCode(code);
                printer.FormFeed(1);
                printer.Reinitialize();


                //Code39
                Code39 code39 = new Code39()
                {
                    EncodeThis = "12321321312",
                    BarcodeWidthMultiplier = 2,
                    BarcodeDotHeight = 30, //Autura do codigo
                    BarcodeFont = ThermalFontsEnum.A
                };

                printer.PrintASCIIString("Code 39 - Altura 30");
                printer.FormFeed(1);
                printer.PrintBarCode(code);
                printer.FormFeed(1);
                printer.Reinitialize();

                //ITF
                ITF itf = new ITF()
                {
                    EncodeThis = "12321321312",
                    BarcodeDotHeight = 100, //Autura do codigo
                    BarcodeWidthMultiplier = 2,
                    BarcodeFont = ThermalFontsEnum.A
                };

                printer.PrintASCIIString("ITF - Altura 100");
                printer.FormFeed(1);
                printer.PrintBarCode(code);
                printer.FormFeed(3);
                printer.Reinitialize();

                //NFCe
                printer.PrintASCIIString("Exemplo QRCode NFCe\n");
                var qrCodeNFCe = new QrCode(4, "http://www.sefaz.mt.gov.br/nfce/consultanfce?chNFe=51131003460900000290650010000000031000000031&nVersao=100&tpAmb=2&cDest=02801244147&dhEmi=323031332D31302D32345431363A32313A30332D30333A3030&vNF=1,00&vICMS=0,00&digVal=78764D34764E2B48586A735657516F653474415A547855547764383D&cIdToken=000001&cHashQRCode=7AF4285DA2D18133BEF9F9370AD4A185B2527AFB");

                printer.PrintBarCode(qrCodeNFCe);
                printer.FormFeed(1);
                printer.Reinitialize();

                //Sat
                printer.PrintASCIIString("Exemplo QRCode SAT\n");
                var qrCodeSat = new QrCode(4, "http://www.sefaz.mt.gov.br/nfce/consultanfce?chNFe=51131003460900000290650010000000031000000031&nVersao=100&tpAmb=2&cDest=02801244147&dhEmi=323031332D31302D32345431363A32313A30332D30333A3030&vNF=1,00&vICMS=0,00&digVal=78764D34764E2B48586A735657516F653474415A547855547764383D&cIdToken=000001&cHashQRCode=7AF4285DA2D18133BEF9F9370AD4A185B2527AFB");

                printer.PrintBarCode(qrCodeNFCe);
                printer.FormFeed(1);

                printer.Cut(CutModeEnum.Parcial);
                printer.Reinitialize();
            }
        }

        private static void TesteCompleto()
        {
            var separador = new StandardSection()
            {
                Justification = FontAlignment.Center,
                HeightScalar = FontHeighScalarEnum.h1,
                WidthScalar = FontWidthScalarEnum.w1,
                Font = ThermalFontsEnum.A,
                AutoNewline = true,
            };

            var separadorUmaLinha = new StandardSection()
            {
                Justification = FontAlignment.Center,
                HeightScalar = FontHeighScalarEnum.h1,
                WidthScalar = FontWidthScalarEnum.w1,
                Font = ThermalFontsEnum.A,
                AutoNewline = true,
            };

            var textoHeader = new StandardSection()
            {
                Justification = FontAlignment.Center,
                HeightScalar = FontHeighScalarEnum.h1,
                WidthScalar = FontWidthScalarEnum.w2,
                Font = ThermalFontsEnum.A,
                AutoNewline = true,
            };

            var textoCabecalho = new StandardSection()
            {
                Justification = FontAlignment.Center,
                HeightScalar = FontHeighScalarEnum.h1,
                WidthScalar = FontWidthScalarEnum.w1,
                Font = ThermalFontsEnum.A,
                AutoNewline = true,
            };

            var textoItens = new StandardSection()
            {
                Justification = FontAlignment.Left,
                HeightScalar = FontHeighScalarEnum.h1,
                WidthScalar = FontWidthScalarEnum.w2,
                Font = ThermalFontsEnum.A,
                AutoNewline = true,
            };

            var document = new StandardDocument()
            {
                //Não esquecer de setar essa prop
                CodePage = CodePagesEnum.CPSPACE,
            };

            document.Sections.Add(new Placeholder());
            document.Sections.Add(separador);
            document.Sections.Add(textoHeader);
            document.Sections.Add(separadorUmaLinha);
            document.Sections.Add(textoCabecalho);
            document.Sections.Add(separador);
            document.Sections.Add(textoItens);
            document.Sections.Add(separador);

            using (var printer = new GenericPrinter(PortaCom))
            {
                printer.Reinitialize();

                //reader
                var reader = new StringBuilder();
                reader.Append("COZINHA" + Environment.NewLine);
                reader.Append("MESA: 35");

                //Cabeçalho
                var cabecalho = new StringBuilder();
                cabecalho.Append($"Data/Hora: {DateTime.Now}" + Environment.NewLine);
                cabecalho.Append("Atendente: Caixaa" + Environment.NewLine);
                cabecalho.Append("Pdv: 099-COMANDA TESTE");

                //Itens
                var itens = new StringBuilder();
                itens.Append("QTDE  ITEM" + Environment.NewLine);
                itens.Append("1/2  ALMONDEGA CREAMS" + Environment.NewLine);
                itens.Append("     * DIGITADA" + Environment.NewLine);
                itens.Append("     * TESTE SQL");

                separador.Content = "".PadRight(40, '=');
                textoHeader.Content = reader.ToString();
                separadorUmaLinha.Content = "".PadRight(40, '-');
                textoCabecalho.Content = cabecalho.ToString();
                textoItens.Content = itens.ToString();

                printer.PrintDocument(document);
                printer.FormFeed(5);
                printer.Cut(CutModeEnum.Parcial);
            }
        }

        private static void TesteImagem()
        {
            var printStatus = new StandardSection()
            {
                Justification = FontAlignment.Left,
                HeightScalar = FontHeighScalarEnum.h1,
                WidthScalar = FontWidthScalarEnum.w1,
                Font = ThermalFontsEnum.B,
                AutoNewline = true,
            };

            var document = new StandardDocument()
            {
                //Não esquecer de setar essa prop
                CodePage = CodePagesEnum.CPSPACE,
            };

            document.Sections.Add(new Placeholder());

            using (var printer = new GenericPrinter(PortaCom))
            {
                var image = new PrinterImage((Bitmap)Image.FromFile(@"XING_B24.BMP"));
                image.ApplyDithering(AlgorithmsEnum.JarvisJudiceNinke, 128);

                for (int i = 3; i >= 1; i--)
                {
                    printer.SetImage(image, document, 0, (FontAlignment)i);
                    printer.PrintDocument(document);
                }

                printer.FormFeed(5);
                printer.Cut(CutModeEnum.Parcial);
            }
        }

        private static void GetStatus()
        {
            Titulo();

            using (var printer = new GenericPrinter(PortaCom))
            {
                var status = printer.GetStatus(StatusTypesEnum.FullStatus).ToJSON(true);

                Console.WriteLine(status);
                Console.ReadKey();
                TestCase();
            }
        }

        #endregion

    }
}
