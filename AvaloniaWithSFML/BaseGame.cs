using SFML.Graphics;
using SFML.Window;


namespace AvaloniaWithSFML
{
    public class BaseGame
    {
        uint Width, Height;

        RenderWindow? renderWindow { get; set; }
        RenderTexture? GeneralRenderTexture { get; set; }

        public RenderWindow RenderWindow
        {
            get { return renderWindow!; }
        }

        public RenderTexture GeneralRender
        {
            get { return GeneralRenderTexture!; }
            set { GeneralRenderTexture = value; }
        }

        SFML.Graphics.Color ClearColor { get; set; } = SFML.Graphics.Color.Blue;

        public void SetClearColor(SFML.Graphics.Color clearColor)
        {
            ClearColor = clearColor;
        }

        public BaseGame(Avalonia.Controls.Window aWindow, ContextSettings contextSettings, uint width, uint height)
        {
            if (aWindow is Avalonia.Controls.Window { PlatformImpl: { } } window &&
                window.TryGetPlatformHandle()?.Handle is IntPtr handle)
            {
                renderWindow = new SFML.Graphics.RenderWindow(handle, new ContextSettings { AntialiasingLevel = 16 });
                renderWindow.Size = new SFML.System.Vector2u(width, height);
                //renderWindow.SetFramerateLimit(60);

                this.Width = width;
                this.Height = height;

                GeneralRenderTexture = new RenderTexture(width, height);
                GeneralRenderTexture.Clear(SFML.Graphics.Color.Yellow);
                GeneralRenderTexture.Display();
            }
        }

        private void RenderWindow_Resized(object? sender, SizeEventArgs e)
        {
            if(renderWindow is not null)
            {
                renderWindow.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
            }
            //GeneralRender.SetView(new View(new FloatRect(0,0,e.Width, e.Height)));
        }

        public virtual void HandleEvents()
        {
            if (renderWindow is not null)
            {
                renderWindow!.DispatchEvents();
            }
        }

        public virtual void Update()
        {
        }

        private void Clear() 
        {
            //renderWindow.Clear(SFML.Graphics.Color.Blue);
            GeneralRender!.Clear(ClearColor);
        }

        public virtual void Draw()
        {
            // override this
        }

        private void DrawTogether()
        {
            this.Clear();
            this.Draw();
        }

        public void OneFrame()
        {
            HandleEvents();
            Update();
            DrawTogether();
            GeneralRenderTexture!.Display();
            //renderWindow.Display();
        }

        public virtual void Destroy() { }
    }
}
