﻿using OldCPUEmulator.Compiler.InstructionParameter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler
{
    class Program
    {
        IList<Instruction> code;
        IDictionary<string, uint> labelMap;
        IDictionary<string, uint> varMap;
        IDictionary<string, uint> procMap;

        public Program(string rawProgram)
        {
            if (rawProgram[rawProgram.Length - 1] != '\n')
            {
                rawProgram += '\n'; //append a newline to eof
            }

            labelMap = new Dictionary<string, uint>();
            procMap = new Dictionary<string, uint>();
            

            string[] program = Preprocess(rawProgram);
            code = new List<Instruction>();
            uint instNum = 0;
            bool codeSection = false;
            for (uint i = 0; i < program.Length; i++)
            {
                string line = program[i];
                if (!line.Trim().Equals(""))
                {
                    if (line.StartsWith(".Code"))
                    {
                        codeSection = true;
                        continue;
                    } else if (line.StartsWith(".Data"))
                    {
                        codeSection = false;
                        continue;
                    }
                    if (codeSection)
                    {
                        code.Add(ParseLine(instNum++, line));
                    }
                }
            }

            

        }

        private string[] Preprocess(string rawProgram)
        {
            varMap = new Dictionary<string, uint>();

            string[] program = rawProgram.Split('\n');
            bool codeSection = false;
            uint varStart = 0;
            uint skiplines = 0;
            for (uint line = 0; line < program.Length; line++)
            {
                string pLine = program[line].Trim();
                int commentIndex = pLine.IndexOf("#");
                if (commentIndex != -1)
                {
                    pLine = pLine.Substring(0, commentIndex);
                }
                if (!pLine.Equals("") && !pLine.StartsWith("#"))
                {
                    if (pLine.StartsWith(".Data"))
                    {
                        codeSection = false;
                        skiplines++;
                        continue;
                    } else if (pLine.StartsWith(".Code"))
                    {
                        codeSection = true;
                        skiplines++;
                        continue;
                    }
                    if (!codeSection)
                    {
                        int sizeStart = pLine.IndexOf('[');
                        int sizeEnd = pLine.IndexOf(']');
                        string sizeStr = pLine.Substring(sizeStart + 1, sizeEnd - sizeStart - 1);
                        uint size = uint.Parse(sizeStr);
                        string name = pLine.Substring(0, sizeStart).Trim();
                        varMap[name] = varStart;
                        varStart += size;
                        skiplines++;
                    } else
                    {
                        foreach (string varName in varMap.Keys)
                        {
                            pLine = pLine.Replace(" " + varName + "[", " " + varMap[varName].ToString() + "[");
                        }
                        int labelSep = pLine.IndexOf(':');
                        if (labelSep != -1)
                        {
                            string label = pLine.Substring(0, labelSep);
                            labelMap[label] = line - skiplines;
                            pLine = pLine.Substring(labelSep + 1, pLine.Length - 1 - labelSep);
                            if (pLine.Length <= labelSep + 1 || pLine.Substring(labelSep + 1, pLine.Length - labelSep).Trim() == "")
                            {
                                skiplines++;
                            }
                        }
                        if (pLine.StartsWith("procstart"))
                        {
                            string lbl = pLine.Split(' ')[1];
                            procMap[lbl] = line - skiplines;
                        }
                    }
                } else
                {
                    skiplines++;
                }
                program[line] = pLine;
            }
            return program;
        }

        private Instruction ParseLine(uint lineNum, string line) //TODO: values
        {
            string[] components = line.Trim().Replace(", ", ",").Split(' ');
            InstructionType inst = InstructionFromString(components[0]);
            if (inst == InstructionType.NONE)
            {
                return new MalformedInstruction("Could not parse instruction");
            }
            int expectedParamNum = inst.NumParams();
            int numParams = 0;
            if (components.Length == 1)
            {
                if (expectedParamNum != 0)
                {
                    return new MalformedInstruction("expected parameters");
                }
                return new CompleteInstruction(inst, new Parameter[] { }, lineNum);
            }
            string[] paramStrings = components[1].Split(',');
            numParams = paramStrings.Length;
            if (numParams != expectedParamNum)
            {
                return new MalformedInstruction("incorrect number of parameters: expected " + expectedParamNum + ", found: " + numParams);
            }
            Parameter[] parameters = new Parameter[numParams];
            for (int i = 0; i < paramStrings.Length; i++)
            {
                ParameterType type = paramFromString(paramStrings[i]);
                Parameter p;
                if (type == ParameterType.NONE)
                {
                    return new MalformedInstruction("malformed parameter");
                }
                if (type == ParameterType.REGISTER)
                {
                    p = new RegisterParam(paramStrings[i]);
                } else if (type == ParameterType.NUMBER)
                {
                    p = new NumberParam(paramStrings[i]);
                } else if (type == ParameterType.POINTER)
                {
                    p = new PointerParam(paramStrings[i]);
                } else
                {
                    p = new LabelParam(paramStrings[i]);
                }
                parameters[i] = p;
            }
            return new CompleteInstruction(inst, parameters, lineNum);
        }


        public uint TranslateLabel(LabelParam p)
        {
            return labelMap[p.Lbl];
        }

        public uint GetEntryLine()
        {
            return procMap["main"];
        }

        public uint TranslateCall(LabelParam p)
        {
            return procMap[p.Lbl];
        }

        public Instruction GetInstruction(uint line)
        {
            return code[(int)line];
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Procedures:");
            foreach (string proc in procMap.Keys)
            {
                sb.AppendLine(proc + ":" + procMap[proc]);
            }
            sb.AppendLine("Labels:");
            foreach (string label in labelMap.Keys)
            {
                sb.AppendLine(label + ":" + labelMap[label]);
            }
            sb.AppendLine("---------------");

            foreach (Instruction i in code)
            {
                if (i == null)
                {
                    //continue;
                }
                sb.AppendLine(i.ToString());
            }
            return sb.ToString();
        }


        private static ParameterType paramFromString(string s)
        {
            foreach (ParameterType p in Enum.GetValues(typeof(ParameterType)))
            {
                if (p.isType(s))
                {
                    return p;
                }
            }
            return ParameterType.NONE;
        }

        public static InstructionType InstructionFromString(string s)
        {
            foreach (InstructionType t in Enum.GetValues(typeof(InstructionType)))
            {
                if (t.ToString().ToLower().Equals(s.ToLower()))
                {
                    return t;
                }
            }
            return InstructionType.NONE;
        }



    }
}