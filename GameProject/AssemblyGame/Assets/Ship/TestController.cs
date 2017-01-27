using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emulator.Execute;
using Emulator.Compiler;
using Emulator.IO;

public class TestController : MonoBehaviour {

    public TurretMover mover;
    public ExecutionUnit eu;
    public int left;
    public int right;
	// Use this for initialization
	void Start () {
        string program = ProgramLoader.LoadProgram(Application.dataPath + "/Emulator/assembly.txt");
        Program p = TokenizedProgramCompiler.Compile(new TokenizedProgram(Preprocessor.Process(program)));
        eu = new ExecutionUnit(600);
        eu.RegisterInterrupt(0, MoveInterrupt);
        eu.RegisterInterrupt(1, KeyboardInterupt);
        eu.SetProgram(p);
        eu.Step(); //procstart main
    }
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < 100; i++)
        {
            eu.Step();
        }
        left = eu.ReadMemory(0).Data;
        right = eu.ReadMemory(1).Data;
	}

    public void MoveInterrupt(ExecutionUnit eu)
    {
        int dir = eu.ReadRegister(0).Data;
        int speed = eu.ReadRegister(1).Data;
        mover.SetRotation(dir, speed);
    }

    public void KeyboardInterupt(ExecutionUnit eu)
    {
        int leftPtr = eu.ReadRegister(0).Data;
        int rightPtr = eu.ReadRegister(1).Data;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            eu.ReadMemory(leftPtr).Data = 1;
        } else
        {
            eu.ReadMemory(leftPtr).Data = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            eu.ReadMemory(rightPtr).Data = 1;
        }
        else
        {
            eu.ReadMemory(rightPtr).Data = 0;
        }
    }
}
