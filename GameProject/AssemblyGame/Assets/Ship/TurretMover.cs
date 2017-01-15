using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMover : MonoBehaviour {

    public int direction;
    public int speed;
    public float modifier;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 axis;
        if (direction == 0)
        {
            axis = Vector3.back;
        } else
        {
            axis = Vector3.forward;
        }
        if (speed != 0)
        {
            transform.Rotate(axis, speed / modifier);
        }
    }

    public void SetRotation(int dir, int speed)
    {
        this.direction = dir;
        this.speed = speed;
    }
}
