using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldCPUEmulator.IO
{
    class ProgramLoader
    {
        private ProgramLoader()
        {

        }

        public static string LoadProgram(string fileLocation)
        {
            using (FileStream f = File.OpenRead(fileLocation))
            {
                using (StreamReader s = new StreamReader(f))
                {
                    return s.ReadToEnd();
                }
            }
        }
    }
}
