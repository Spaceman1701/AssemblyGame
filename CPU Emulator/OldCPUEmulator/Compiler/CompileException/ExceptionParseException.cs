namespace OldCPUEmulator.Compiler.CompileException
{
    public class ExceptionParseException : CompilationException
    {
        public ExceptionParseException(int line, string msg) : base(line, msg)
        {

        }
    }
}
