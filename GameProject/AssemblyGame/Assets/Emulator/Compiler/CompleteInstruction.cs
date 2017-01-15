
using Emulator.Compiler.InstructionParameter;

namespace Emulator.Compiler
{
    class CompleteInstruction : Instruction
    {
        private uint line;
        private InstructionType type;
        private Parameter[] parameters;

        public CompleteInstruction(InstructionType type, Parameter[] parameters, uint line)
        {
            this.type = type;
            this.parameters = parameters;
            this.line = line;
        }

        public Parameter[] GetParams()
        {
            return parameters;
        }

        public InstructionType getType()
        {
            return type;
        }

        public override string ToString()
        {
            string parameterString = "";
            foreach (Parameter p in parameters)
            {
                parameterString += p.ToString() + " ";
            }
            return line + ": " + type.ToString() + " " + parameterString;
        }
    }
}
