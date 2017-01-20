using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Emulator.Compiler;
using Emulator.IO;
using Emulator.Execute;

public class TestNewCompile : MonoBehaviour {

    public bool step;
    ExecutionUnit eu;
	// Use this for initialization
	void Start () {
        string programString = ProgramLoader.LoadProgram(Application.dataPath + "/Emulator/test.txt");

        TokenizedProgram t = new TokenizedProgram(Preprocessor.Process(programString));
        Program p = TokenizedProgramCompiler.Compile(t);
        eu = new ExecutionUnit(1000);
        eu.SetProgram(p);

    }

    // Update is called once per frame
    void Update () {
		if (step)
        {
            eu.Step();
            Debug.Log("ax: " + eu.ReadRegister(0).Data);
            Debug.Log("MEM 2: " + eu.ReadMemory(2).Data);
            step = false;
        }
    }
}
