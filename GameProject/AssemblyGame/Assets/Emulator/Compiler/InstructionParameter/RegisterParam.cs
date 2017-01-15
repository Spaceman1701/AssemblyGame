using Emulator.Compiler.CompileException;
using Emulator.Execute;


namespace Emulator.Compiler.InstructionParameter
{
    public class RegisterParam : Parameter
    {
        private int reg;

        public RegisterParam(string reg)
        {
            this.reg = ExcutionUnit.GetRegisterIndex(reg.Trim());
            if (this.reg == -1)
            {
                throw new ParameterParseException(-1, "Could not parse register reference");
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
