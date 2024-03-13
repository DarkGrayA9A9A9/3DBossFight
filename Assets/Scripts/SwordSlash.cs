using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    ParticleSystem particle;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        particle.enableEmission = false;
    }

    void Update()
    {
        if (GameManager.instance.clear)
        {
            particle.enableEmission = false;

            return;
        }

        if (PlayerController.instance.attacking && PlayerController.instance.lastAttack > 0.3f && !particle.enableEmission)
        {
            particle.Play();
            particle.enableEmission = true;

            if (PlayerController.instance.attackCombo)
                transform.localRotation = Quaternion.Euler(220, -70, 0);
            else
                transform.localRotation = Quaternion.Euler(30, -70, 0);
        }

        if (!PlayerController.instance.attacking && particle.enableEmission)
        {
            particle.Stop();
            particle.enableEmission = false;
        }
    }
}
