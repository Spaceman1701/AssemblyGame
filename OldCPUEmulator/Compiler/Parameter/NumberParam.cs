using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler.InstructionParameter
{
    class NumberParam : Parameter
    {
        private ushort num;

        public NumberParam(string s)
        {
            num = ushort.Parse(s);
        }

        public ParameterType GetParamType()
        {
            return ParameterType.NUMBER;
        }

        public override string ToString()
        {
            return "NUM:" + num;
        }

        public ushort Num
        {
            get
            {
                return num;
            }
        }
    }
}
