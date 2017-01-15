

namespace OldCPUEmulator.Compiler.CompileException
{
    public class ParameterParseException : CompilationException
    {
        public ParameterParseException(int line, string msg) : base(line, msg) {

        }
    }
}
