using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    BoxCollider collider;
    // Start is called before the first frame update
    void Awake()
    {
        collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.instance.attacking && PlayerController.instance.lastAttack > 0.3f && PlayerController.instance.lastAttack < 0.5f)
        {
            collider.enabled = true;
        }
        else
        {
            collider.enabled = false;
        }
    }
}
