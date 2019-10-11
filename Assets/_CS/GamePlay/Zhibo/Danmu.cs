using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DanmuView
{
    public Text textField;
    public Image image0;
    public Image BadBg;
}

public enum ENM_DanmuType
{
    NORMAL,
    GOOD,
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

    public void init(string txt, bool isBad, ZhiboGameMode gameMode)
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

        if (isBad)
        {
            color = Color.red;
        }
        else
        {
            color = Color.black;
        }
        //color = getRandomColor();
        BindView();
        RegisterEvent();

        anim.Play("Normal");
        view.textField.color = color;
        destroying = false;

        if (isBad)
        {
            view.textField.color = Color.white;
            view.BadBg.gameObject.SetActive(true);
        }
        else
        {
            view.BadBg.gameObject.SetActive(false);
        }

        view.textField.raycastTarget = true;
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
        view.textField = transform.Find("Text").GetComponent<Text>();
        view.image0 = transform.Find("Image").GetComponent<Image>();
        view.BadBg = transform.Find("BadBg").GetComponent<Image>();

    }

    public void RegisterEvent()
    {
        DragEventListener listener = view.textField.gameObject.GetComponent<DragEventListener>();
        if (listener == null)
        {
            listener = view.textField.gameObject.AddComponent<DragEventListener>();
            listener.OnClickEvent += delegate (PointerEventData eventData) {
                if (destroying) return;

                gameMode.mUICtrl.HitDanmu(this);
            };
        }
    }

    public void Tick(float dTime)
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
        view.textField.fontSize = 46;
        view.textField.color = getRandomColor();
        //大弹幕不能是坏的
        isBad = false;
        view.BadBg.gameObject.SetActive(false);
        isBig = true;
    }


}
