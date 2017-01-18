using Emulator.Compiler.CompileException;
using Emulator.Execute;
using UnityEngine;

namespace Emulator.Compiler.InstructionParameter
{
    public class RegisterParam : Parameter
    {
        private int reg;

        public RegisterParam(string reg)
        {
            this.reg = ExecutionUnit.GetRegisterIndex(reg.Trim().ToLower());
            if (this.reg == -1)
            {
                throw new ParameterParseException(-1, "Could not parse register reference: " + reg);
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
