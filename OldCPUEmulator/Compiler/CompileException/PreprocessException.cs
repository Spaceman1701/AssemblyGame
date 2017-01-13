using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler.CompileException
{
    class PreprocessException : CompilationException
    {
        public PreprocessException(int line, string message) : base(line, message)
        {
        }
    }
}
