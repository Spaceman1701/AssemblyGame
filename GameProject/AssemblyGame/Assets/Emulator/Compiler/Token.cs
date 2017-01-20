using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emulator.Compiler
{
    public class Token
    {
        public enum TokenType
        {
            INST, REG_ARG, NUM_ARG, PTR_ARG, LBL_ARG, LBL, NOP, DATA_START, CODE_START, ARRAY_DEC, VAR_DEC
        }

        private TokenType t;
        private string data;
        private int line;

        public Token(TokenType t, string data, int line)
        {
            this.t = t;
            this.data = data.Trim();
            this.line = line;
        }


        public override string ToString()
        {
            return t.ToString() + ":" + data;
        }

        public TokenType Type
        {
            get
            {
                return t;
            }

            set
            {
                t = value;
            }
        }

        public string Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }

        public int Line
        {
            get
            {
                return line;
            }

            set
            {
                line = value;
            }
        }
    }

}
