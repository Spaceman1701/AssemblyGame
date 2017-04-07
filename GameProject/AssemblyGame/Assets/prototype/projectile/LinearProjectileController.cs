using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace prototype.projectile
{
    public class LinearProjectileController : MonoBehaviour, IProjectileController
    {
        public float speed = 10;
        public float duration = 10;

        public void Fire(Transform initalTransform, float initialSpeed)
        {
            this.transform.position = initalTransform.position;
            transform.rotation = initalTransform.rotation;
            this.speed = initialSpeed;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.position += transform.up * speed * Time.deltaTime;
            duration -= 1 * Time.deltaTime;

            if (duration <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}