using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler
{
    class Params : Attribute
    {
        IList<ParameterType[]> paramList;
        int numParams;

        internal Params(string p)
        {
            string[] paramArray = p.Replace(" ", "").Split(',');
            paramList = new List<ParameterType[]>();
            foreach (string param in paramArray)
            {
                int length = param.Length;
                this.numParams = length;
                ParameterType[] array = new ParameterType[length];
                for (int i = 0; i < length; i++)
                {
                    switch (param[i])
                    {
                        case 'R':
                            array[i] = ParameterType.REGISTER;
                            break;
                        case 'N':
                            array[i] = ParameterType.NUMBER;
                            break;
                        case 'L':
                            array[i] = ParameterType.LABEL;
                            break;
                        case 'M':
                            array[i] = ParameterType.POINTER;
                            break;
                    }
                }
                paramList.Add(array);
            }
        }

        public IList<ParameterType[]> GetParamList()
        {
            return paramList;
        }

        public int GetNumParams()
        {
            return numParams;
        }
    }

    public enum InstructionType
    {
        [Params("RR, RN, RM, MR")]MOV,
        [Params("RR, RN")]ADD,
        [Params("RR, RN")]SUB,
        [Params("R, N")]DIV,
        [Params("RR, RN")]MUL,
        [Params("RR, RN")]CMP,
        [Params("L")]JEQ,
        [Params("L")]JGT,
        [Params("L")]JLT,
        [Params("L")]JGE,
        [Params("L")]JLE,
        [Params("R")]PUSH,
        [Params("R")]POP,
        [Params("")]PUSHF,
        [Params("")]POPF,
        [Params("RR, RN")]AND,
        [Params("RR, RN")]OR,
        [Params("RR, RN")]XOR,
        [Params("L")]CALL,
        [Params("")]RET,
        [Params("L")]PROCSTART,
        [Params("L")]PROCEND,
        [Params("")]NONE,
        [Params("RR, RN")] SHL,
        [Params("RR, RN")] SHR
    }

    public static class InstructionTypeExtension
    {
        public static InstructionType fromString(string s)
        {
            foreach (InstructionType t in Enum.GetValues(typeof(InstructionType))) {
                if (t.ToString().ToLower().Equals(s))
                {
                    return t;
                }
            }
            return InstructionType.NONE;
        }
        public static IList<ParameterType[]> Params(this InstructionType it)
        {
            return GetAttr(it).GetParamList();
        }

        public static bool HasParams(this InstructionType it)
        {
            Params p = GetAttr(it);
            return p.GetNumParams() > 0;
        }

        public static int NumParams(this InstructionType it)
        {
            return GetAttr(it).GetNumParams();
        }

        private static Params GetAttr(InstructionType it)
        {
            return (Params)Attribute.GetCustomAttribute(ForValue(it), typeof(Params));
        }

        private static MemberInfo ForValue(InstructionType p)
        {
            return typeof(InstructionType).GetField(Enum.GetName(typeof(InstructionType), p));
        }
    }
}
