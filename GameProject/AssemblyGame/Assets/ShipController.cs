using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emulator.Execute;
using Emulator.Compiler;
using Emulator.IO;

public class ShipController : MonoBehaviour {


    Dictionary<int, GunController> gunMap;

    public ExecutionUnit eu;

    // Use this for initialization
    void Awake()
    {
        string program = ProgramLoader.LoadProgram(Application.dataPath + "/Emulator/testship.txt");
        Program p = TokenizedProgramCompiler.Compile(new TokenizedProgram(Preprocessor.Process(program)));
        eu = new ExecutionUnit(1024);
        gunMap = new Dictionary<int, GunController>();
        eu.Step(); //step into main function
    }
    void Start () {
        eu.RegisterInterrupt(0x1, PingSensors);
        eu.RegisterInterrupt(0x2, RotateShip);
        eu.RegisterInterrupt(0x3, MoveShip);
        eu.RegisterInterrupt(0x4, FireGun);
        eu.RegisterInterrupt(0x5, RotateGun);
        eu.RegisterInterrupt(0x6, FireGun);
        eu.RegisterInterrupt(0x7, KeyboardInput);
	}
	
	// Update is called once per frame
	void Update () {
        eu.RunForCycles(2000);
	}

    void KeyboardInput(ExecutionUnit eu)
    {

    }

    void PingSensors(ExecutionUnit eu)
    {

    }

    void RotateShip(ExecutionUnit eu)
    {

    }

    void MoveShip(ExecutionUnit eu)
    {

    }

    void FireGun(ExecutionUnit eu)
    {

    }

    void RotateGun(ExecutionUnit eu)
    {
        ushort ax = eu.ReadRegister(0).Data;
        ushort direction = eu.ReadRegister(1).Data;
        ushort speed = eu.ReadRegister(2).Data;
        GunController[] gcs = GetComponentsInChildren<GunController>();
        foreach (GunController gc in gcs)
        {
            if (ax == gc.id)
            {

            }
        }
    }
}
