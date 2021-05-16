using System.Windows.Media.Imaging;

namespace CSharpToolkit.Models {
    public class ImageTextPair {

        BitmapImage _image;
        string _text;
        
        public ImageTextPair(BitmapImage image, string text) {
            _image = image;
            _text = text;
        }

        public BitmapImage Image => _image;
        public string Text => _text;

    }
}
