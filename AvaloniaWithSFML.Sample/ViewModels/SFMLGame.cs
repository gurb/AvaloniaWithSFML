using Avalonia.Controls;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace AvaloniaWithSFML.Sample.ViewModels
{
    public class SFMLGame: BaseGame
    {
        Clock clock = new Clock();
        RectangleShape rectangle = new RectangleShape(new Vector2f(25, 25))
        {
            FillColor = SFML.Graphics.Color.Red // Kırmızı renk ile dikdörtgeni oluştur
        };
        RectangleShape[] shapes = new RectangleShape[1000];
        bool movingUp = false;
        float speed = 100f;

        uint Width = 640;
        uint Height = 480;

        Vector2f selectionStart = new Vector2f(); 
        bool isSelecting = false;

        RectangleShape selectionRectangle = new RectangleShape
        {
            FillColor = new Color(0, 0, 255, 50),
            OutlineColor = Color.Blue,
            OutlineThickness = 1
        };

        public SFMLGame(Avalonia.Controls.Window aWindow, ContextSettings settings): base(aWindow, settings) 
        {
            rectangle.Position = new Vector2f(400, 300);
            if(RenderWindow is not null)
            {
                RenderWindow.Resized += RenderWindow_Resized;
                RenderWindow.MouseButtonPressed += RenderWindow_MouseButtonPressed;
                RenderWindow.MouseButtonReleased += RenderWindow_MouseButtonReleased;
            }
            SetClearColor(Color.Blue);

            for (int i = 0; i < shapes.Length; i++)
            {
                // Rastgele boyutlar
                float width = new Random().Next(10, 50);
                float height = new Random().Next(10, 50);

                // RectangleShape oluştur
                RectangleShape shape = new RectangleShape(new Vector2f(width, height))
                {
                    FillColor = new Color((byte)new Random().Next(256), (byte)new Random().Next(256), (byte)new Random().Next(256)),
                    Position = new Vector2f(new Random().Next((int)640), new Random().Next((int)480))
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

        public override void Update()
        {
            if (RenderWindow is not null)
            {
                GeneralRender.SetView(new View(new FloatRect(0, 0, Width, Height)));
                Vector2i mousePosition = Mouse.GetPosition(RenderWindow);
                Vector2f mouseWorldPosition = GeneralRender.MapPixelToCoords(mousePosition);
                Trace.WriteLine(mouseWorldPosition);

                var test = rectangle.GetGlobalBounds();
                Trace.WriteLine(test);
                if (IsMouseOver(rectangle.GetGlobalBounds(), mouseWorldPosition))
                {
                    rectangle.FillColor = Color.Green; // Change color if mouse is over the rectangle
                }
                else
                {
                    rectangle.FillColor = Color.Red; // Default color
                }
            }
            //GeneralRender.SetView(new View(new FloatRect(0, 0, Width, Height)));
            float deltaTime = clock.Restart().AsSeconds();

            


            // Dikdörtgeni yukarı veya aşağı hareket ettir
            if (movingUp)
            {
                rectangle.Position = new Vector2f(rectangle.Position.X, rectangle.Position.Y - speed * deltaTime);
                if (rectangle.Position.Y <= 0) movingUp = false; // Üste geldiğinde aşağıya gitmeye başla
            }
            else
            {
                rectangle.Position = new Vector2f(rectangle.Position.X, rectangle.Position.Y + speed * deltaTime);
                if (rectangle.Position.Y >= 480 - rectangle.Size.Y) movingUp = true; // Aşağıya geldiğinde yukarıya gitmeye başla
            }

            base.Update();
        }

        public override void Draw()
        {
            // Tüm şekilleri çiz
            //foreach (var shape in shapes)
            //{
            //    RenderWindow.Draw(shape);
            //}

            //RenderWindow.Draw(rectangle);


            //foreach (var shape in shapes)
            //{
            //    GeneralRender.Draw(shape);
            //}

            GeneralRender.Draw(rectangle);

            if (isSelecting)
            {
                GeneralRender.Draw(selectionRectangle);
            }


            base.Draw();
        }

    }
}
