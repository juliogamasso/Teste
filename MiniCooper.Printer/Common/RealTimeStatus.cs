using ESCPOS.Printer.Helpers;
using Newtonsoft.Json;

namespace ESCPOS.Printer.Common
{
    #region BaseStatus for JSONify
    [JsonConverter(typeof(ToStringJsonConverter))]
    public class BaseVal
    {
        protected bool m_value;

        public override string ToString()
        {
            return m_value.ToString();
        }
    }
    #endregion

    /// <summary>
    /// Impressora está relatando on-line se o valor for verdadeiro
    /// </summary>
    public class IsOnlineVal : BaseVal
    {
        public static implicit operator bool(IsOnlineVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator IsOnlineVal(bool val)
        {
            return new IsOnlineVal { m_value = val };
        }
    }

    /// <summary>
    /// A tampa está fechada se o valor for verdadeiro
    /// </summary>  
    public class IsCoverClosedVal : BaseVal
    {
        public static implicit operator bool(IsCoverClosedVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator IsCoverClosedVal(bool val)
        {
            return new IsCoverClosedVal { m_value = val };
        }
    }

    /// <summary>
    /// A última alimentação de papel NÃO foi devido ao botão de diagnóstico se o valor for verdadeiro
    /// </summary>    
    public class IsNormalFeedVal : BaseVal
    {
        public static implicit operator bool(IsNormalFeedVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator IsNormalFeedVal(bool val)
        {
            return new IsNormalFeedVal { m_value = val };
        }
    }

    /// <summary>
    /// O nível do papel está em ou acima do limite mínimo de papel se o valor for verdadeiro
    /// </summary>
    public class IsPaperLevelOkayVal : BaseVal
    {
        public static implicit operator bool(IsPaperLevelOkayVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator IsPaperLevelOkayVal(bool val)
        {
            return new IsPaperLevelOkayVal { m_value = val };
        }
    }

    /// <summary>
    /// Há algum papel presente se esse valor for verdadeiro. Note, o nível do papel
    /// pode ser baixo, mas ainda é considerado presente.
    /// </summary>
    public class IsPaperPresentVal : BaseVal
    {
        public static implicit operator bool(IsPaperPresentVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator IsPaperPresentVal(bool val)
        {
            return new IsPaperPresentVal { m_value = val };
        }
    }

    /// <summary>
    /// Se a impressora estiver relatando algum tipo de erro, esse valor será verdadeiro
    /// </summary>     
    public class HasErrorVal : BaseVal
    {
        public static implicit operator bool(HasErrorVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator HasErrorVal(bool val)
        {
            return new HasErrorVal { m_value = val };
        }
    }

    /// <summary>
    /// O cortador está ok se esse valor for verdadeiro
    /// </summary>
    public class IsCutterOkayVal : BaseVal
    {
        public static implicit operator bool(IsCutterOkayVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator IsCutterOkayVal(bool val)
        {
            return new IsCutterOkayVal { m_value = val };
        }
    }

    /// <summary>
    /// Existe um estado de erro não recuperável se esse valor for verdadeiro
    /// </summary>
    public class HasFatalErrorVal : BaseVal
    {
        public static implicit operator bool(HasFatalErrorVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator HasFatalErrorVal(bool val)
        {
            return new HasFatalErrorVal { m_value = val };
        }
    }

    /// <summary>
    /// Existe um estado de erro recuperável se esse valor for verdadeiro
    /// </summary>
    public class HasRecoverableErrorVal : BaseVal
    {
        public static implicit operator bool(HasRecoverableErrorVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator HasRecoverableErrorVal(bool val)
        {
            return new HasRecoverableErrorVal { m_value = val };
        }
    }

    /// <summary>
    /// O motor de papel está atualmente desligado se esse valor for verdadeiro
    /// </summary>
    public class IsPaperMotorOffVal : BaseVal
    {
        public static implicit operator bool(IsPaperMotorOffVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator IsPaperMotorOffVal(bool val)
        {
            return new IsPaperMotorOffVal { m_value = val };
        }
    }

    /// <summary>
    /// O papel está na posição atual se esse valor for verdadeiro
    /// </summary>
    public class IsTicketPresentAtOutputVal : BaseVal
    {
        public static implicit operator bool(IsTicketPresentAtOutputVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator IsTicketPresentAtOutputVal(bool val)
        {
            return new IsTicketPresentAtOutputVal { m_value = val };
        }
    }

    /// <summary>
    /// O botão de diagnóstico NÃO está sendo pressionado se esse valor for verdadeiro
    /// </summary>
    public class IsDiagButtonReleasedVal : BaseVal
    {
        public static implicit operator bool(IsDiagButtonReleasedVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator IsDiagButtonReleasedVal(bool val)
        {
            return new IsDiagButtonReleasedVal { m_value = val };
        }
    }

    /// <summary>
    /// A temperatura de impressão esta ok se esse valor for verdadeiro
    /// </summary>
    public class IsHeadTemperatureOkayVal : BaseVal
    {
        public static implicit operator bool(IsHeadTemperatureOkayVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator IsHeadTemperatureOkayVal(bool val)
        {
            return new IsHeadTemperatureOkayVal { m_value = val };
        }
    }

    /// <summary>
    /// Comms estão ok, sem erros, se esse valor for verdadeiro
    /// </summary>
    public class IsCommsOkayVal : BaseVal
    {
        public static implicit operator bool(IsCommsOkayVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator IsCommsOkayVal(bool val)
        {
            return new IsCommsOkayVal { m_value = val };
        }
    }

    /// <summary>
    /// A tensão da fonte de alimentação está dentro da tolerância se esse valor for verdadeiro
    /// </summary>
    public class IsPowerSupplyVoltageOkayVal : BaseVal
    {
        public static implicit operator bool(IsPowerSupplyVoltageOkayVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator IsPowerSupplyVoltageOkayVal(bool val)
        {
            return new IsPowerSupplyVoltageOkayVal { m_value = val };
        }
    }

    /// <summary>
    /// O caminho do papel está desobstruído
    /// </summary>
    public class IsPaperPathClearVal : BaseVal
    {
        public static implicit operator bool(IsPaperPathClearVal o)
        {
            return ((o == null) ? false : o.m_value);
        }

        public static implicit operator IsPaperPathClearVal(bool val)
        {
            return new IsPaperPathClearVal { m_value = val };
        }
    }
}
