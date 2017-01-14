using OldCPUEmulator.Compiler.CompileException;
using OldCPUEmulator.Execute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler.InstructionParameter
{
    class PointerParam : Parameter
    {
        private int register = -1;
        private ushort baseValue;
        private ushort offset;
        private string ptr;
        
        public PointerParam(string ptr)
        {
            this.ptr = ptr;
            int offsetIndex = ptr.Length - ptr.IndexOf('[');
            int startOffset = ptr.IndexOf('[');
            int endIndex = ptr.Length - 1;
            int length = ptr.Length;


            string baseString = ptr.Substring(0, startOffset);
            if (!ushort.TryParse(baseString, out baseValue))
            {
                throw new ParameterParseException(-1, "Could not parse memory pointer");
            }

            string offsetString = ptr.Substring(startOffset + 1, endIndex - startOffset - 1);

            if (!ushort.TryParse(offsetString, out offset))
            {
                register = new RegisterParam(offsetString).Reg;
            }
        }

        public ParameterType GetParamType()
        {
            return ParameterType.POINTER;
        }

        public override string ToString()
        {
            ushort printVal = baseValue;
            if (HasRegisterOffset())
            {
                return baseValue.ToString() + "+REG:" + register;
            }
            return "MEM:" + (baseValue + offset).ToString();
        }

        public bool HasRegisterOffset()
        {
            return register != -1;
        }

        public int Reg
        {
            get
            {
                return register;
            }
        }

        public ushort Base
        {
            get
            {
                return baseValue;
            }
        }

        public ushort FlatOffset
        {
            get
            {
                return offset;
            }
        }

    }
}
