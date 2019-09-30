using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DanmuView
{
    public Text textField;
    public Image image0;
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
    public float spd;

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

    public RectTransform rect;
    public Animator anim;

    public DanmuView view = new DanmuView();

    ZhiboGameMode gameMode;

    public void init(string txt, ZhiboGameMode gameMode)
    {
        rect = (RectTransform)transform;

        this.gameMode = gameMode;

        anim = GetComponent<Animator>();
        NeedDestroy = false;
        spd = 160.0f;
        strength = 1;
        left = 1;
        color = getRandomColor();
        BindView();
        RegisterEvent();

        anim.Play("Normal");
        view.textField.color = color;

    }

    private Color getRandomColor()
    {
        int colorIdx = Random.Range(0, 10);
        if (colorIdx < 5)
        {
            return Color.black;
        }
        else if (colorIdx<6)
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
        view.textField = transform.GetChild(1).GetComponent<Text>();
        view.image0 = transform.GetChild(0).GetComponent<Image>();
    }

    public void RegisterEvent()
    {
        DragEventListener listener = view.textField.gameObject.GetComponent<DragEventListener>();
        if (listener == null)
        {
            listener = view.textField.gameObject.AddComponent<DragEventListener>();
            listener.OnClickEvent += delegate (PointerEventData eventData) {
                gameMode.mUICtrl.HitDanmu(this);
               
            };
        }
    }

    public void Tick(float dTime)
    {
        rect.anchoredPosition += Vector2.left * spd * dTime;
        if (rect.anchoredPosition.x < -100) {
            NeedDestroy = true; 
        }
    }


    public void OnDestroy()
    {
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


}
