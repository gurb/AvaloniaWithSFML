using Avalonia.OpenGL;
using OpenTK;

namespace AvaloniaWithSFML
{
    public class AvaloniaBindingContext : IBindingsContext
    {
        private readonly GlInterface glInterface;

        public AvaloniaBindingContext(GlInterface glInterface)
        {
            this.glInterface = glInterface;
        }

        nint IBindingsContext.GetProcAddress(string procName)
        {
            return glInterface.GetProcAddress(procName);
        }
    }
}
