namespace ESCPOS.Imaging
{
    internal struct Pixel
    {
        public byte B;  //< Blue
        public byte G;  //< Green
        public byte R;  //< Red
        public byte A;  //< Alpha

        /// <summary>
        /// Construtor não verificado espera um buffer de entrada de 4 bytes
        /// Order: ARGB
        /// </summary>
        /// <param name="slice4"></param>
        internal Pixel(byte[] slice4)
            : this(slice4[3], slice4[2], slice4[1], slice4[0])
        { }

        /// <summary>
        /// Construtor requisitado: ARGB
        /// </summary>
        /// <param name="a">Alpha</param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>       
        public Pixel(int a, int r, int g, int b)
            : this((byte)a, (byte)r, (byte)g, (byte)b)
        { }

        /// <summary>
        /// Construtor requisitado: ARGB
        /// </summary>
        /// <param name="a">Alpha</param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>   
        public Pixel(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// O branco é definido como 255,255,255 na paleta RGB.Retorna
        /// true se algum valor RGB não for 255.
        /// </summary>
        /// <returns>True if this pixel is non-white</returns>
        internal bool IsNotWhite()
        {
            var m = (A + R + G) / 3;
            return m != 255;
        }
    }
}
