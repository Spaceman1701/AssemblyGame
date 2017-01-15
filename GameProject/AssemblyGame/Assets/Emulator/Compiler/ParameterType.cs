using System.Linq;
using System.Text.RegularExpressions;

namespace Emulator.Compiler
{
    public enum ParameterType
    {
        REGISTER, NUMBER, POINTER, LABEL, NONE
    }

    public static class ParameterExtension
    {
        private static Regex pointerRegex = new Regex(@"\[(.*?)\]");
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
            return pointerRegex.IsMatch(pString);
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
