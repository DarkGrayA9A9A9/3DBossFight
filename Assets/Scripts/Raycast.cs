using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    public bool raycasting;

    RaycastHit hit;

    public static Raycast instance;

    void Awake()
    {
        if (Raycast.instance == null)
            Raycast.instance = this;
    }

    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down, new Color(1, 0, 0));

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.2f, LayerMask.GetMask("Floor")))
        {
            raycasting = true;
        }
        else
        {
            raycasting = false;
        }
    }
}
