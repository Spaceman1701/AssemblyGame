using OldCPUEmulator.Compiler;
using OldCPUEmulator.Compiler.InstructionParameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Execute
{
    class ExcutionUnit
    {
        private const int GREATER_THAN = 2;
        private const int LESS_THAN = 1;
        private const int EQUAL_TO = 0;
        public static IDictionary<string, int> registerNameMap = new Dictionary<string, int>
        {
            ["ax"] = 0,
            ["bx"] = 1,
            ["cx"] = 2,
            ["dx"] = 3,
            ["sp"] = 4,
            ["bp"] = 5,
            ["si"] = 6,
            ["di"] = 7
        };

        private const int AX = 0;
        private const int BX = 1;
        private const int CX = 2;
        private const int DX = 3;
        private const int SP = 4;
        private const int BP = 5;
        private const int SI = 6;
        private const int DI = 7;


        public static int GetRegisterIndex(string name)
        {
            return registerNameMap[name];
        }


        public Register[] registers; //make a better way to access later
        public Register flags;
        public MemoryWord[] memory;

        Stack<uint> callStack;

        private Program currentProgram;
        private uint currentLine;
        private uint nextLine;

        public ExcutionUnit(int memorySize)
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
            ResetStack();
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

        public ushort ReadMemory(int ptr)
        {
            return memory[ptr].Data;
        }

        public ushort ReadRegister(int register)
        {
            return registers[register].Data;
        }

        public void Step()
        {
            nextLine = currentLine + 1;
            if (currentProgram == null)
            {
                return;
            }
            Compiler.Instruction inst = currentProgram.GetInstruction(currentLine);
            if (inst.GetType() == typeof(CompleteInstruction)) {
                CompleteInstruction instruction = (CompleteInstruction)inst;
                switch (instruction.getType())
                {
                    case InstructionType.MOV: Mov(instruction.GetParams()); break;
                    case InstructionType.ADD: Add(instruction.GetParams()); break;
                    case InstructionType.SUB: Sub(instruction.GetParams()); break;
                    case InstructionType.DIV: Div(instruction.GetParams()); break;
                    case InstructionType.MUL: Mul(instruction.GetParams()); break;
                    case InstructionType.SHL: Shl(instruction.GetParams()); break;
                    case InstructionType.SHR: Shr(instruction.GetParams()); break;
                    case InstructionType.CMP: Cmp(instruction.GetParams()); break;
                    case InstructionType.JEQ: Jeq(instruction.GetParams()); break;
                    case InstructionType.JGT: Jgt(instruction.GetParams()); break;
                    case InstructionType.JLT: Jlt(instruction.GetParams()); break;
                    case InstructionType.JGE: Jge(instruction.GetParams()); break;
                    case InstructionType.JLE: Jle(instruction.GetParams()); break;
                    case InstructionType.PUSH: Push(instruction.GetParams()); break;
                    case InstructionType.POP: Push(instruction.GetParams()); break;
                    case InstructionType.PUSHF: Pushf(); break;
                    case InstructionType.POPF: Popf(); break;
                    case InstructionType.AND: And(instruction.GetParams()); break;
                    case InstructionType.OR: Or(instruction.GetParams()); break;
                    case InstructionType.XOR: Xor(instruction.GetParams()); break;
                    case InstructionType.CALL: Call(instruction.GetParams()); break;
                    case InstructionType.RET: Ret(); break;
                    case InstructionType.PROCEND: currentProgram = null; break;
                }
            }

            currentLine = nextLine;
        }


        private ushort DereferencePtr(PointerParam ptr)
        {
            ushort location = (ushort)ptr.Base;
            if (ptr.HasRegisterOffset())
            {
                location += registers[ptr.Reg].Data;
            }
            else
            {
                location += ptr.FlatOffset;
            }
            return location;
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
                Jmp((LabelParam)p[0]);
            }
        }

        public void Jgt(Parameter[] p)
        {
            if (flags.Data == GREATER_THAN)
            {
                Jmp((LabelParam)p[0]);
            }
        }

        public void Jlt(Parameter[] p)
        {
            if (flags.Data == LESS_THAN)
            {
                Jmp((LabelParam)p[0]);
            }
        }

        public void Jge(Parameter[] p)
        {
            if (flags.Data == GREATER_THAN || flags.Data == EQUAL_TO)
            {
                Jmp((LabelParam)p[0]);
            }
        }

        public void Jle(Parameter[] p)
        {
            if (flags.Data == LESS_THAN || flags.Data == EQUAL_TO)
            {
                Jmp((LabelParam)p[0]);
            }
        }

        private void Jmp(LabelParam p)
        {
            nextLine = currentProgram.TranslateLabel(p);
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

        public void Pushf() {
            memory[registers[SP].Data - 1].Data = flags.Data;
            registers[SP].Data -= 1;
        }

        public void Popf()
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

        
        public void Call(Parameter[] p0)
        {
            LabelParam l = (LabelParam)p0[0];
            registers[BP].Data = registers[SP].Data;
            callStack.Push(nextLine);
            nextLine = currentProgram.TranslateCall(l);
        }

        public void Ret()
        {
            nextLine = callStack.Pop();
            registers[SP].Data = registers[BP].Data;
        }

    }
}
