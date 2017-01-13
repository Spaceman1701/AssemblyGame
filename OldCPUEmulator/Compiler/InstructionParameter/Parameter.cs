using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler.InstructionParameter
{
    interface Parameter
    {
        ParameterType GetParamType();
    }
}