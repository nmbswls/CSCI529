using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WildExploreCtrl : MonoBehaviour
{
    public FieldMovableCamera fieldCamera;
    public WildMap wildMap;

    public WildExploreUICtrl UICtrl;

    // Start is called before the first frame update
    void Start()
    {
        Rect activeRect = new Rect(wildMap.transform.position, new Vector2(wildMap.Width/ wildMap.meterPerUnit, wildMap.Height/ wildMap.meterPerUnit));
        fieldCamera.Init(Camera.main, activeRect);
        fieldCamera.MoveTo(wildMap.player.transform.position);

        wildMap.mainCtrl = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowPop(Vector3 pos)
    {
        UICtrl.ShowPopup(pos);
    }
    public void HidePop()
    {
        UICtrl.HidePopup();
    }
}
