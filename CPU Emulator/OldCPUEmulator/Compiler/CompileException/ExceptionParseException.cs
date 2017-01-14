using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler.CompileException
{
    class ExceptionParseException : CompilationException
    {
        public ExceptionParseException(int line, string msg) : base(line, msg)
        {

        }
    }
}
