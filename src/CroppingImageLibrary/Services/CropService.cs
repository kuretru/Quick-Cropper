using CroppingImageLibrary.Services.Event;
using CroppingImageLibrary.Services.State;
using CroppingImageLibrary.Services.Tools;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace CroppingImageLibrary.Services
{
    public class CropArea
    {
        public readonly Size OriginalSize;
        public readonly Rect CroppedRectAbsolute;

        public CropArea(Size originalSize, Rect croppedRectAbsolute)
        {
            OriginalSize = originalSize;
            CroppedRectAbsolute = croppedRectAbsolute;
        }
    }

    public class CropService
    {
        private readonly CropAdorner _cropAdorner;
        private readonly Canvas _canvas;
        private readonly Tools.CropTool _cropTool;
        private readonly FrameworkElement _adornedElement;

        private IToolState _currentToolState;
        private readonly IToolState _createState;
        private readonly IToolState _dragState;
        private readonly IToolState _completeState;

        public event CroppedAreaChangedHandler CroppedAreaChangedEvent;

        public Adorner Adorner => _cropAdorner;

        private enum TouchPoint
        {
            OutsideRectangle,
            InsideRectangle
        }

        public CropService(FrameworkElement adornedElement)
        {
            _adornedElement = adornedElement;
            _canvas = new Canvas
            {
                Height = adornedElement.ActualHeight,
                Width = adornedElement.ActualWidth
            };
            _cropAdorner = new CropAdorner(adornedElement, _canvas);
            var adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            Debug.Assert(adornerLayer != null, nameof(adornerLayer) + " != null");
            adornerLayer.Add(_cropAdorner);

            _cropTool = new CropTool(_canvas);
            _cropTool.CroppedAreaChangedEvent += OnCroppedAreaChanged;
            _createState = new CreateState(_cropTool, _canvas);
            _completeState = new CompleteState();
            _dragState = new DragState(_cropTool, _canvas);
            _currentToolState = _completeState;

            _cropAdorner.MouseLeftButtonDown += AdornerOnMouseLeftButtonDown;
            _cropAdorner.MouseMove += AdornerOnMouseMove;
            _cropAdorner.MouseLeftButtonUp += AdornerOnMouseLeftButtonUp;

            _cropTool.Redraw(0, 0, 0, 0);
        }

        public CropArea GetCroppedArea() =>
            new CropArea(
                _cropAdorner.RenderSize,
                new Rect(_cropTool.TopLeftX, _cropTool.TopLeftY, _cropTool.Width, _cropTool.Height)
            );

        /// <summary>
        /// 改变窗体大小时，重新设置画板大小
        /// </summary>
        public void Reisze()
        {
            _canvas.Width = _adornedElement.ActualWidth;
            _canvas.Height = _adornedElement.ActualHeight;
            _cropTool.Resize();
        }

        /// <summary>
        /// 重新设置裁切框位置与大小
        /// </summary>
        /// <param name="left">裁切框左上角位置横坐标</param>
        /// <param name="top">裁切框左上角位置纵坐标</param>
        /// <param name="width">裁切框宽度</param>
        /// <param name="height">裁切框高度</param>
        public void SetCropArea(double left, double top, double width, double height)
        {
            _cropTool.Redraw(left, top, width, height);
        }

        private void OnCroppedAreaChanged(object sender)
        {
            CroppedAreaChangedEvent?.Invoke(sender);
        }

        private void AdornerOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _canvas.ReleaseMouseCapture();
            _currentToolState = _completeState;
        }

        private void AdornerOnMouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(_canvas);
            var newPosition = _currentToolState.OnMouseMove(point);
            if (newPosition.HasValue)
            {
                _cropTool.Redraw(newPosition.Value.Left, newPosition.Value.Top, newPosition.Value.Width, newPosition.Value.Height);
            }

        }

        private void AdornerOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _canvas.CaptureMouse();
            var point = e.GetPosition(_canvas);
            var touch = GetTouchPoint(point);
            if (touch == TouchPoint.OutsideRectangle)
            {
                _currentToolState = _createState;
            }
            else if (touch == TouchPoint.InsideRectangle)
            {
                _currentToolState = _dragState;
            }
            _currentToolState.OnMouseDown(point);
        }

        private TouchPoint GetTouchPoint(Point mousePoint)
        {
            //left
            if (mousePoint.X < _cropTool.TopLeftX)
                return TouchPoint.OutsideRectangle;
            //right
            if (mousePoint.X > _cropTool.BottomRightX)
                return TouchPoint.OutsideRectangle;
            //top
            if (mousePoint.Y < _cropTool.TopLeftY)
                return TouchPoint.OutsideRectangle;
            //bottom
            if (mousePoint.Y > _cropTool.BottomRightY)
                return TouchPoint.OutsideRectangle;

            return TouchPoint.InsideRectangle;
        }
    }
}
