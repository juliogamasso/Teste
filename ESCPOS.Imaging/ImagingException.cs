namespace ESCPOS.Imaging
{
    [System.Serializable]
    public class ImagingException : System.Exception
    {
        public ImagingException(string message)
            : base(message)
        { }
    }
}
