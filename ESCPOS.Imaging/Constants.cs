namespace ESCPOS.Imaging
{
    public static class Constants
    {
        /// <summary>
        /// Filtro de tipo de arquivo para cada tipo de imagem com suporte
        /// </summary>
        public static readonly string ImageFileFilter = "All Images (*.emf;*.wmf;*.jpg;*.jpeg;*.jfif;*.jpe;*.png;*.bmp;*.dib;*" +
                            ".rle;*.gif;*.tif;*.tiff)|*.emf;*.wmf;*.jpg;*.jpeg;*.jfif;*.jpe;*.png;" +
                            "*.bmp;*.dib;*.rle;*.gif;*.tif;*.tiff|Windows Enhanced Metafile (*.emf)|" +
                            "*.emf|Windows Metafile (*.wmf)|*.wmf|JPEG File Interchange Format (*.jpg;" +
                            "*.jpeg;*.jfif;*.jpe)|*.jpg;*.jpeg;*.jfif;*.jpe|Portable Networks Graphic (*.png)|" +
                            "*.png|Windows Bitmap (*.bmp;*.dib;*.rle)|*.bmp;*.dib;*.rle|Graphics Interchange Format (*.gif)|" +
                            "*.gif|Tagged Image File Format (*.tif;*.tiff)|*.tif;*.tiff|All files (*.*)|*.*";
    }
}
