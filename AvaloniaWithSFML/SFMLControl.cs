using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;


namespace AvaloniaWithSFML
{
    public sealed class SFMLControl: Control
    {
        public static readonly DirectProperty<SFMLControl, BaseGame?> WindowProperty =
        AvaloniaProperty.RegisterDirect<SFMLControl, BaseGame?>(
            nameof(BaseGame),
            o => o.Window,
            (o, v) => o.Window = v);

        private byte[] _bufferData = Array.Empty<byte>();
        private WriteableBitmap? _bitmap;
        private bool _isInitialized;
        private BaseGame? _window;
        
        public IBrush FallbackBackground { get; set; } = Brushes.Purple;


        public SFMLControl()
        {
            Focusable = true;
        }

        public BaseGame? Window
        {
            get => _window;
            set
            {
                if (_window == value) return;
                _window = value;

                if (_isInitialized)
                {
                    Initialize();
                }
            }
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            finalSize = base.ArrangeOverride(finalSize);
            if(_window is not  null)
            {
                //_window.RenderWindow.Size = new Vector2u((uint)finalSize.Width, (uint)finalSize.Height);
            }
            if (finalSize != _bitmap?.Size)
            {
                Reset();
            }


            return finalSize;
        }
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            Start();
        }

        private void Reset()
        {
            _bitmap = new WriteableBitmap(
                    new PixelSize((int)_window.RenderWindow.Size.X, (int)_window.RenderWindow.Size.Y),
                    new Vector(96d, 96d),
                    PixelFormat.Rgba8888,
                    AlphaFormat.Opaque);
        }

        private void Start()
        {
            if (_isInitialized)
            {
                return;
            }

            Initialize();
            _isInitialized = true;
        }
        public override void Render(DrawingContext context)
        {
            if (Window is not { } sfmlWindow
                || _bitmap is null
                || Bounds is { Width: < 1, Height: < 1 })
            {
                context.DrawRectangle(FallbackBackground, null, new Rect(Bounds.Size));
                return;
            }
            
            RunFrame(_window);
            
            //CaptureFrame(_window.RenderWindow, _bitmap);

            context.DrawImage(_bitmap, new Rect(_bitmap.Size), Bounds);
        }


        private void Initialize()
        {
            if (this.GetVisualRoot() is Avalonia.Controls.Window { PlatformImpl: { } } window &&
                window.TryGetPlatformHandle()?.Handle is IntPtr handle)
            {
                _bitmap = new WriteableBitmap(
                    new PixelSize((int)_window.RenderWindow.Size.X, (int)_window.RenderWindow.Size.Y),
                    new Vector(96d, 96d),
                    PixelFormat.Rgba8888,
                    AlphaFormat.Opaque);

                //_window.RenderWindow.Closed += (_, _) => _window.RenderWindow.Close();
            }

            RunFrame(_window);
        }

        private void RunFrame(BaseGame? window)
        {
            if(window is null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            try
            {
                _window.OneFrame();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
            }
        }
    }
}
