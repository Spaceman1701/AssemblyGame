﻿using Emulator.Compiler.CompileException;
using Emulator.Compiler.InstructionParameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Emulator.Compiler
{
    class TokenizedProgramCompiler
    {
        public static Program Compile(TokenizedProgram tokens)
        {
            Dictionary<string, uint> labelMap = new Dictionary<string, uint>();
            Dictionary<string, uint> procMap = new Dictionary<string, uint>();
            Dictionary<string, ushort> varMap = new Dictionary<string, ushort>();
            IList<Instruction> instructions = new List<Instruction>();

            IList<Token>[] data = tokens.Data;

            int requiredMemory = ProcessDataSection(tokens.DataStart, tokens.DataEnd, data, varMap);
            ProcessCodeSection(tokens.CodeStart, tokens.CodeEnd, tokens.Data, instructions, labelMap, procMap, varMap);

            return new Program(instructions, labelMap, procMap);
        }

        private static void ProcessCodeSection(int start, int end, IList<Token>[] lines, IList<Instruction> instructions, 
            Dictionary<string, uint> labelMap, Dictionary<string, uint> procMap, Dictionary<string, ushort> varMap)
        {
            uint executionLine = 0;
            for (int i = start; i < end; i++)
            {
                IList<Token> tokens = lines[i];
                Token first = tokens[0];

                if (first.Type == Token.TokenType.INST)
                {
                    ProcessInstructionLine(tokens, instructions, labelMap, procMap, varMap, executionLine);
                    executionLine++;
                }
                else if (first.Type == Token.TokenType.LBL)
                {
                    string data = first.Data;
                    labelMap.Add(data, executionLine);
                    if (tokens.Count > 1)
                    {
                        tokens.Remove(first);
                        ProcessInstructionLine(tokens, instructions, labelMap, procMap, varMap, executionLine);
                        executionLine++;
                    }
                }
            }
        }

        private static void ProcessInstructionLine(IList<Token> tokens, IList<Instruction> instructions,Dictionary<string, uint> labelMap, 
            Dictionary<string, uint> procMap, Dictionary<string, ushort> varMap, uint executionLine)
        {
            InstructionType inst = InstructionTypeExtension.fromString(tokens[0].Data);
            if (inst == InstructionType.NONE)
            {
                throw new CompilationException(tokens[0].Line, ErrorCode.Instance.GetMessage("MALFORMED_INSTRUCTION"));
            }
            if (inst == InstructionType.PROCSTART)
            {
                procMap.Add(tokens[1].Data, executionLine);
            }
            Parameter[] p = new Parameter[tokens.Count - 1];
            for (int i = 1; i < tokens.Count; i++)
            {
                ParameterType t = ParameterTypeExtension.GetTypeFromString(tokens[i].Data);
                switch(t)
                {
                    case ParameterType.REGISTER:
                        p[i - 1] = new RegisterParam(tokens[i].Data, tokens[i].Line);
                        break;
                    case ParameterType.POINTER:
                        p[i - 1] = new PointerParam(tokens[i].Data, varMap, tokens[i].Line);
                        break;
                    case ParameterType.NUMBER:
                        p[i - 1] = new NumberParam(tokens[i].Data, tokens[i].Line);
                        break;
                    case ParameterType.LABEL:
                        if (varMap.ContainsKey(tokens[i].Data))
                        {
                            p[i - 1] = new NumberParam(varMap[tokens[i].Data]); 
                        } else
                        {
                            p[i - 1] = new LabelParam(tokens[i].Data); 
                        }
                        break;
                    default:
                        throw new CompilationException(tokens[i].Line, ErrorCode.Instance.GetMessage("MALFORMED_PARAMETER"));
                }
            }
            CheckParameters(inst, p, tokens[0].Line);
            Instruction instruction = new Instruction(inst, p, executionLine);
            instructions.Add(instruction);
        }


        private static void CheckParameters(InstructionType i, Parameter[] p, int line)
        {
            int numParams = i.NumParams();
            IList<ParameterType[]> possibleParams = i.Params();
            if (p.Length != numParams)
            {
                throw new CompilationException(line, 
                    ErrorCode.Instance.GetMessageExpectedFound("WRONG_NUM_PARAMS", numParams, p.Length));
            }
            bool foundMatch = false;
            foreach (ParameterType[] expected in possibleParams) //TODO: simplify this...
            {
                bool isMatch = true;
                for (int j = 0; j < expected.Length; j++)
                {
                    if (expected[j] != p[j].GetParamType())
                    {
                        isMatch = false;
                    }
                }
                if (isMatch)
                {
                    foundMatch = true;
                    break;
                }
            }
            if (!foundMatch)
            {
                ParameterType[] pt = { ParameterType.NONE, ParameterType.NONE };

                for (int j = 0; j < p.Length; j++)
                {
                    pt[j] = p[j].GetParamType();
                }

                throw new CompilationException(line, ErrorCode.Instance.GetMessage("WRONG_PARAM_TYPE", pt[0], pt[1]));
            }
        }

        private static int ProcessDataSection(int start, int end, IList<Token>[] lines, 
            Dictionary<string, ushort> varMap)
        {
            int heapHead = 0;
            for (int i = start; i < end; i++)
            {
                IList<Token> tokens = lines[i];
                Token first = tokens[0];
                if (first.Type == Token.TokenType.ARRAY_DEC)
                {
                    heapHead = ProcessArrayDeclaration(tokens, varMap, heapHead);
                }
                if (first.Type == Token.TokenType.VAR_DEC)
                {
                    ProcessVarDeclaration(tokens, varMap);
                }
            }
            return heapHead;
        }

        private static int ProcessArrayDeclaration(IList<Token> tokens, Dictionary<string, ushort> arrayMap, int heapHead)
        {
            try
            {
                string data = tokens[0].Data;
                int bracketIndex = data.IndexOf('[');
                int lengthDistance = data.Length - bracketIndex;
                int length = int.Parse(data.Substring(bracketIndex + 1, lengthDistance - 2));
                string name = data.Substring(0, bracketIndex);
                arrayMap.Add(name, (ushort)heapHead);
                return heapHead + length;
            } catch (Exception e)
            {
                throw new CompilationException(tokens[0].Line, ErrorCode.Instance.GetMessage("MALFORMED_ARRAY_DEC"));
            }
        }

        private static void ProcessVarDeclaration(IList<Token> tokens, Dictionary<string, ushort> varMap)
        {
            try
            {
                string name = tokens[1].Data;
                ushort value = Utils.Parse(tokens[2].Data);
                varMap.Add(name, value);
            }
            catch (Exception e)
            {
                throw;// new CompilationException(tokens[0].Line, ErrorCode.Instance.GetMessage("MALFORMED_VAR_DEC"));
            }
        }


    }
}
