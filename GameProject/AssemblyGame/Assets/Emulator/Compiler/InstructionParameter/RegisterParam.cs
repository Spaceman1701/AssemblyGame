using Assets.Emulator;
using Emulator.Compiler.CompileException;
using Emulator.Execute;
using System.Collections.Generic;
using UnityEngine;

namespace Emulator.Compiler.InstructionParameter
{
    public class RegisterParam : Parameter
    {
        private int reg;

        public RegisterParam(string reg, int line)
        {
            this.reg = ExecutionUnit.GetRegisterIndex(reg.Trim().ToLower());
            if (this.reg == -1)
            {
                throw new ParameterParseException(line, ErrorCode.Instance.GetMessage("BAD_REGISTER"));
            }
        }

        public RegisterParam(int reg)
        {
            this.reg = reg;
        }

        public ParameterType GetParamType()
        {
            return ParameterType.REGISTER;
        }

        public override string ToString()
        {
            return "REG:" + reg;
        }

        public int Reg
        {
            get
            {
                return reg;
            }
        }
    }
}
