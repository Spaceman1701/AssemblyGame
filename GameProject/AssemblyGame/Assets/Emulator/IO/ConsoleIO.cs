using Emulator.Compiler;
using Emulator.Execute;
using System;
using System.Collections.Generic;


namespace Emulator.IO
{
    class ConsoleIO
    {
        private ExecutionUnit eu;

        private IDictionary<string, Program> programs;

        public ConsoleIO(ExecutionUnit eu)
        {
            this.eu = eu;
            programs = new Dictionary<string, Program>();

            Console.WriteLine("Console IO loaded for Execution Unit with " + eu.MemorySize + " Words of Memory");
        }

        public void Run()
        {
            bool shouldExit = false;

            while (!shouldExit)
            {
                string input = Console.ReadLine();
                try
                {
                    if (input.StartsWith("load"))
                    {

                        string[] command = input.Split(' ');
                        LoadProgram(command[1], command[2]);
                    }

                    if (input.StartsWith("delete"))
                    {
                        DeleteProgram(input.Split(' ')[1]);
                    }

                    if (input == "list")
                    {
                        foreach (string program in programs.Keys)
                        {
                            Console.WriteLine(program);
                        }
                    }

                    if (input.StartsWith("set"))
                    {
                        eu.SetProgram(programs[input.Split(' ')[1]]);
                    }

                    if (input == "step")
                    {
                        if (!eu.IsRunningProgram())
                        {
                            Console.WriteLine("no program set");
                        }
                        eu.Step();
                    }

                    if (input.StartsWith("reg"))
                    {
                        string[] command = input.Split(' ');
                        int reg = ExecutionUnit.GetRegisterIndex(command[1].ToLower());
                        Console.WriteLine("Register " + command[1] + " = " + eu.ReadRegister(reg));
                    }

                    if (input.StartsWith("mem"))
                    {
                        string[] command = input.Split(' ');
                        int ptr = int.Parse(command[1]);
                        Console.WriteLine("Memory " + ptr + " = " + eu.ReadMemory(ptr));
                    }

                    if (input == "line")
                    {
                        Console.WriteLine("EU current line:" + eu.CurrentLine);
                    }

                    if (input == "exit")
                    {
                        Console.WriteLine("exiting");
                        break;
                    }
                } catch (IndexOutOfRangeException e)
                {
                    Console.WriteLine("invalid syntax");
                }

            }
            Console.ReadLine();
        }

        private void LoadProgram(string name, string fileLocation)
        {
            Program p = TokenizedProgramCompiler.Compile(new TokenizedProgram(Preprocessor.Process(ProgramLoader.LoadProgram(fileLocation))));
            programs.Add(name, p);
        }

        private void DeleteProgram(string name)
        {
            programs.Remove(name);
        }
    }
}
