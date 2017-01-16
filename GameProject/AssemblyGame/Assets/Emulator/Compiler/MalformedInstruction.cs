


namespace Emulator.Compiler
{
    class MalformedInstruction : Instruction
    {
        private string message;

        public MalformedInstruction(string message)
        {
            this.message = message;
        }

        public InstructionType GetInstructionType()
        {
            return InstructionType.NONE;
        }

        public override string ToString()
        {
            return "MAL: " + message;
        }
    }
}
