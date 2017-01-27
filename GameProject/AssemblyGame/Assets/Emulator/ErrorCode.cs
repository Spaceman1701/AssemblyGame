using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Emulator
{
    class ErrorCode
    {
        private Dictionary<string, string> messages;
        private const string ERROR_STRINGS_PATH = @"/Emulator/errorstrings.txt";
        private const string DEFUALT_ERROR = "NO ERROR MESSAGE FOUND!";

        private static ErrorCode INSTANCE;

        private ErrorCode()
        {
            messages = new Dictionary<string, string>();

            string errorStringsLocation = Application.dataPath + ERROR_STRINGS_PATH;

            using (FileStream f = File.OpenRead(errorStringsLocation))
            {
                using (StreamReader s = new StreamReader(f))
                {
                    string line = null;
                    while ((line = s.ReadLine()) != null)
                    {
                        if (line.Trim() != "")
                        {
                            string[] splt = line.Split('=');
                            string message = splt[1].Trim();
                            messages[splt[0].Trim().ToLower()] = message.Substring(1, message.Length - 2);
                        }
                    }
                }
            }
        }


        public string GetMessage(string code)
        {
            if (messages.ContainsKey(code.ToLower()))
            {
                return messages[code.ToLower()];
            }
            return DEFUALT_ERROR;
        }

        public string GetMessageExpectedFound(string code, System.Object expected, System.Object found)
        {
            return GetMessage(code) + " Expected: " + expected.ToString() + ". Found: " + found.ToString() + ".";
        }

        public string GetMessage(string code, params System.Object[] info) //TODO: should use string builder, though not really high perf code
        {
            string message = GetMessage(code) + " " + info[0].ToString();
            for (int i = 1; i < info.Length; i++)
            {
                message += ", " + info[i].ToString();
            }
            return message;
        }

        public static ErrorCode Instance
        {
            get
            {
                if (INSTANCE == null)
                {
                    INSTANCE = new ErrorCode();
                }
                return INSTANCE;
            }
        }
    }
}
