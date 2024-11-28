using Avalonia;
using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SFML.Graphics;
using System.Numerics;
using ErrorCode = OpenTK.Graphics.OpenGL4.ErrorCode;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace AvaloniaWithSFML
{
    public class SFMLControl : OpenGlControlBase
    {
        public static readonly DirectProperty<SFMLControl, BaseGame?> WindowProperty =
        AvaloniaProperty.RegisterDirect<SFMLControl, BaseGame?>(
            nameof(BaseGame),
            o => o.Window,
            (o, v) => o.Window = v);
        private BaseGame? _window;
        private bool _isInitialized;

        string vertexShaderSource = @"#version 300 es
        precision mediump float;
        layout(location = 0) in vec3 aPosition;
        layout(location = 1) in vec2 aTexCoord;    
        out vec2 TexCoords;

        void main(void)
        {
            gl_Position = vec4(aPosition.xy, 0, 1.0);
            TexCoords = aTexCoord;
        }
        ";
        string fragmentShaderSource = @"#version 300 es
        precision mediump float;
        out vec4 outputColor;

        uniform vec4 ourColor;

        uniform sampler2D screenTexture;

        in vec2 TexCoords;

        void main()
        {
            vec4 texColor = texture(screenTexture, TexCoords);
            outputColor = texColor;
        }";

        private Shader _shader;

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
        public SFMLControl()
        {
            Focusable = true;
        }
        private void Initialize()
        {
            OnLoad();
            RunFrame(_window);
        }
        private void RunFrame(BaseGame? window)
        {
            if (window is null)
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
                Dispatcher.UIThread.Post(RequestNextFrameRendering, DispatcherPriority.Background);
            }
        }

        private static readonly uint[] indices = {
            0, 1, 2,
            2, 3, 0
        };
        private int shaderProgram;
        private int textureID;
        private bool isInitialized = false;
        RenderTexture renderTexture;

        private readonly float[] _vertices =
        {
             -1.0f,  -1.0f,   0.0f,  0.0f,  0.0f,
              1.0f,  -1.0f,   0.0f,  1.0f,  0.0f,
              1.0f,   1.0f,   0.0f,  1.0f,  1.0f,
             -1.0f,   1.0f,   0.0f,  0.0f,  1.0f
        };

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            if (_window is not null)
            {
               
            }

            base.OnSizeChanged(e);
        }

        private int vbo;
        private int vao;
        private int ebo;

        private int mainTextureID;
        private  void trInitializeBuffers()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.DynamicDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);


            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Texture koordinatları için veri
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
           
            _shader = new Shader(vertexShaderSource, fragmentShaderSource);
            OnLoad();

            _shader.UseProgram();
            mainTextureID = (int)renderTexture.Texture.NativeHandle;
            GL.BindTexture(TextureTarget.Texture2D, mainTextureID);

            GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    (int)renderTexture.Texture.Size.X,   
                    (int)renderTexture.Texture.Size.Y,
                    0,                   
                    OpenTK.Graphics.OpenGL4.PixelFormat.Rgba,  
                    PixelType.UnsignedByte,
                    renderTexture.Texture.CopyToImage().Pixels
            );

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }

        private void ResizeOfTexture()
        {
            int mainTextureID = (int)renderTexture.Texture.NativeHandle;
            GL.BindTexture(TextureTarget.Texture2D, mainTextureID);
            var image = _window.GeneralRender.Texture.CopyToImage();
            image.FlipVertically();
            _window.GeneralRender.Texture.CopyToImage().FlipVertically();
            GL.TexSubImage2D(
                    TextureTarget.Texture2D,
                    0,
                    0,
                    (int)_window.GeneralRender.Size.X,
                    (int)_window.GeneralRender.Size.Y,
                    0,
                    OpenTK.Graphics.OpenGL4.PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    image.Pixels
            );
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        private void OnLoad()
        {
            renderTexture = new RenderTexture(_window.RenderWindow.Size.X, _window.RenderWindow.Size.Y)
            {
            };
            renderTexture.Clear(SFML.Graphics.Color.Cyan);
            renderTexture.Display();
        }

        GlInterface gl;
        int fb;
        protected override void OnOpenGlInit(GlInterface gl)
        {
            this.gl = gl;
            base.OnOpenGlInit(gl);
            var version = gl.ContextInfo.Version;
            
            GL.LoadBindings(new AvaloniaBindingContext(gl));
            //InitializeBuffers();
            trInitializeBuffers();
            isInitialized = true;
        }
       
        protected override void OnOpenGlRender(GlInterface gl, int fb)
        {
            this.fb = fb;
            GL.Viewport(0, 0, (int)Bounds.Width, (int)Bounds.Height);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            GL.ClearColor(new Color4(0, 32, 48, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            _window.OneFrame();


            // Bind the shader
            _shader.UseProgram();

            int mainTextureID = (int)renderTexture.Texture.NativeHandle;
            var image = _window.GeneralRender.Texture.CopyToImage();
            image.FlipVertically();
            GL.BindTexture(TextureTarget.Texture2D, mainTextureID);
            GL.TexSubImage2D(
                TextureTarget.Texture2D,
                0,
                0,
                0,
                (int)_window.GeneralRender.Texture.Size.X,   
                (int)_window.GeneralRender.Texture.Size.Y,
                OpenTK.Graphics.OpenGL4.PixelFormat.Rgba,
                PixelType.UnsignedByte,
                image.Pixels
            );
           
            GL.BindVertexArray(vao);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            Dispatcher.UIThread.Post(RequestNextFrameRendering, DispatcherPriority.Background);
        }

        private void CheckGLError(string operation)
        {
            ErrorCode error = GL.GetError();  // OpenGL hatasını al
            if (error != ErrorCode.NoError)
            {
                Console.WriteLine($"OpenGL Error after {operation}: {error}");
            }
        }
    }   
}
