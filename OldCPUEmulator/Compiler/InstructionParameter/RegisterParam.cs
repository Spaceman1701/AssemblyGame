﻿using OldCPUEmulator.Execute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler.InstructionParameter
{
    class RegisterParam : Parameter
    {
        private int reg;

        public RegisterParam(string reg)
        {
            this.reg = ExcutionUnit.GetRegisterIndex(reg.Trim());
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