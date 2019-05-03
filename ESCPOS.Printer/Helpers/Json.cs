using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCPOS.Printer.Helpers
{

    public static class Json
    {
        // Queremos substituir globalmente os padrões JSONConvert, então precisamos que este seja um estado estático
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static Json()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>() {
                    new StringEnumConverter { CamelCaseText = false },
                },
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        /// <summary>
        /// Imprima uma string json usando recuo e espaçamento JSON padrão
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        public static string Serialize(object obj, bool prettyPrint = false)
        {
            if (prettyPrint)
                return PrettyPrint(JsonConvert.SerializeObject(obj));
            else
                return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Formata em JSON a cadeia de entrada fornecida
        /// </summary>
        /// <param name="inputText">JSON string to format</param>
        /// <returns>Tab formatted JSON</returns>
        private static string PrettyPrint(string inputText)
        {

            bool escaped = false;
            bool inquotes = false;
            int column = 0;
            int indentation = 0;
            Stack<int> indentations = new Stack<int>();
            int TABBING = 8;
            StringBuilder sb = new StringBuilder();
            foreach (char x in inputText)
            {
                sb.Append(x);
                column++;
                if (escaped)
                {
                    escaped = false;
                }
                else
                {
                    if (x == '\\')
                    {
                        escaped = true;
                    }
                    else if (x == '\"')
                    {
                        inquotes = !inquotes;
                    }
                    else if (!inquotes)
                    {
                        if (x == ',')
                        {
                            // se virmos uma vírgula, vamos para a próxima linha e recuamos para a mesma profundidade
                            sb.Append("\r\n");
                            column = 0;
                            for (int i = 0; i < indentation; i++)
                            {
                                sb.Append(" ");
                                column++;
                            }
                        }
                        else if (x == '[' || x == '{')
                        {
                            // se abrirmos um colchete ou chave, recue mais (empurre a pilha)
                            indentations.Push(indentation);
                            indentation = column;
                        }
                        else if (x == ']' || x == '}')
                        {
                            // se fecharmos um colchete ou chave, desfaça um nível de recuo (pop)
                            indentation = indentations.Pop();
                        }
                        else if (x == ':')
                        {
                            // se virmos dois pontos, adicionamos espaços até chegarmos ao próximo
                            // tab stop, mas sem usar caracteres de tabulação!
                            while ((column % TABBING) != 0)
                            {
                                sb.Append(' ');
                                column++;
                            }
                        }
                    }
                }
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Conversor para permitir que o JsonConvert use o método ToString de uma classe
    /// </summary>
    internal class ToStringJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) { return true; }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
