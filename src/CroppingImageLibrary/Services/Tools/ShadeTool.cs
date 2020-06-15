using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CroppingImageLibrary.Services.Tools
{
    internal class ShadeTool
    {
        private readonly CropTool _cropTool;
        private readonly RectangleGeometry _rectangleGeo;

        private readonly Canvas _canvas;
        private readonly GeometryGroup _geometryGroup;

        public Path ShadeOverlay { get; set; }

        public ShadeTool(Canvas canvas, CropTool cropTool)
        {
            _canvas = canvas;
            _cropTool = cropTool;

            ShadeOverlay = new Path
            {
                Fill = Brushes.Black,
                Opacity = 0.5
            };

            _geometryGroup = new GeometryGroup();
            RectangleGeometry geometry1 = new RectangleGeometry(new Rect(new Size(_canvas.Width, _canvas.Height)));
            _rectangleGeo = new RectangleGeometry(
                new Rect(
                    _cropTool.TopLeftX,
                    _cropTool.TopLeftY,
                    _cropTool.Width,
                    _cropTool.Height
                )
            );
            _geometryGroup.Children.Add(geometry1);
            _geometryGroup.Children.Add(_rectangleGeo);
            ShadeOverlay.Data = _geometryGroup;
        }

        public void Redraw()
        {
            _rectangleGeo.Rect = new Rect(
                _cropTool.TopLeftX,
                _cropTool.TopLeftY,
                _cropTool.Width,
                _cropTool.Height
            );
        }

        public void Resize()
        {
            _geometryGroup.Children[0] = new RectangleGeometry(new Rect(new Size(_canvas.Width, _canvas.Height)));
        }
    }
}
