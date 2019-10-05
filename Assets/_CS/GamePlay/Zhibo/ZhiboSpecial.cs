using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZhiboSpecial : MonoBehaviour
{
    public string type;
    public int ClickNum = 0;

    ZhiboGameMode gameMode;

    private static float clickInterval = 0.1f;
    private float clickcd;

    private Transform ClickArea;
    public void Tick(float dTime)
    {
        if(clickcd > 0)
        {
            clickcd -= dTime;
        }
    }

    public void Init(string type, ZhiboGameMode gameMode)
    {
        this.type = type;
        ClickNum = 5;
        this.gameMode = gameMode;
        BindView();
        RegisterEvent();
        transform.localScale = Vector3.one;
    }

    public void BindView()
    {
        ClickArea = transform.GetChild(0);
    }

    public void RegisterEvent()
    {
        ClickEventListerner listern = GetComponent<ClickEventListerner>();

        if(listern == null)
        {
            listern = gameObject.AddComponent<ClickEventListerner>();
            listern.OnClickEvent += delegate {
                GetHit();
            };
        }
    }

    public void GetHit()
    {

        if (clickcd > 0)
        {
            return;
        }
        ClickNum -= 1;
        transform.localScale += new Vector3(0.05f,0.05f,0);
        if(ClickNum <= 0)
        {
            gameMode.HitSpecial(this);
        }
        clickcd = clickInterval;
    }



}
