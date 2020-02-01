using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildExploreCtrl : MonoBehaviour
{
    public FieldMovableCamera fieldCamera;
    public WildMap wildMap;
    // Start is called before the first frame update
    void Start()
    {
        Rect activeRect = new Rect(wildMap.transform.position, new Vector2(wildMap.Width/ wildMap.meterPerUnit, wildMap.Height/ wildMap.meterPerUnit));
        fieldCamera.Init(Camera.main, activeRect);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
