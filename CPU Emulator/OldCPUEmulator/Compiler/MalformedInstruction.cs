


namespace OldCPUEmulator.Compiler
{
    class MalformedInstruction : Instruction
    {
        private string message;

        public MalformedInstruction(string message)
        {
            this.message = message;
        }

        public InstructionType getType()
        {
            return InstructionType.NONE;
        }

        public override string ToString()
        {
            return "MAL: " + message;
        }
    }
}
