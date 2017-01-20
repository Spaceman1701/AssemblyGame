
using Emulator.Compiler.InstructionParameter;

namespace Emulator.Compiler
{
    public class Instruction
    {
        private uint line;
        private InstructionType type;
        private Parameter[] parameters;

        public Instruction(InstructionType type, Parameter[] parameters, uint line)
        {
            this.type = type;
            this.parameters = parameters;
            this.line = line;
        }

        public Parameter[] GetParams()
        {
            return parameters;
        }

        public InstructionType GetInstructionType()
        {
            return type;
        }

        public override string ToString()
        {
            string parameterString = "";
            foreach (Parameter p in parameters)
            {
                if (p != null)
                {
                    parameterString += p.ToString() + " ";
                }
            }
            return line + ": " + type.ToString() + " " + parameterString;
        }
    }
}
