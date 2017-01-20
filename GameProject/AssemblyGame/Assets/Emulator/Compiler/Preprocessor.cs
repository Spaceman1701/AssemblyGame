using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emulator.Compiler
{
    class Preprocessor
    {

        private static string[] MATH_SYM = {"+", "-"};

        private Preprocessor()
        {

        }


        public static string[] Process(string program)
        {
            string[] lines = program.Split('\n');
            RemoveComments(lines);
            FormatParameters(lines);
            FromatMath(lines);
            TrimLines(lines);
            InsertTokenSeps(lines);
            return lines;
        }


        private static void RemoveComments(string[] program)
        {
            for (int i = 0; i < program.Length; i++)
            {
                string line = program[i];
                int commentIndex = line.IndexOf('#');
                if (commentIndex != -1)
                {
                    line = line.Substring(0, commentIndex);
                }
                program[i] = line;
            }
        }

        private static void FormatParameters(string[] program)
        {
            for (int i = 0; i < program.Length; i++)
            {
                string line = program[i];
                line = line.Replace(", ", ",").Replace(" ,", ",");
                program[i] = line;
            }
        }

        private static void FromatMath(string[] program)
        {
            for (int i = 0; i < program.Length; i++)
            {
                foreach (string c in MATH_SYM)
                {
                    program[i] = program[i].Replace(" " + c, c).Replace(c + " ", c);
                }
            }
        }

        private static void TrimLines(string[] program)
        {
            for (int i = 0; i < program.Length; i++)
            {
                program[i] = program[i].Trim();
            }
        }

        private static void InsertTokenSeps(string[] program)
        {
            char sep = TokenizedProgram.TOKEN_SEP;
            for (int i = 0; i < program.Length; i++)
            {
                program[i] = program[i].Replace(':', sep).Replace(' ', sep).Replace(',', sep);
            }
        }
    }
}
