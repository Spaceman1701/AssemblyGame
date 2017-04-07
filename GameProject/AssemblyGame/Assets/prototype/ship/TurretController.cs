using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace prototype.ship
{
    public class TurretController : MonoBehaviour, ShipComponent
    {

        public int id;
        public float multiplier;

        public Rotator rotator;

        void Start()
        {
            rotator = new Rotator(multiplier, Vector3.forward);
            Debug.Log(id + " created rotator");
        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        void Update()
        {
            Debug.Log(id + " update " + rotator);
            rotator.DoRotation(transform);
        }

        public void SetRotation(int dir, int speed)
        {
            if (rotator == null)
            {
                Debug.Log(id + " has null rotator");
            }
            rotator.SetRotation(dir, speed);
        }
    }

}