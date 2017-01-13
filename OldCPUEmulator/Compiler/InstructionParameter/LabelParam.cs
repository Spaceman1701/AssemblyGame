using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler.InstructionParameter
{
    class LabelParam : Parameter
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
