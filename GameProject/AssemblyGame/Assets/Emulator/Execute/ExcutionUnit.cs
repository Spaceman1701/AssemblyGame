using Emulator.Compiler;
using Emulator.Compiler.InstructionParameter;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Emulator.Execute
{
    public class ExecutionUnit
    {
        public delegate void InteruptDelegate(ExecutionUnit eu);

        private const int GREATER_THAN = 2;
        private const int LESS_THAN = 1;
        private const int EQUAL_TO = 0;

        private const int AX = 0;
        private const int BX = 1;
        private const int CX = 2;
        private const int DX = 3;
        private const int SP = 4;
        private const int BP = 5;
        private const int SI = 6;
        private const int DI = 7;

        public const int POINTER_DEFERENCE_COST = 1;

        public static int GetRegisterIndex(string name)
        {
            switch (name)
            {
                case "ax": return 0;
                case "bx": return 1;
                case "cx": return 2;
                case "dx": return 3;
                case "sp": return 4;
                case "bp": return 5;
                case "si": return 6;
                case "di": return 7;
                default: return -1;
            }
        }


        private Register[] registers; //make a better way to access later
        private Register flags;
        private MemoryWord[] memory;

        private Stack<uint> callStack;

        private Program currentProgram;
        private uint currentLine;
        private uint nextLine;

        private IDictionary<InstructionType, MethodInfo> functions;

        private IDictionary<ushort, InteruptDelegate> interuptMap;

        private int currentStepCost;

        int numCalls;
        int numPops;

        public ExecutionUnit(int memorySize)
        {
            registers = new Register[8];
            for (int i = 0; i < 8; i++)
            {
                registers[i] = new Register();
            }
            flags = new Register();

            callStack = new Stack<uint>();

            memory = new MemoryWord[memorySize];
            for (int i = 0; i < memorySize; i++)
            {
                memory[i] = new MemoryWord();
            }

            interuptMap = new Dictionary<ushort, InteruptDelegate>();

            CreateMethodDictionary();

            ResetStack();
        }

        private void CreateMethodDictionary()
        {
            functions = new Dictionary<InstructionType, MethodInfo>();
            MethodInfo[] methods = this.GetType().GetMethods();
            foreach (MethodInfo m in methods)
            {
                string methodName = m.Name.ToLower();

                InstructionType iType = InstructionTypeExtension.fromString(methodName);
                if (iType != InstructionType.NONE)
                {
                    functions.Add(iType, m);
                }
            }
        }

        private void ResetStack()
        {
            registers[SP].Data = (ushort)memory.Length;
            registers[BP].Data = registers[SP].Data;
            callStack.Clear();
        }

        private void ClearMemory()
        {
            foreach (MemoryWord mw in memory)
            {
                mw.Data = 0;
            }
        }

        private void ClearRegisters()
        {
            foreach (Register r in registers)
            {
                r.Data = 0;
            }
        }

        public void SetProgram(Program p)
        {
            ResetStack();
            this.currentProgram = p;
            this.currentLine = currentProgram.GetEntryLine();
        }

        public bool IsRunningProgram()
        {
            return currentProgram != null;
        }

        public uint CurrentLine
        {
           get
            {
                return currentLine;
            }
        }

        public int MemorySize
        {
            get
            {
                return memory.Length;
            }
        }

        public MemoryWord ReadMemory(int ptr)
        {
            return memory[ptr];
        }

        public Register ReadRegister(int register)
        {
            return registers[register];
        }

        public int RunForCycles(int cycles)
        {
            int usedCycles = 0;
            while (usedCycles < cycles)
            {
                Step();
                usedCycles += currentStepCost;
            }
            return usedCycles - cycles;
        }

        public void Step()
        {
            currentStepCost = 0;
            nextLine = currentLine + 1;
            if (currentProgram == null)
            {
                return;
            }
            Instruction instruction = currentProgram.GetInstruction(currentLine);

            currentStepCost += instruction.GetInstructionType().GetCycles();

            MethodInfo method = functions[instruction.GetInstructionType()];
            method.Invoke(this, new System.Object[] {instruction.GetParams()});

            currentLine = nextLine;
        }

        public void RegisterInterrupt(ushort key, InteruptDelegate id)
        {
            interuptMap[key] = id;
        }

        private ushort DereferencePtr(PointerParam ptr)
        {
            currentStepCost += POINTER_DEFERENCE_COST;
            Stack<ushort> stack = new Stack<ushort>();
            Queue<string> readQ = new Queue<string>(ptr.ExpQueue);
            while (readQ.Count != 0)
            {
                string value = readQ.Dequeue().Trim();
                if (value == "+" || value == "-")
                {
                    ushort right = stack.Pop();
                    ushort left = stack.Pop();
                    switch (value)
                    {
                        case "+": 
                            stack.Push((ushort)(left + right));
                            break;
                        case "-":
                            stack.Push((ushort)(left - right));
                            break;
                    }
                    continue;
                }
                int reg = GetRegisterIndex(value);
                if (reg != -1)
                {
                    stack.Push(registers[reg].Data);
                    continue;
                }
                stack.Push(ushort.Parse(value));
            }

            return stack.Pop();
        }

        private ushort EvaluateValue(Parameter p)
        {
            if (p.GetParamType() == ParameterType.NUMBER)
            {
                return ((NumberParam)p).Num;
            }
            return registers[((RegisterParam)p).Reg].Data;
        }

        public void ProcStart(Parameter[] p)
        {

        }

        public void ProcEnd(Parameter[] p)
        {
            currentProgram = null;
        }

        public void Mov(Parameter[] p)
        {
            if (p[0].GetParamType() == ParameterType.REGISTER && p[1].GetParamType() == ParameterType.REGISTER)
            {
                Mov((RegisterParam)p[0], (RegisterParam)p[1]);
                return;
            }
            if (p[0].GetParamType() == ParameterType.REGISTER && p[1].GetParamType() == ParameterType.NUMBER)
            {
                Mov((RegisterParam)p[0], (NumberParam)p[1]);
                return;
            }
            if (p[0].GetParamType() == ParameterType.REGISTER && p[1].GetParamType() == ParameterType.POINTER)
            {
                Mov((RegisterParam)p[0], (PointerParam)p[1]);
                return;
            }
            if (p[0].GetParamType() == ParameterType.POINTER && p[1].GetParamType() == ParameterType.REGISTER)
            {
                Mov((PointerParam)p[0], (RegisterParam)p[1]);
                return;
            }
            if (p[0].GetParamType() == ParameterType.POINTER && p[1].GetParamType() == ParameterType.NUMBER)
            {
                Mov((PointerParam)p[0], (NumberParam)p[1]);
            }
        }

        private void Mov(PointerParam pointerParam, NumberParam numberParam)
        {
            ushort ptr = DereferencePtr(pointerParam);
            memory[ptr].Data = numberParam.Num;
        }

        private void Mov(RegisterParam p1, RegisterParam p2)
        {
            registers[p1.Reg].Data = registers[p2.Reg].Data;
        }

        private void Mov(RegisterParam p1, NumberParam p2)
        {
            registers[p1.Reg].Data = p2.Num;
        }

        private void Mov(RegisterParam p1, PointerParam p2)
        {
            registers[p1.Reg].Data = memory[DereferencePtr(p2)].Data;
        }

        private void Mov(PointerParam p1, RegisterParam p2)
        {
            ushort ptr = DereferencePtr(p1);
            memory[ptr].Data = registers[p2.Reg].Data;
        }


        public void Add(Parameter[] p)
        {
            if (p[1].GetParamType() == ParameterType.REGISTER)
            {
                Add((RegisterParam)p[0], (RegisterParam)p[1]);
                return;
            }
            if (p[1].GetParamType() == ParameterType.NUMBER)
            {
                Add((RegisterParam)p[0], (NumberParam)p[1]);
                return;
            }
        }

        private void Add(RegisterParam p1, RegisterParam p2)
        {
            registers[p1.Reg].Data = (ushort)(registers[p1.Reg].Data + registers[p2.Reg].Data);
        }

        private void Add(RegisterParam p1, NumberParam p2)
        {
            registers[p1.Reg].Data = (ushort)(registers[p1.Reg].Data + p2.Num);
        }


        public void Sub(Parameter[] p)
        {
            if (p[1].GetParamType() == ParameterType.REGISTER)
            {
                Sub((RegisterParam)p[0], (RegisterParam)p[1]);
            }
            if (p[1].GetParamType() == ParameterType.NUMBER)
            {
                Sub((RegisterParam)p[0], (NumberParam)p[0]);
            }
        }

        private void Sub(RegisterParam p1, RegisterParam p2)
        {
            registers[p1.Reg].Data = (ushort)(registers[p1.Reg].Data - registers[p2.Reg].Data);
        }

        private void Sub(RegisterParam p1, NumberParam p2)
        {
            registers[p1.Reg].Data = (ushort)(registers[p1.Reg].Data - registers[p2.Num].Data);

        }


        public void Div(Parameter[] p)
        {
            ushort divisor;
            if (p[0].GetParamType() == ParameterType.REGISTER)
            {
                divisor = registers[((RegisterParam)p[0]).Reg].Data;
            } else
            {
                divisor = ((NumberParam)p[0]).Num;
            }
            ushort value = (ushort)(registers[AX].Data / divisor);
            ushort remainder = (ushort)(registers[AX].Data % divisor);
            registers[AX].Data = value;
            registers[DX].Data = remainder;
        }

        public void Mul(Parameter[] p)
        {
            if (p[1].GetParamType() == ParameterType.REGISTER)
            {
                Mul((RegisterParam)p[0], (RegisterParam)p[1]);
                return;
            }
            if (p[1].GetParamType() == ParameterType.NUMBER)
            {
                Mul((RegisterParam)p[0], (NumberParam)p[1]);
            }
        }

        private void Mul(RegisterParam p1, RegisterParam p2)
        {
            registers[p1.Reg].Data = (ushort)(registers[p1.Reg].Data * registers[p2.Reg].Data);
        }

        private void Mul(RegisterParam p1, NumberParam p2)
        {
            registers[p1.Reg].Data = (ushort)(registers[p1.Reg].Data * p2.Num);

        }


        public void Shl(Parameter[] p)
        {
            Register reg = registers[((RegisterParam)p[0]).Reg];
            ushort shift;
            if (p[1].GetParamType() == ParameterType.REGISTER)
            {
                shift = registers[((RegisterParam)p[1]).Reg].Data;
            } else
            {
                shift = ((NumberParam)p[1]).Num;
            }
            reg.Data = (ushort)(reg.Data << shift);
        }

        public void Shr(Parameter[] p)
        {
            Register reg = registers[((RegisterParam)p[0]).Reg];
            ushort shift;
            if (p[1].GetParamType() == ParameterType.REGISTER)
            {
                shift = registers[((RegisterParam)p[1]).Reg].Data;
            }
            else
            {
                shift = ((NumberParam)p[1]).Num;
            }
            reg.Data = (ushort)((uint)reg.Data >> shift);
        }


        public void Cmp(Parameter[] p)
        {
            ushort v1 = registers[((RegisterParam)p[0]).Reg].Data;
            ushort v2;
            if (p[1].GetParamType() == ParameterType.REGISTER)
            {
                v2 = registers[((RegisterParam)p[1]).Reg].Data;
            } else
            {
                v2 = ((NumberParam)p[1]).Num;
            }
            if (v1 > v2)
            {
                flags.Data = GREATER_THAN;
            } else if (v1 < v2)
            {
                flags.Data = LESS_THAN;
            } else
            {
                flags.Data = EQUAL_TO;
            }
        }


        public void Jeq(Parameter[] p)
        {
            if (flags.Data == EQUAL_TO)
            {
                Jmp(p);
            }
        }

        public void Jgt(Parameter[] p)
        {
            if (flags.Data == GREATER_THAN)
            {
                Jmp(p);
            }
        }

        public void Jlt(Parameter[] p)
        {
            if (flags.Data == LESS_THAN)
            {
                Jmp(p);
            }
        }

        public void Jge(Parameter[] p)
        {
            if (flags.Data == GREATER_THAN || flags.Data == EQUAL_TO)
            {
                Jmp(p);
            }
        }

        public void Jle(Parameter[] p)
        {
            if (flags.Data == LESS_THAN || flags.Data == EQUAL_TO)
            {
                Jmp(p);
            }
        }

        public void Jmp(Parameter[] p)
        {
            nextLine = currentProgram.TranslateLabel((LabelParam)p[0]);
        }

        public void Jnq(Parameter[] p)
        {
            if (flags.Data != EQUAL_TO)
            {
                Jmp(p);
            }
        }


        public void Push(Parameter[] p)
        {
            ushort d = registers[((RegisterParam)p[0]).Reg].Data;
            memory[registers[SP].Data - 1].Data = d;
            registers[SP].Data -= 1;
        }

        public void Pop(Parameter[] p)
        {
            registers[((RegisterParam)p[0]).Reg].Data = memory[registers[SP].Data].Data;
            registers[SP].Data += 1;
        }

        public void Pushf(Parameter[] p) {
            memory[registers[SP].Data - 1].Data = flags.Data;
            registers[SP].Data -= 1;
        }

        public void Popf(Parameter[] p)
        {
            flags.Data = memory[registers[SP].Data].Data;
            registers[SP].Data += 1;
        }


        public void And(Parameter[] p)
        {
            RegisterParam p1 = (RegisterParam)p[0];
            ushort v1 = registers[p1.Reg].Data;
            ushort v2;
            if (p[1].GetParamType() == ParameterType.REGISTER)
            {
                v2 = registers[((RegisterParam)p[1]).Reg].Data;
            } else
            {
                v2 = ((NumberParam)p[1]).Num;
            }
            registers[p1.Reg].Data = (ushort)(v1 & v2);
        }

        
        public void Or(Parameter[] p)
        {
            RegisterParam p1 = (RegisterParam)p[0];
            ushort v1 = registers[p1.Reg].Data;
            ushort v2;
            if (p[1].GetParamType() == ParameterType.REGISTER)
            {
                v2 = registers[((RegisterParam)p[1]).Reg].Data;
            }
            else
            {
                v2 = ((NumberParam)p[1]).Num;
            }
            registers[p1.Reg].Data = (ushort)(v1 | v2);
        }


        public void Xor(Parameter[] p)
        {
            RegisterParam p1 = (RegisterParam)p[0];
            ushort v1 = registers[p1.Reg].Data;
            ushort v2;
            if (p[1].GetParamType() == ParameterType.REGISTER)
            {
                v2 = registers[((RegisterParam)p[1]).Reg].Data;
            }
            else
            {
                v2 = ((NumberParam)p[1]).Num;
            }
            registers[p1.Reg].Data = (ushort)(v1 ^ v2);
        }


        public void Not(Parameter[] p)
        {
            RegisterParam p1 = (RegisterParam)p[0];
            registers[p1.Reg].Data = (ushort)~registers[p1.Reg].Data;
        }

        
        public void Call(Parameter[] p0)
        {
            LabelParam l = (LabelParam)p0[0];
            registers[SP].Data--;
            memory[registers[SP].Data].Data = registers[BP].Data; //push bp
            registers[BP].Data = registers[SP].Data;
            callStack.Push(nextLine);
            nextLine = currentProgram.TranslateCall(l);
        }

        public void Ret(Parameter[] p)
        {
            nextLine = callStack.Pop();
            registers[SP].Data = registers[BP].Data; //new top of the stack
            registers[BP].Data = memory[registers[SP].Data].Data;
            registers[SP].Data++; //pop bp
        }

        public void Inc(Parameter[] p)
        {
            if (p[0].GetParamType() == ParameterType.REGISTER)
            {
                registers[((RegisterParam)p[0]).Reg].Data++;
            } else
            {
                ushort ptr = DereferencePtr((PointerParam)p[0]);
                memory[ptr].Data++;
            }
        }

        public void Dec(Parameter[] p)
        {
            if (p[0].GetParamType() == ParameterType.REGISTER)
            {
                registers[((RegisterParam)p[0]).Reg].Data--;
            }
            else
            {
                ushort ptr = DereferencePtr((PointerParam)p[0]);
                memory[ptr].Data--;
            }
        }

        public void Loop(Parameter[] p)
        {
            if (registers[CX].Data != 0)
            {
                registers[CX].Data--;
                Jmp(p);
            }
        }

        public void Int(Parameter[] p)
        {
            NumberParam n = (NumberParam)p[0];
            interuptMap[n.Num].Invoke(this);
        }

        public void Lea(Parameter[] p)
        {
            registers[((RegisterParam)p[0]).Reg].Data = DereferencePtr((PointerParam)p[1]);
        }
    }
}
