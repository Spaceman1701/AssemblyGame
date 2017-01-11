using OldCPUEmulator.Compiler;
using OldCPUEmulator.Execute;
using OldCPUEmulator.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OldCPUEmulator
{
    class App
    {
        static void Main(string[] args)
        {
            string sProgram =
                ".Data" + '\n' +
                "HEAP[100]" + '\n' +
                ".Code" + '\n' +
                "procstart main" + '\n' +
                "start: mov ax, 9 #this is an inline comment" + '\n' +
                "cmp ax, 9" + '\n' +
                "jeq lbl" + '\n' +
                "end: xor ax, ax" + '\n' +
                "lbl: cmp ax, 9" + '\n' +
                "jeq end" + '\n' + 
                "procend main";
            ExcutionUnit eu = new ExcutionUnit(256);
            ConsoleIO io = new ConsoleIO(eu);
            io.Run();
        }
    }
}
