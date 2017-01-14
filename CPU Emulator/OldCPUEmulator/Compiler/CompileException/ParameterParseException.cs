using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler.CompileException
{
    class ParameterParseException : CompilationException
    {
        public ParameterParseException(int line, string msg) : base(line, msg) {

        }
    }
}
