

namespace OldCPUEmulator.Compiler.InstructionParameter
{
    public class LabelParam : Parameter
    {
        private string label;

        public LabelParam(string s)
        {
            this.label = s.Trim();
        }

        public ParameterType GetParamType()
        {
            return ParameterType.LABEL;
        }

        public string Lbl
        {
            get
            {
                return label;
            }
        }

        public override string ToString()
        {
            return "LBL:" + label;
        }
    }
}
