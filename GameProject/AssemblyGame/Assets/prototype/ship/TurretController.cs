using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace prototype.ship
{
    public class TurretController : MonoBehaviour, ShipComponent
    {

        public int id;
        public float multiplier;

        Rotator rotator;

        void Start()
        {
            rotator = new Rotator(multiplier, Vector3.up);
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
            rotator.DoRotation(transform);
        }

        public void SetRotation(int dir, int speed)
        {
            rotator.SetRotation(dir, speed);
        }
    }

}