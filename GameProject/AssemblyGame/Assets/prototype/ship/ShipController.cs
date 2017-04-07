using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace prototype.ship
{
    public abstract class ShipController : MonoBehaviour
    {
        public float turnMultipiler;
        public float speedMultipler;

        public Rotator rotator;
        public int speed;

        protected TurretController[] turrets;
        protected GunController[] guns;

        void Start()
        {
            turrets = GetComponentsInChildren<TurretController>();
            guns = GetComponentsInChildren<GunController>();
            rotator = new Rotator(turnMultipiler, Vector3.forward);
        }

        void Update()
        {
            rotator.DoRotation(transform);
            DoUpdate();
            transform.position += transform.right * speed * speedMultipler;
        }

        protected abstract void DoUpdate();

        T GetComponentByID<T>(int id, T[] components) where T : ShipComponent
        {
            foreach (T comp in components)
            {
                if (comp.Id == id)
                {
                    return comp;
                }
            }
            return default(T);
        }
 
        protected void SetTurretRotation(int id, int direction, int speed)
        {
            TurretController turret;
            if ((turret = GetComponentByID(id, turrets)) != null)
            {
                Debug.Log(turret.id + ", " + turret.rotator);
                turret.SetRotation(direction, speed);
            } else
            {
                Debug.Log("couldn't find turret");
            }
        }

        protected void SetShipRotation(int direction, int speed)
        {
            rotator.SetRotation(direction, speed);
        }

        protected void SetShipSpeed(int speed)
        {
            this.speed = speed;
        }

        protected void FireGun(int id)
        {
            GunController gun;
            if ((gun = GetComponentByID(id, guns)) != null)
            {
                gun.FireGun();
            } else
            {
                //error
            }
        }

        protected void SetGunFireMode(int id, int mode)
        {
            GunController gun;
            if ((gun = GetComponentByID(id, guns)) != null)
            {
                gun.SetFireMode(mode);
            } else
            {
                //error
            }
        }

    }

}