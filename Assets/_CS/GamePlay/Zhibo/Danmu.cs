using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DanmuView
{
    public Text Content;
    public Image Hengfu;
    public Image BadBG;
}

public enum ENM_DanmuType
{
    NORMAL,
    RARE,
    BAD
}

public class Danmu : MonoBehaviour
{



    [HideInInspector]
    public int left = 1;

    [HideInInspector]
    public int strength;

    [HideInInspector]
    public Color color;

    [HideInInspector]
    public ENM_DanmuType danmuType = ENM_DanmuType.NORMAL;

    [HideInInspector]
    public bool NeedDestroy = false;

    [HideInInspector]
    public bool isBad = false;

    [HideInInspector]
    public bool isBig = false;


    bool destroying = false;

    public RectTransform rect;
    public Animator anim;

    public DanmuView view = new DanmuView();

    ZhiboGameMode gameMode;

    public virtual void init(string txt, bool isBad, ZhiboGameMode gameMode)
    {
        rect = (RectTransform)transform;
        this.isBad = isBad;
        this.gameMode = gameMode;
        isBig = false;

        anim = GetComponent<Animator>();
        NeedDestroy = false;
        //spd = gameMode.state.DanmuSpd;
        strength = 1;
        left = 1;


        //color = getRandomColor();
        BindView();
        RegisterEvent();

        anim.Play("Normal");
        destroying = false;

        if (isBad)
        {
            view.Content.color = Color.white;
            view.BadBG.gameObject.SetActive(true);
        }
        else
        {
            view.Content.color = Color.black;
            view.BadBG.gameObject.SetActive(false);
        }

        view.Content.fontSize = 30;
        view.Content.fontSize += Random.Range(0, 6);
        view.Hengfu.raycastTarget = true;
        view.Content.text = txt;
        view.Hengfu.rectTransform.sizeDelta = new Vector2(txt.Length* view.Content.fontSize + 10, view.Hengfu.rectTransform.sizeDelta.y);

    }

    private Color getRandomColor()
    {
        int colorIdx = Random.Range(5, 10);
        if (colorIdx<6)
        {
            return Color.blue;
        }
        else if (colorIdx < 7)
        {
            return Color.red;
        }
        else if (colorIdx < 8)
        {
            return Color.green;
        }
        else if (colorIdx < 9)
        {
            return Color.yellow;
        }
        else if (colorIdx < 10)
        {
            return Color.magenta;
        }
        return Color.white;
    }

    private void BindView()
    {
        view.Hengfu = transform.Find("Hengfu").GetComponent<Image>();

        view.Content = transform.Find("Hengfu").Find("Text").GetComponent<Text>();
        view.BadBG = transform.Find("Hengfu").Find("BadBG").GetComponent<Image>();


    }

    private void RegisterEvent()
    {
        ClickEventListerner listener = view.Hengfu.gameObject.GetComponent<DragEventListener>();
        if (listener == null)
        {
            listener = view.Hengfu.gameObject.AddComponent<DragEventListener>();
        }
        listener.ClearClickEvent();
        listener.OnClickEvent += delegate (PointerEventData eventData) {
            Clicked();
        };
    }

    public void Clicked()
    {
        if (destroying) return;

        if (left > 0)
        {
            left -= 1;
        }
        if(left <= 0)
        {
            gameMode.DestroyDanmu(this);
        }
    }
    public virtual void Tick(float dTime)
    {
        rect.anchoredPosition += Vector2.left *  gameMode.state.DanmuSpd * dTime;
        if (rect.anchoredPosition.x < -100) {
            NeedDestroy = true; 
        }
    }


    public void OnDestroy()
    {
        destroying = true;
        if (gameObject.activeInHierarchy)
        {
            anim.SetTrigger("Disappear");
            StartCoroutine(WaitDeathAnim());
        }
    }


    IEnumerator WaitDeathAnim()
    {
        yield return new WaitForSeconds(0.5f);
        gameMode.RecycleDanmu(this);
    }

    public void SetAsBig()
    {
        view.Content.fontSize = 46;
        view.Content.color = getRandomColor();
        //大弹幕不能是坏的
        isBad = false;
        view.BadBG.gameObject.SetActive(false);
        isBig = true;
    }

}
