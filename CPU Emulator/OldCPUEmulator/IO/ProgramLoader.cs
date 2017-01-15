
using System.IO;


namespace OldCPUEmulator.IO
{
    public class ProgramLoader
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
