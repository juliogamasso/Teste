namespace ESCPOS.Imaging
{
    public static class DitherFactory
    {
        // Nós não usamos arrays irregulares porque a sintaxe de multi-dimensional
        // arrays é mais fácil de trabalhar
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body")]
        public static IDitherable GetDitherer(AlgorithmsEnum algorithm, byte threshold = 128)
        {
            switch (algorithm)
            {
                case AlgorithmsEnum.Atkinson:
                    // 0,0,1,1,1,1,1,0,0,1,0,0
                    return new Dither(new byte[,] {
                       {0,0,1,1},
                       {1,1,1,0},
                       {0,1,0,0}
                   }, 3, threshold, true);

                case AlgorithmsEnum.Burkes:
                    // 0,0,0,8,4,2,4,8,4,2
                    return new Dither(new byte[,] {
                       {0,0,0,8,4},
                       {2,4,8,4,2},
                   }, 5, threshold, true);

                case AlgorithmsEnum.FloydSteinberg:
                    // 0,0,7,3,5,1
                    return new Dither(new byte[,] {
                       {0,0,7},
                       {3,5,1},
                   }, 4, threshold, true);

                case AlgorithmsEnum.FloydSteinbergFalse:
                    // 0,3,3,2
                    return new Dither(new byte[,] {
                       {0,3},
                       {3,2},
                   }, 3, threshold, true);

                case AlgorithmsEnum.JarvisJudiceNinke:
                    // 0,0,0,7,5,3,5,7,5,3,1,3,5,3,1
                    return new Dither(new byte[,] {
                       {0,0,0,7,5},
                       {3,5,7,5,3},
                       {1,3,5,3,1}
                   }, 48, threshold);

                case AlgorithmsEnum.Sierra:
                    // 0,0,0,5,3,2,4,5,4,2,0,2,3,2,0
                    return new Dither(new byte[,] {
                       {0,0,0,5,3},
                       {2,4,5,4,2},
                       {0,2,3,2,0},
                   }, 5, threshold, true);

                case AlgorithmsEnum.Sierra2:
                    // 0,0,0,4,3,1,2,3,2,1
                    return new Dither(new byte[,] {
                       {0,0,0,4,3},
                       {1,2,3,2,1},
                   }, 4, threshold, true);

                case AlgorithmsEnum.SierraLite:
                    // 0,0,2,1,1,0
                    return new Dither(new byte[,] {
                       {0,0,2},
                       {1,1,0},
                   }, 2, threshold, true);

                case AlgorithmsEnum.Stucki:
                    // 0,0,0,8,4,2,4,8,4,2,1,2,4,2,1
                    return new Dither(new byte[,] {
                       {0,0,0,8,4},
                       {2,4,8,4,2},
                       {1,2,4,2,1},
                   }, 42, threshold);

                default:
                    // Precisamos pelo menos fazer um bitmap de 1bpp, caso contrário o impresso terá lixo.
                    return new OneBPP(threshold);
            }
        }
    }
}
