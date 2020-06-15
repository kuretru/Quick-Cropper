using CroppingImageLibrary.Services;
using CroppingImageLibrary.Services.Event;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CroppingImageLibrary
{
    /// <summary>
    /// Interaction logic for CropToolControl.xaml
    /// </summary>
    public partial class CropToolControl : UserControl
    {

        public event CroppedAreaChangedHandler CroppedAreaChangedEvent;

        public CropService CropService { get; private set; }

        public CropToolControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取图片实际渲染大小
        /// </summary>
        /// <returns>图片实际渲染大小</returns>
        public Size GetImageRenderSize()
        {
            return SourceImage.RenderSize;
        }

        private void OnCroppedAreaChanged(object sender)
        {
            CroppedAreaChangedEvent(sender);
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (CropService != null)
            {
                CropService.Reisze();
            }
        }

        private void RootGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CropService.Adorner.RaiseEvent(e);
        }

        private void RootGrid_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CropService.Adorner.RaiseEvent(e);
        }

        private void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            CropService = new CropService(this);
            CropService.CroppedAreaChangedEvent += OnCroppedAreaChanged;
        }

        public void SetImage(BitmapImage bitmapImage)
        {
            SourceImage.Source = bitmapImage;
            // RootGrid.Height = bitmapImage.Height;
            // RootGrid.Width = bitmapImage.Width;
        }
    }
}
