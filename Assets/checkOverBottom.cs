using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class checkOverBottom : MonoBehaviour
{
    //public Scrollbar sb;
    public ScrollRect sr;
    // Start is called before the first frame update
    void Start()
    {
        //sb = this.GetComponent<Scrollbar>();
        sr = this.GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(sb.value);
        Debug.Log(sr.verticalNormalizedPosition);
    }
}
