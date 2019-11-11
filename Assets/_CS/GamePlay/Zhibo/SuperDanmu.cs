using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SuperDanmuView
{
    public Text Content;
    public Image Icon;
    public Image Hengfu;
}
public class SuperDanmu : MonoBehaviour
{


    public SuperDanmuView view = new SuperDanmuView();
    ZhiboGameMode gameMode;

    public RectTransform rect;
    public Animator anim;

    public bool Activated = false;

    [HideInInspector]
    public bool NeedDestroy = false;

    public bool HasDisapeared = false;

    bool destroying = false;

    bool statusEffectOn = false;

    string txt;

    int HpLeft = 0;

    int hengfuSize = 0;
    public void init(string txt, string type, ZhiboGameMode gameMode)
    {
        this.gameMode = gameMode;
        this.txt = txt;
        rect = (RectTransform)transform;
        anim = GetComponent<Animator>();

        anim.Play("FadeIn");
        anim.ResetTrigger("anim");
        Activated = false;

        HasDisapeared = false;
        NeedDestroy = false;

        destroying = false;

        HpLeft = 8;

        hengfuSize = txt.Length * 20 + 10;

        BindView();
        RegisterEvent();
        view.Content.text = txt;
        AdjustWidth();
    }
    private void BindView()
    {
        view.Content = transform.Find("Tiaofu").Find("Content").GetComponent<Text>();
        view.Icon = transform.Find("Icon").GetComponent<Image>();
        view.Hengfu = transform.Find("Tiaofu").GetComponent<Image>();
    }

    private void RegisterEvent()
    {
        ClickEventListerner listener = view.Icon.gameObject.GetComponent<DragEventListener>();
        if (listener == null)
        {
            listener = view.Icon.gameObject.AddComponent<DragEventListener>();
        }
        listener.ClearClickEvent();
        listener.OnClickEvent += delegate (PointerEventData eventData) {
            Clicked();
        };
    }

    public void Clicked()
    {
        if (destroying) return;

        if (HpLeft > 0)
        {
            HpLeft -= 1;
            gameMode.mUICtrl.ShowHitEffect(transform.position);
            if (HpLeft <= 0)
            {
                Disappear();
                gameMode.GainStatus(6f);
            }
        }
    }

    public void Tick(float dTime)
    {
        if (!Activated || HasDisapeared)
        {
            return;
        }
        if (!statusEffectOn) {
            StatusEffect();
            statusEffectOn = true;
        }
        rect.anchoredPosition += Vector2.left * gameMode.state.DanmuSpd * dTime;
        if (rect.anchoredPosition.x < -100)
        {
            NeedDestroy = true;
        }
    }

    public void AdjustWidth()
    {
        view.Hengfu.rectTransform.sizeDelta = new Vector2(hengfuSize, view.Hengfu.rectTransform.sizeDelta.y);
        view.Hengfu.rectTransform.SetAsLastSibling();
    }

    public void StatusEffect()
    {
        gameMode.GainStatus(-5f);
    }

    public void Disappear()
    {
        HasDisapeared = true;
        destroying = true;
        anim.SetTrigger("Disappear");
        Invoke("DelayDestroy", 0.5f);
    }

    public void DelayDestroy()
    {
        gameMode.DestroySuperDanmu(this);
    }
}
