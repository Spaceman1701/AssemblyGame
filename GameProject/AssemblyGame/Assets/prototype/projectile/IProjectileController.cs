using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileController {

    void Fire(Transform initalTransform, float initialSpeed);
}
