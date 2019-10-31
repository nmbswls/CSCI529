using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTest : MonoBehaviour
{
    public GameObject gggg;
    // Start is called before the first frame update
    void Start()
    {
        gggg.transform.GetChild(0).position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            gggg.transform.position -= Vector3.left * 10;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            gggg.transform.position -= Vector3.right * 10;
        }
    }
}
