using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WildExploreCtrl : MonoBehaviour
{
    public FieldMovableCamera fieldCamera;
    public WildMap wildMap;

    public WildExploreUICtrl UICtrl;

    public float GameTimeRate = 1;

    public float gameTime;
    private int pauseCount;

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
        float gameDeltaTime = Time.deltaTime * GameTimeRate;
        gameTime += gameDeltaTime;
        UICtrl.UpdateTimer();

        if (Input.GetKeyDown(KeyCode.K))
        {
            Pause();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            UnPause();
        }
    }

    public void Pause()
    {
        pauseCount += 1;
    }

    public void UnPause()
    {
        if(pauseCount > 0)
        {
            pauseCount -= 1;
        }
    }

    public void ShowPop(Vector3 pos)
    {

        UICtrl.ShowPopup(pos);
        Pause();
    }
    public void HidePop()
    {
        UICtrl.HidePopup();
        UnPause();
    }

}
