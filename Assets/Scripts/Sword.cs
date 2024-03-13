using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Sword : MonoBehaviour
{
    MeshRenderer mesh;

    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (GameManager.instance.clear)
        {
            mesh.enabled = false;

            return;
        }

        if (PlayerController.instance.attacking && !mesh.enabled && !PlayerController.instance.hit && !PlayerController.instance.die)
        {
            mesh.enabled = true;
        }

        if ((PlayerController.instance.lastAttack > PlayerController.instance.attackInit && mesh.enabled) || PlayerController.instance.hit || PlayerController.instance.die)
        {
            mesh.enabled = false;
        }
    }
}
