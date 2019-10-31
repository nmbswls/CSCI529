using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWWTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameMain.GetInstance().GetModule<ResLoader>().LoadWWWResAsync<TextAsset>("http://www.baidu.com",(TextAsset t) => {

            });
        }
    }
}
