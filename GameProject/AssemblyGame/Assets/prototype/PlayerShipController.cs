using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace prototype
{
    public class PlayerShipController : ship.ShipController
    {
        public int acceleration = 1;
        public int rotationSpeed;
        public int currentGun;

        protected override void DoStart()
        {
            health = 100;
        }

        public void SelectGun()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currentGun = 0;
            } else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currentGun = 1;
            } else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                currentGun = 2;
            } else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                currentGun = 3;
            } else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                currentGun = 4;
            }
        }

        void RotateTurret()
        {
            if (currentGun != 0)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    SetTurretRotation(currentGun, 1, 100);
                } else if (Input.GetKey(KeyCode.RightArrow))
                {
                    SetTurretRotation(currentGun, 0, 100);
                } else
                {
                    SetTurretRotation(currentGun, 0, 0);
                }
            }
        }

        protected override void DoUpdate()
        {
            RotateTurret();
            SelectGun();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FireGun(currentGun);
            }
            /**
             * Speed control 
             **/
            if (Input.GetKey(KeyCode.W))
            {
                SetShipSpeed(speed - acceleration);
            } else if (Input.GetKey(KeyCode.S))
            {
                SetShipSpeed(speed + acceleration);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                SetShipSpeed(0);
            }

            /**
             * Rotation control
             **/
            if (Input.GetKey(KeyCode.A))
            {
                SetShipRotation(1, rotationSpeed);
            } else if (Input.GetKey(KeyCode.D))
            {
                SetShipRotation(0, rotationSpeed);
            } else
            {
                SetShipRotation(0, 0);
            }
        }
    }
}
