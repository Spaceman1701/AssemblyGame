using Emulator.Compiler.CompileException;
using Emulator.Compiler.InstructionParameter;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Emulator.Compiler
{
    public class Program
    {
        IList<Instruction> code;
        IDictionary<string, uint> labelMap;
        IDictionary<string, uint> procMap;

        public Program(IList<Instruction> code, IDictionary<string, uint> labelMap, IDictionary<string, uint> procMap)
        {
            this.code = code;
            this.labelMap = labelMap;
            this.procMap = procMap;
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
                sb.AppendLine(i.ToString());
            }
            return sb.ToString();
        }



    }
}
