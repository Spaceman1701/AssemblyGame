using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler.CompileException
{
    class CompilationException : Exception
    {
        public CompilationException(int line, string message) : base("On Line " + line + ": ")
        {

        }

    }
}
