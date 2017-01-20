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

        public PointerParam(string ptr)
        {
            ptr = ptr.Trim();
            reg = new List<int>();
            if (ptr[0] == '[')
            {
                ParseNew(ptr);
                return;
            }
            int startOffset = ptr.IndexOf('[');
            int endIndex = ptr.Length - 2;
            int length = ptr.Length;


            string baseString = ptr.Substring(0, startOffset);
            if (!ushort.TryParse(baseString, out baseValue))
            {
                throw new ParameterParseException(-1, "Could not parse memory pointer");
            }
            Debug.Log(endIndex - startOffset + 1);

            string offsetString = ptr.Substring(startOffset + 1, endIndex - startOffset);

            if (!ushort.TryParse(offsetString, out offset))
            {
                Debug.Log("pointer reg: " + offsetString);
                register = new RegisterParam(offsetString).Reg;
                reg.Add(register);
            }
        }

        public PointerParam(string ptr, Dictionary<string, ushort> varMap)
        {
            ptr = ptr.Replace("+", " + ").Replace("-", " - ").Substring(1, ptr.Length);
            Debug.Log(ptr);
            string[] exp = ptr.Split(' ');
            outputQueue = new Queue<string>();
            Stack<string> operatorStack = new Stack<string>();
            ushort temp;
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
                } else if (ushort.TryParse(token, out temp))
                {
                    outputQueue.Enqueue(temp.ToString());
                } else
                {
                    throw new Exception(token);
                }
            }

            while (operatorStack.Count != 0)
            {
                outputQueue.Enqueue(operatorStack.Pop());
            }
        }

        private void ParseNew(string ptr)
        {
            ptr = ptr.Trim().Substring(1, ptr.Length - 1);
            string exp = CleanExpression(ptr);

            reg = new List<int>();
            baseValue = 0;
            if (exp.Contains("+"))
            {
                string[] split = exp.Split('+');
                foreach (string s in split)
                {
                    Debug.Log(s);
                    if (ExecutionUnit.GetRegisterIndex(s) != -1)
                    {
                        reg.Add(ExecutionUnit.GetRegisterIndex(s));
                    } else
                    {
                        baseValue += ushort.Parse(s);
                    }
                }
            } else if (exp.Contains("-"))
            {
                string[] split = exp.Split('-');
                int numReg = 0;
                foreach (string s in split)
                {
                    if (ExecutionUnit.GetRegisterIndex(s) != -1)
                    {
                        if (numReg > 0)
                        {
                            throw new ParameterParseException(-1, "Cannot subtract registers");
                        }
                        reg.Add(ExecutionUnit.GetRegisterIndex(s));
                    }
                    else
                    {
                        baseValue -= ushort.Parse(s);
                    }
                }
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
