using Emulator.Compiler.CompileException;
using System.Globalization;
using UnityEngine;

namespace Emulator.Compiler.InstructionParameter
{
    public class NumberParam : Parameter
    {
        private ushort num;

        public NumberParam(string s)
        {
            if (s.Contains("("))
            {
                s = s.Substring(1, s.Length - 2);
                ushort baseValue;
                ushort offset;
                int startOffset = s.IndexOf('[');
                int endIndex = s.Length - 1;
                int length = s.Length;


                string baseString = s.Substring(0, startOffset);
                if (!ushort.TryParse(baseString, out baseValue))
                {
                    throw new ParameterParseException(-1, "Could not parse memory pointer");
                }

                string offsetString = s.Substring(startOffset + 1, endIndex - startOffset - 1);
                if (!ushort.TryParse(offsetString, out offset))
                {
                    throw new ParameterParseException(-1, "Could not parse memory pointer");
                }
                num = (ushort)(baseValue + offset);
            } else if (!ushort.TryParse(s, out num))
            {
                throw new ParameterParseException(-1, "Could not parse immediate value " + s);
            }
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
