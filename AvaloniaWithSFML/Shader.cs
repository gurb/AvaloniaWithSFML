using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaWithSFML
{
    public class Shader
    {
        public readonly int shaderProgram;
        
        public Shader(string vertSource, string fragSource)
        {
            try
            {
                var vertexShader = GL.CreateShader(ShaderType.VertexShader);

                GL.ShaderSource(vertexShader, vertSource);

                CompileShader(vertexShader);
                GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int status);
                if (status == (int)All.False)
                {
                    string infoLog = GL.GetShaderInfoLog(vertexShader);
                    throw new Exception($"Shader compilation failed: {infoLog}");
                }

                var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(fragmentShader, fragSource);
                CompileShader(fragmentShader);

                GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int statuse);
                if (statuse == (int)All.False)
                {
                    string infoLog = GL.GetShaderInfoLog(fragmentShader);
                    throw new Exception($"Shader compilation failed: {infoLog}");
                }

                shaderProgram = GL.CreateProgram();

                GL.AttachShader(shaderProgram, vertexShader);
                GL.AttachShader(shaderProgram, fragmentShader);

                LinkProgram(shaderProgram);

                GL.DetachShader(shaderProgram, vertexShader);
                GL.DetachShader(shaderProgram, fragmentShader);
                GL.DeleteShader(fragmentShader);
                GL.DeleteShader(vertexShader);

                GL.GetProgram(shaderProgram, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            }
            catch(Exception ex)
            {
                string message = ex.Message;
            }
            
        }

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }

        public void UseProgram()
        {
            GL.UseProgram(shaderProgram);
        }
    }
}
