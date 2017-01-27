using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Emulator
{
    class Utils
    {
        private Utils() { }

        public static ushort Parse(string s)
        {
            if (s.ToLower().StartsWith("0x"))
            {
                return ushort.Parse(s.Substring(2, s.Length - 2), NumberStyles.AllowHexSpecifier);
            }
            return ushort.Parse(s);
        }
        
        public static bool TryParse(string s, out ushort value)
        {
            if (s.ToLower().StartsWith("0x"))
            {
                try
                {
                    value = ushort.Parse(s.Substring(2, s.Length - 2), NumberStyles.AllowHexSpecifier);
                    return true;
                } catch
                {
                    value = 0;
                    return false;
                }
            }
            return ushort.TryParse(s, out value);
        }
    }
}
