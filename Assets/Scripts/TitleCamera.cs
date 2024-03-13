using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleCamera: MonoBehaviour
{
    public float rot;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rot += speed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(20, rot, 0);
    }
}
