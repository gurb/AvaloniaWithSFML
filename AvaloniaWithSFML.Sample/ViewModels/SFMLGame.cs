using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;


namespace AvaloniaWithSFML.Sample.ViewModels
{
    public class SFMLGame: BaseGame
    {
        Clock clock = new Clock();
        RectangleShape rectangle = new RectangleShape(new Vector2f(25, 25))
        {
            FillColor = SFML.Graphics.Color.Red
        };
        RectangleShape[] shapes = new RectangleShape[1000];
        bool movingUp = false;
        float speed = 100f;

        uint Width = 640;
        uint Height = 480;

        Vector2f selectionStart = new Vector2f(); 
        bool isSelecting = false;

        private float _zoom;
        public float Zoom { get; set; }
        View gameView = new View(new FloatRect(0,0,1024,720));

        Dictionary<float, float> ZoomLevels = new Dictionary<float, float>
        {
            { 0.0f, -0.50f },
            { 0.1f, -0.45f },
            { 0.2f, -0.40f },
            { 0.3f, -0.35f },
            { 0.4f, -0.30f },
            { 0.5f, -0.25f },
            { 0.6f, -0.20f },
            { 0.7f, -0.15f },
            { 0.8f, -0.10f },
            { 0.9f, -0.05f },
            { 1.0f,  0.00f  },
            { 1.1f,  0.05f },
            { 1.2f,  0.10f },
            { 1.3f,  0.15f },
            { 1.4f,  0.20f },
            { 1.5f,  0.25f },
            { 1.6f,  0.30f },
            { 1.7f,  0.35f },
            { 1.8f,  0.40f },
            { 1.9f,  0.45f },
            { 2.0f,  0.50f },
        };

        RectangleShape selectionRectangle = new RectangleShape
        {
            FillColor = new Color(0, 0, 255, 50),
            OutlineColor = Color.Blue,
            OutlineThickness = 1
        };

        public SFMLGame(Avalonia.Controls.Window aWindow, ContextSettings settings, uint width, uint height): base(aWindow, settings, width, height) 
        {
            rectangle.Position = new Vector2f(400, 300);
            this.Width = width;
            this.Height = height;
            if(RenderWindow is not null)
            {
                RenderWindow.Resized += RenderWindow_Resized;
                RenderWindow.MouseButtonPressed += RenderWindow_MouseButtonPressed;
                RenderWindow.MouseButtonReleased += RenderWindow_MouseButtonReleased;
            }
            SetClearColor(Color.Blue);

            for (int i = 0; i < shapes.Length; i++)
            {
                float S_width = new Random().Next(10, 50);
                float S_height = new Random().Next(10, 50);

                RectangleShape shape = new RectangleShape(new Vector2f(S_width, S_height))
                {
                    FillColor = new Color((byte)new Random().Next(256), (byte)new Random().Next(256), (byte)new Random().Next(256)),
                    Position = new Vector2f(new Random().Next((int)width), new Random().Next((int)height))
                };

                shapes[i] = shape;
            }
        }

        private void RenderWindow_MouseButtonReleased(object? sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                isSelecting = false;
            }
        }

        private void RenderWindow_MouseButtonPressed(object? sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                isSelecting = true;
                selectionStart = new Vector2f(e.X, e.Y);
                selectionRectangle.Position = selectionStart;
                selectionRectangle.Size = new Vector2f(0, 0);
            }
        }
        private void RenderWindow_Resized(object? sender, SizeEventArgs e)
        {
            Width = e.Width;
            Height = e.Height;
            
        }
        bool IsMouseOver(FloatRect rect, Vector2f mousePosition)
        {
            return mousePosition.X > rect.Left &&
                   mousePosition.X < rect.Left + rect.Width &&
                   mousePosition.Y > rect.Top &&
                   mousePosition.Y < rect.Top + rect.Height;
        }
        public void ChangeZoom(float zoom)
        {
            gameView.Reset(new FloatRect(0, 0, 1024, 720));
            gameView.Zoom(1 + ZoomLevels[zoom]);
            GeneralRender.SetView(gameView);
            _zoom = zoom;
        }
        public override void Update()
        {
            if (RenderWindow is not null)
            {
                RenderWindow.SetView(new View(new FloatRect(0, 0, Width, Height)));
                GeneralRender.SetView(gameView);
                Vector2i mousePosition = Mouse.GetPosition(RenderWindow);
                Vector2f mouseWorldPosition = GeneralRender.MapPixelToCoords(mousePosition);

                if (IsMouseOver(rectangle.GetGlobalBounds(), mouseWorldPosition))
                {
                    rectangle.FillColor = Color.Green;
                }
                else
                {
                    rectangle.FillColor = Color.Red; 
                }
            }
            float deltaTime = clock.Restart().AsSeconds();

            if (movingUp)
            {
                rectangle.Position = new Vector2f(rectangle.Position.X, rectangle.Position.Y - speed * deltaTime);
                if (rectangle.Position.Y <= 0) movingUp = false; 
            }
            else
            {
                rectangle.Position = new Vector2f(rectangle.Position.X, rectangle.Position.Y + speed * deltaTime);
                if (rectangle.Position.Y >= Height - rectangle.Size.Y) movingUp = true; 
            }

            base.Update();
        }

        public override void Draw()
        {
            foreach(var shape in shapes)
            {
                GeneralRender.Draw(shape);
            }

            GeneralRender.Draw(rectangle);

            if (isSelecting)
            {
                GeneralRender.Draw(selectionRectangle);
            }

            base.Draw();
        }
    }
}
