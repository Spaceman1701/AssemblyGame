using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OldCPUEmulator.Compiler
{
    public enum ParameterType
    {
        REGISTER, NUMBER, POINTER, LABEL, NONE
    }

    public static class ParameterExtension
    {
        private static Regex pointerRegex = new Regex(@"\[(.+)\]");
        private static string[] regNames = { "ax", "bx", "cx", "dx", "sp", "bp", "si", "di"};
        public static bool isType(this ParameterType t, string pString)
        {
            switch (t)
            {
                case ParameterType.REGISTER:
                    return isReg(pString);
                case ParameterType.NUMBER:
                    return isNumber(pString);
                case ParameterType.LABEL:
                    return isLabal(pString);
                case ParameterType.POINTER:
                    return isPointer(pString);
                default:
                    return pString.Length == 0; //is NONE
            }
        }

        private static bool isReg(string pString)
        {
            return regNames.Contains<string>(pString);
        }

        private static bool isNumber(string pString)
        {
            ushort n;
            return ushort.TryParse(pString, out n);
        }

        private static bool isPointer(string pString)
        {
            if (pointerRegex.IsMatch(pString))
            {
                string p = pointerRegex.Match(pString).Groups[0].ToString().Trim();
                ushort n;
                if (ushort.TryParse(p.Substring(1, p.Length - 2), out n))
                {
                    return true;
                }
                if (isReg(p))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool isLabal(string pString) //improve later
        {
            if (pString.Any(char.IsDigit) | pString.Any(c => c == ' '))
            {
                return false;
            }
            return true;
        }
    }
}
