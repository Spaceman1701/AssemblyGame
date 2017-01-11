using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler
{
    class InstructionInfo
    {
        private InstructionType inst;
        private ParameterType parameterOne;
        private ParameterType parameterTwo;

        public InstructionInfo(InstructionType it, ParameterType p1, ParameterType p2)
        {
            this.inst = it;
            this.parameterOne = p1;
            this.parameterTwo = p2;
        }

        public InstructionInfo(string data)
        {
            string[] splitData = data.Split(' ');
            inst = Program.InstructionFromString(splitData[0]);
            parameterOne = ParameterType.NONE;
            parameterTwo = ParameterType.NONE;
            if (splitData.Length > 1)
            {
                parameterOne = fromChar(splitData[1][0]);
                if (splitData[1].Length > 1)
                {
                    parameterTwo = fromChar(splitData[1][1]);
                }
            }
        }

        private static ParameterType fromChar(char c)
        {
            switch (c)
            {
                case 'R':
                    return ParameterType.REGISTER;
                case 'N':
                    return ParameterType.NUMBER;
                case 'L':
                    return ParameterType.LABEL;
                case 'M':
                    return ParameterType.POINTER;
                default:
                    return ParameterType.NONE;
            }
        }

        public override int GetHashCode()
        {
            return parameterOne.GetHashCode() * 100 + parameterTwo.GetHashCode() * 10 + inst.GetHashCode();
        }
    }
}
