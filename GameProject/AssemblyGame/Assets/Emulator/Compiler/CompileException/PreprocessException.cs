

namespace Emulator.Compiler.CompileException
{
    public class PreprocessException : CompilationException
    {
        public PreprocessException(int line, string message) : base(line, message)
        {
        }
    }
}
