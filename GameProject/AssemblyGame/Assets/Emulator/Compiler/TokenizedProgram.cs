using Emulator.Execute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Emulator.Compiler
{
    public class TokenizedProgram
    {
        public const char TOKEN_SEP = '$';
        public const string CODE_START = ".Code";
        public const string DATA_START = ".Data";
        public const string VAR_DEC = "const";

        private int numLines;
        private IList<Token>[] data;

        private int dataStart = -1;
        private int dataEnd = -1;

        private int codeStart = -1;
        private int codeEnd = -1;

        public TokenizedProgram(string[] program)
        {
            Tokenize(program);
            CalcDataRange();
            CalcCodeRange();
        }


        private void CalcDataRange()
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i][0].Type == Token.TokenType.DATA_START)
                {
                    dataStart = i;
                }
                if (data[i][0].Type == Token.TokenType.CODE_START && dataStart != -1)
                {
                    dataEnd = i;
                    return; //found both values
                }
            }
            dataEnd = data.Length;
        }

        private void CalcCodeRange()
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i][0].Type == Token.TokenType.CODE_START)
                {
                    codeStart = i;
                }
                if (data[i][0].Type == Token.TokenType.DATA_START && codeStart != -1)
                {
                    codeEnd = i;
                    return; //found both values
                }
            }
            codeEnd = data.Length;
        }

        public IList<Token>[] Data
        {
            get
            {
                return data;
            }
        }

        public int DataStart
        {
            get
            {
                return dataStart;
            }

            set
            {
                dataStart = value;
            }
        }

        public int DataEnd
        {
            get
            {
                return dataEnd;
            }

            set
            {
                dataEnd = value;
            }
        }

        public int CodeStart
        {
            get
            {
                return codeStart;
            }

            set
            {
                codeStart = value;
            }
        }

        public int CodeEnd
        {
            get
            {
                return codeEnd;
            }

            set
            {
                codeEnd = value;
            }
        }

        private void Tokenize(string[] program)
        {
            numLines = program.Length;

            data = new IList<Token>[numLines];

            for (int i = 0; i < numLines; i++)
            {
                int lineNum = i + 1;
                IList<Token> lineData = new List<Token>();
                data[i] = lineData;
                string[] tokenStrings = program[i].Trim().Split(TOKEN_SEP);
                if (tokenStrings.Length == 0 || tokenStrings[0].Trim() == "")
                {
                    lineData.Add(new Token(Token.TokenType.NOP, "", lineNum));
                    continue;
                }
                lineData.Add(ProcessFirstToken(tokenStrings[0], lineNum));
                for (int j = 1; j < tokenStrings.Length; j++)
                {
                    if (tokenStrings[j] != "")
                    {
                        string tString = tokenStrings[j];
                        if (IsRegister(tString))
                        {
                            lineData.Add(new Token(Token.TokenType.REG_ARG, tString, lineNum));
                        }
                        else if (IsNumber(tString))
                        {
                            lineData.Add(new Token(Token.TokenType.NUM_ARG, tString, lineNum));
                        }
                        else if (tString.Contains("["))
                        {
                            lineData.Add(new Token(Token.TokenType.PTR_ARG, tString, lineNum));
                        }
                        else
                        {
                            lineData.Add(new Token(Token.TokenType.LBL_ARG, tString, lineNum));
                        }
                    }
                }
            }
        }

        private bool IsRegister(string data)
        {
            return ExecutionUnit.GetRegisterIndex(data) != -1;
        }

        private bool IsNumber(string data)
        {
            ushort n;
            return ushort.TryParse(data, out n);
        }

        private Token ProcessFirstToken(string data, int line)
        {
            Token t;
            if (InstructionTypeExtension.isInstruction(data))
            {
                t = new Token(Token.TokenType.INST, data, line);
            } else if (data == CODE_START)
            {
                t = new Token(Token.TokenType.CODE_START, data, line);
            } else if (data == DATA_START)
            {
                t = new Token(Token.TokenType.DATA_START, data, line);
            } else if (data == VAR_DEC)
            {
                t = new Token(Token.TokenType.VAR_DEC, data, line);
            } else if (data.Contains("["))
            {
                t = new Token(Token.TokenType.ARRAY_DEC, data, line);
            } else
            {
                t = new Token(Token.TokenType.LBL, data, line);
            }

            return t;
        }
    }
}
