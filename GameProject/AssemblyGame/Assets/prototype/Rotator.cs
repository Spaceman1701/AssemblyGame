using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace prototype
{
    public class Rotator
    {
        private readonly float multiplier;
        private int direction;
        private int speed;
        private Vector3 axis;
        public Rotator(float multiplier, Vector3 axis)
        {
            this.multiplier = multiplier;
            this.axis = axis;
        }

        public void SetRotation(int direction, int speed)
        {
            this.direction = direction;
            this.speed = speed;
        }

        public void DoRotation(Transform transform)
        {
            Vector3 rotationAxis;
            if (direction == 0)
            {
                rotationAxis = -axis;
            }
            else
            {
                rotationAxis = axis;
            }
            if (speed != 0)
            {
                transform.Rotate(rotationAxis, speed * multiplier);
            }
        }

        public float Multiplier
        {
            get
            {
                return multiplier;
            }
        }

        public int Direction
        {
            get
            {
                return direction;
            }

            set
            {
                direction = value;
            }
        }

        public int Speed
        {
            get
            {
                return speed;
            }

            set
            {
                speed = value;
            }
        }
    }
}
