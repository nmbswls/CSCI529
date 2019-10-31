using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class ZhiboSpecial : MonoBehaviour
{
    public string type;
    public int ClickNum = 0;
    public int MaxHp = 5;

    ZhiboGameMode gameMode;

    private static float clickInterval = 0.1f;
    private float clickcd;

    private Transform ClickArea;

    float timer = 0;
    float basicScaleRate = 1f;
    float clickScaleRate = 1f;
    private static float ScaleInterval = 2f;

    public void Tick(float dTime)
    {
        if(clickcd > 0)
        {
            clickcd -= dTime;
        }
        timer += dTime;
        GetBasicRate();
        transform.localScale = Vector3.one * basicScaleRate * clickScaleRate;
    }

    private void GetBasicRate()
    {
        float a = Mathf.Abs(1 - (timer - (int)(timer / ScaleInterval) * ScaleInterval) / ScaleInterval * 2);
        basicScaleRate = (a * 0.3f) + 1f;
    }

    public void Init(string type, ZhiboGameMode gameMode)
    {
        this.type = type;
        ClickNum = 5;
        this.gameMode = gameMode;
        BindView();
        RegisterEvent();
        basicScaleRate = 1f;
        timer = 0;
        preTween = null;

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

    private Tween preTween;
    private void TweenChangeSize()
    {
        if(preTween != null)
        {
            preTween.Kill();
        }
        Tween tween = DOTween.To
                (
                    () => clickScaleRate,
                    (x) => { clickScaleRate = x; },
                    1+(MaxHp- ClickNum)*0.1f,
                    0.2f
                );
        preTween = tween;
    }

    public void GetHit()
    {

        if (clickcd > 0)
        {
            return;
        }
        ClickNum -= 1;

        TweenChangeSize();
        if(ClickNum <= 0)
        {
            gameMode.HitSpecial(this);
        }
        clickcd = clickInterval;
    }



}
