using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emulator.Compiler;
using Emulator.IO;
using Emulator.Execute;

public class test : MonoBehaviour {

    public ExecutionUnit eu;
    public ushort ax;
    public int numSteps;
    public bool shouldStep = false;
	// Use this for initialization
	void Start () {
        string program = ProgramLoader.LoadProgram(Application.dataPath + "/Emulator/assembly.txt");
        Program p = new Program(program);
        eu = new ExecutionUnit(600);
        eu.SetProgram(p);  
	}
	
	// Update is called once per frame
	void Update () {
		if (shouldStep)
        {
            eu.Step();
            ax = eu.ReadRegister(0);
            shouldStep = false;
            numSteps++;
        }
	}
}
