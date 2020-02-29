using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DanmuView2
{
    public Text Content;
    public Image Hengfu;
    public Image BadBG;

    public Image SpeMark;
}

public enum eDanmu2Type
{
    NORMAL,
    BIG,
    SNAKE,
    FLASH,
    AVODE,
    HIGHSPEED,
    MAX,
}

public class DanmuMode2 : MonoBehaviour
{



    [HideInInspector]
    public int left = 1;

    [HideInInspector]
    public int strength;

    [HideInInspector]
    public Color color;

    [HideInInspector]
    public eDanmu2Type danmuType = eDanmu2Type.NORMAL;

    [HideInInspector]
    public bool NeedDestroy = false;

    [HideInInspector]
    public bool isBad = false;

    [HideInInspector]
    public bool isBig = false;

    public static int BidSizeEach = 10;
    public static int NormalSize = 32;

    bool destroying = false;

    public RectTransform rect;
    public Animator anim;

    public DanmuView2 view = new DanmuView2();

    FightingDanmuGameMode gameMode;


    public float AvoidOffset = 200f;
    public float AvoidCD = 0.8f;
    private float avoidCd = 0f;
    //private float yOffset;
    private float yOffTarget;
    private bool isAvoiding;

    public float FlashDistance = 200f;
    public float FlashCD = 2.5f;
    private float flashCd = 0f;

    public float SnakePeriod = 1f;
    public float SnakeDIs = 14f;
    private float aliveTime;

    public CanvasGroup RootCanvasGroup;

    public virtual void init(string txt, bool isBad, FightingDanmuGameMode gameMode)
    {
        rect = (RectTransform)transform;

        RootCanvasGroup = GetComponent<CanvasGroup>();

        this.gameMode = gameMode;
        this.isBad = isBad;
        isBig = false;

        anim = GetComponent<Animator>();
        NeedDestroy = false;
        strength = 1;
        left = 1;



        //color = getRandomColor();
        BindView();
        RegisterEvent();

        this.danmuType = eDanmu2Type.NORMAL;
        view.SpeMark.gameObject.SetActive(false);

        anim.Play("Normal");
        destroying = false;

        RootCanvasGroup.alpha = 1;

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
        //view.Content.color = Color.white;
        //view.BadBG.gameObject.SetActive(false);


        view.Content.fontSize = NormalSize;
        //view.Content.fontSize += Random.Range(0, 6);
        view.Hengfu.raycastTarget = true;
        view.Content.text = txt;
        view.Hengfu.rectTransform.sizeDelta = new Vector2(txt.Length * view.Content.fontSize + 10, view.Hengfu.rectTransform.sizeDelta.y);

    }

    private Color getRandomColor()
    {
        int colorIdx = Random.Range(5, 10);
        if (colorIdx < 6)
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

        view.SpeMark = view.Hengfu.transform.Find("SpeMark").GetComponent<Image>();
    }

    private void RegisterEvent()
    {
        DragEventListener listener = view.Hengfu.gameObject.GetComponent<DragEventListener>();
        if (listener == null)
        {
            listener = view.Hengfu.gameObject.AddComponent<DragEventListener>();
        }
        listener.ClearClickEvent();
        listener.ClearPointerEvent();
        listener.OnClickEvent += delegate (PointerEventData eventData) {
            Clicked();
        };
        listener.PointerEnterEvent += delegate (PointerEventData eventData) {
            OnPointerEnter();
        };
    }

    public void OnPointerEnter()
    {
        if(danmuType == eDanmu2Type.AVODE && avoidCd <= 0)
        {
            StartAvoid();
            avoidCd = AvoidCD;
        }
    }

    private void StartAvoid()
    {

        if(rect.anchoredPosition.y > -AvoidOffset - 20f)
        {

            yOffTarget = rect.anchoredPosition.y - AvoidOffset;
        }
        else if (rect.anchoredPosition.y < -gameMode.mUICtrl.DanmuFieldHeight + 20f + AvoidOffset)
        {
            yOffTarget = rect.anchoredPosition.y + AvoidOffset;
        }
        else
        {
            if (Random.value < 0.5f)
            {
                yOffTarget = rect.anchoredPosition.y + AvoidOffset;
            }
            else
            {
                yOffTarget = rect.anchoredPosition.y - AvoidOffset;
            }
        }

        yOffTarget = ClampPositionY(yOffTarget);
        isAvoiding = true;
    }

