using Emulator.Compiler.CompileException;
using Emulator.Execute;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Emulator.Compiler.InstructionParameter
{
    public class PointerParam : Parameter
    {
        private int register = -1;
        private ushort baseValue;
        private ushort offset;

        private List<int> reg;

        private Queue<string> outputQueue;

        public PointerParam(string ptr, Dictionary<string, ushort> varMap, int line)
        {
            ptr = ptr.Replace("+", " + ").Replace("-", " - ");
            ptr = ptr.Substring(1, ptr.Length - 2);
            string[] exp = ptr.Split(' ');
            outputQueue = new Queue<string>();
            Stack<string> operatorStack = new Stack<string>();
            ushort temp;
            int numRegisters = 0;
            for (int i = 0; i < exp.Length; i++)
            {
                string token = exp[i];
                if (varMap.ContainsKey(token))
                {
                    token = varMap[token].ToString();
                }
                if (token == "+" || token == "-")
                {
                    operatorStack.Push(token);
                } else if (Utils.TryParse(token, out temp))
                {
                    outputQueue.Enqueue(temp.ToString());
                } else if (ExecutionUnit.GetRegisterIndex(token) != -1) {
                    numRegisters++;
                    outputQueue.Enqueue(token);
                } else
                {
                    throw new ParameterParseException(line, ErrorCode.Instance.GetMessage("BAD_POINTER"));
                }
            }

            if (numRegisters > 2) //make this not a magic number
            {
                throw new ParameterParseException(line, ErrorCode.Instance.GetMessageExpectedFound("OVER_REG_MATH_LIMIT", 2, numRegisters));
            }

            while (operatorStack.Count != 0)
            {
                outputQueue.Enqueue(operatorStack.Pop());
            }
        }

        public IList<int> Registers
        {
            get
            {
                return reg;
            }
        }

        public Queue<string> ExpQueue
        {
            get
            {
                return outputQueue;
            }
        }

        private string CleanExpression(string exp)
        {
            exp = exp.Replace(" +", "+").Replace("+ ", "+");
            exp = exp.Replace(" -", "-").Replace("- ", "-");
            return exp;
        }

        public ParameterType GetParamType()
        {
            return ParameterType.POINTER;
        }

        public override string ToString()
        {
            if (HasRegisterOffset())
            {
                return baseValue.ToString() + "+REG:" + register;
            }
            return "MEM:" + (baseValue + offset).ToString();
        }

        public bool HasRegisterOffset()
        {
            return register != -1;
        }

        public int Reg
        {
            get
            {
                return register;
            }
        }

        public ushort Base
        {
            get
            {
                return baseValue;
            }
        }

        public ushort FlatOffset
        {
            get
            {
                return offset;
            }
        }

    }
}
