using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace prototype.ship
{
    public class GunController : MonoBehaviour, ShipComponent
    {

        public GameObject[] projectilePrefab;
        public int cooldownTime;
        public int fireMode;
        public int id;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        public int GetCooldownTime()
        {
            return cooldownTime;
        }

        public void FireGun()
        {
            GameObject projectile = projectilePrefab[fireMode];
            GameObject instance = Instantiate(projectile);
            IProjectileController pc = instance.GetComponentInChildren<IProjectileController>();
            pc.Fire(transform, 1);
        }

        public void SetFireMode(int mode)
        {
            fireMode = mode;
        }

        public int GetFireMode()
        {
            return fireMode;
        }

    }

}