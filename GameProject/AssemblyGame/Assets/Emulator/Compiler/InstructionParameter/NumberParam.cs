using Emulator.Compiler.CompileException;
using System.Globalization;
using UnityEngine;

namespace Emulator.Compiler.InstructionParameter
{
    public class NumberParam : Parameter
    {
        private ushort num;

        public NumberParam(string s, int line)
        { 
            if (!Utils.TryParse(s, out num))
            {
                throw new ParameterParseException(line, ErrorCode.Instance.GetMessage("BAD_NUMBER"));
            }
        }


        public NumberParam(ushort num)
        {
            this.num = num;
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
