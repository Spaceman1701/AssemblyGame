using Emulator.Compiler.CompileException;


namespace Emulator.Compiler.InstructionParameter
{
    public class NumberParam : Parameter
    {
        private ushort num;

        public NumberParam(string s)
        {
            if (!ushort.TryParse(s, out num))
            {
                throw new ParameterParseException(-1, "Could not parse immediate value");
            }
        }

        public ParameterType GetParamType()
        {
            return ParameterType.NUMBER;
        }

        public override string ToString()
        {
            return "NUM:" + num;
        }

        public ushort Num
        {
            get
            {
                return num;
            }
        }
    }
}
