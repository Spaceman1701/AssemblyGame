using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Emulator.Compiler
{
    class Params : Attribute
    {
        IList<ParameterType[]> paramList;
        int numParams;
        int cycles;

        internal Params(string p, int cycles)
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
            this.cycles = cycles;
        }

        public IList<ParameterType[]> GetParamList()
        {
            return paramList;
        }

        public int GetNumParams()
        {
            return numParams;
        }

        public int Cycles
        {
            get
            {
                return cycles;
            }
        }
    }

    public enum InstructionType
    {
        [Params("RR, RN, RM, MR", 1)]MOV,
        [Params("RR, RN", 1)]ADD,
        [Params("RR, RN", 1)]SUB,
        [Params("R, N", 3)]DIV,
        [Params("RR, RN", 2)]MUL,
        [Params("RR, RN", 1)]CMP,
        [Params("L", 1)]JEQ,
        [Params("L", 1)]JNQ,
        [Params("L", 1)]JGT,
        [Params("L", 1)]JLT,
        [Params("L", 1)]JGE,
        [Params("L", 1)]JLE,
        [Params("L", 1)]JMP,
        [Params("R", 1)]PUSH,
        [Params("R", 1)]POP,
        [Params("", 1)]PUSHF,
        [Params("", 1)]POPF,
        [Params("RR, RN", 1)]AND,
        [Params("RR, RN", 1)]OR,
        [Params("RR, RN", 1)]XOR,
        [Params("R", 1)]NOT,
        [Params("L", 1)]CALL,
        [Params("", 1)]RET,
        [Params("L", 1)]PROCSTART,
        [Params("L", 1)]PROCEND,
        [Params("", 1)]NONE,
        [Params("RR, RN", 1)] SHL,
        [Params("RR, RN", 1)] SHR,
        [Params("R, M", 1)] INC,
        [Params("R, M", 1)] DEC,
        [Params("L", 1)] LOOP,
        [Params("N", 1)] INT,
        [Params("RM", 2)] LEA
    }

    public static class InstructionTypeExtension
    {
        public static InstructionType fromString(string s)
        {
            foreach (InstructionType t in Enum.GetValues(typeof(InstructionType))) {
                if (t.ToString().ToLower().Equals(s.Trim()))
                {
                    return t;
                }
            }
            return InstructionType.NONE;
        }

        public static bool isInstruction(string s)
        {
            return fromString(s) != InstructionType.NONE;
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

        public static int GetCycles(this InstructionType it)
        {
            return GetAttr(it).Cycles;
        }
    }
}
