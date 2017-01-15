using System;

namespace Emulator.Compiler.CompileException
{
    public class CompilationException : Exception
    {
        private int line;
        public CompilationException(int line, string message) : base("On Line " + line + ": " + message)
        {
            this.line = line;
        }

        public int Line
        {
            get
            {
                return line;
            }
            set
            {
                line = value;
            }
        }

    }
}
