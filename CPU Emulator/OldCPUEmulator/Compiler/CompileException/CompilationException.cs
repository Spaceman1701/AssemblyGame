using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler.CompileException
{
    class CompilationException : Exception
    {
        private int line;
        public CompilationException(int line, string message) : base("On Line " + line + ": ")
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