    public void Clicked()
    {
        if (destroying) return;
        if (isBad)
        {
            if (left > 0)
            {
                left -= 1;

            }
            if (left == 1)
            {
                DOTween.To
                    (
                        () => view.Content.fontSize,
                        (x) => view.Content.fontSize = x,
                        NormalSize,
                        0.2f
                    );
                Vector2 TargetSize = new Vector2(view.Content.text.Length * NormalSize + 10, view.Hengfu.rectTransform.sizeDelta.y);
                DOTween.To
                    (
                        () => view.Hengfu.rectTransform.sizeDelta,
                        (x) => view.Hengfu.rectTransform.sizeDelta = x,
                        TargetSize,
                        0.2f
                    );
                //view.Content.fontSize = NormalSize;
                //view.Hengfu.rectTransform.sizeDelta = new Vector2(view.Content.text.Length * view.Content.fontSize + 10, view.Hengfu.rectTransform.sizeDelta.y);
            }
            else if (left <= 0)
            {
                gameMode.DanmuClicked(this);
            }
        }
        else
        {
            gameMode.DanmuClicked(this);
        }

    }
    public virtual void Tick(float dTime)
    {

        if(flashCd > 0)
        {
            flashCd -= dTime;
        }

        if (avoidCd > 0)
        {
            avoidCd -= dTime;
        }

        if(danmuType==eDanmu2Type.FLASH&&flashCd < 0)
        {
            Flash();
            flashCd = FlashCD;
        }

        aliveTime += dTime;

        if (danmuType == eDanmu2Type.SNAKE)
        {
            
            float yOffNormalized = Mathf.Sin((aliveTime % SnakePeriod)/ SnakePeriod * 2 * Mathf.PI);
            //Debug.Log(avoidCd);
            float yOff = yOffNormalized * SnakeDIs;
            rect.anchoredPosition += yOff * Vector2.up;
        }

        Vector2 spd = Vector2.left * gameMode.state.DanmuSpd;
        if(danmuType == eDanmu2Type.HIGHSPEED)
        {
            spd *= 2.5f;
        }
        rect.anchoredPosition += spd * dTime;
        if(danmuType == eDanmu2Type.AVODE && isAvoiding)
        {
            float yy = Mathf.Lerp(rect.anchoredPosition.y, yOffTarget,0.3f);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, yy);
            if(Mathf.Abs(yy - yOffTarget) < 1e-2)
            {
                isAvoiding = false;
            }
        }



        if (rect.anchoredPosition.x < -100)
        {
            NeedDestroy = true;
        }

        float y = ClampPositionY(rect.anchoredPosition.y);
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, y);
    }



    public void Flash()
    {

        anim.Play("FadeIn");
        float angle = Random.value * 2 * Mathf.PI;
        Vector2 v = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * FlashDistance;
        if(v.x > 0)
        {
            v.x = -v.x;
        }
        rect.anchoredPosition += v;
        float y = ClampPositionY(rect.anchoredPosition.y);
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x,y);
    }

    private float ClampPositionY(float originY)
    {
        float ret = originY;
        if (ret > -20f)
        {
            ret = -20;
        }
        if (ret < -gameMode.mUICtrl.DanmuFieldHeight + 20f)
        {
            ret =  - gameMode.mUICtrl.DanmuFieldHeight + 20f;
        }
        return ret;
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

    private void SetAsBig()
    {
        left += 1;
        view.Content.fontSize = BidSizeEach*(left-1) + NormalSize;
        //view.Content.color = getRandomColor();
        isBig = true;
    }

    private void SetAsHighSpeed()
    {
        this.danmuType = eDanmu2Type.HIGHSPEED;
    }

    private void SetAsSnake()
    {
        this.danmuType = eDanmu2Type.SNAKE;
        aliveTime = 0f;
    }

    private void SetAsAvode()
    {
        this.danmuType = eDanmu2Type.AVODE;
        isAvoiding = false;
    }

    private void SetAsFlash()
    {
        this.danmuType = eDanmu2Type.FLASH;
        flashCd = FlashCD;
    }

    public void SetAsSpecial()
    {
        //this.isBad = true;
        if(!isBad)
        {
            return;
        }
        view.Content.color = Color.white;
        view.BadBG.gameObject.SetActive(true);
        view.SpeMark.gameObject.SetActive(true);
        int randIdx = Random.Range(1,(int)eDanmu2Type.MAX);
        //int randIdx = 4;
        switch (randIdx)
        {
            case (int)eDanmu2Type.BIG:
                SetAsBig();
                break;
            case (int)eDanmu2Type.AVODE:
                SetAsAvode();
                break;
            case (int)eDanmu2Type.HIGHSPEED:
                SetAsHighSpeed();
                break;
            case (int)eDanmu2Type.FLASH:
                SetAsFlash();
                break;
            case (int)eDanmu2Type.SNAKE:
                SetAsSnake();
                break;
            default:
                break;
        }

    }



}
