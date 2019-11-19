using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ZhiboMode2View : BaseView
{

    public Text TimeLeft;

    public Text ScoreValue;
    public Text ScoreTarget;

    public Slider Hp;
    public Text HpValue;

    public Image ShieldBg;
    public Image ShieldFront;
    public Text ShieldTimeLeft;


    public Image EnegyBar;
    public Text EnegyValue;

    public List<OperatorView> Actions = new List<OperatorView>();

    public OperatorView Passive;

    public RectTransform DanmuField;
    public RectTransform GoodLayer;
    public RectTransform BadLayer;


    public SkillDetailPanel skillDetail;

}

public class SkillDetailPanel
{
    public RectTransform root;
    public Text SkillName;
    public Text CdLeft;
    public Text Cost;
    public Text Desp;

    public void BindView(Transform root)
    {
        this.root = root as RectTransform;
        this.SkillName = root.Find("Name").GetComponent<Text>();
        this.Desp = root.Find("Desp").GetComponent<Text>();
        this.CdLeft = root.Find("CdLeft").GetComponent<Text>();
        this.Cost = root.Find("Cost").GetComponent<Text>();
    }
}

public class OperatorView
{
    public RectTransform root;
    public Image ActiveButton;
    public Image Outline;
    public Image CDMask;
    public Text Title;
    public Image Picture;

    public void BindView(Transform root)
    {
        this.root = (RectTransform)root;
        this.ActiveButton = root.Find("OptFace").GetComponent<Image>();
        this.CDMask = root.Find("Cd").GetComponent<Image>();
        this.Title = root.Find("Title").GetComponent<Text>();
        this.Picture = root.Find("Picture").GetComponent<Image>();
    }


}



public class ZhiboMode2UICtrl : UIBaseCtrl<BaseModel, ZhiboMode2View>
{



    private int width = 0;
    private int height = 0;
    private int numOfGridVertical;
    private static float MinDanmuInterval = 0.5f;


    public int DanmuFieldHeight = 0;

    private float TimerPerSec = 0;

    private int preDanmuGrid;
    private float[] preDanmuTime;

    private string DanmuFengxiang;


    IResLoader mResLoader;
    public ZhiboGameMode2 gameMode;

    private int nowIdx = -2;


    public override void Init()
    {
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        gameMode = GameMain.GetInstance().GetModule<CoreManager>().GetGameMode() as ZhiboGameMode2;
    }

    public override void PostInit()
    {
        width = (int)view.DanmuField.rect.width;
        height = (int)view.DanmuField.rect.height;

        DanmuFieldHeight = height;
        Debug.Log(DanmuFieldHeight);
        numOfGridVertical = 40;
        preDanmuGrid = -1;
        preDanmuTime = new float[numOfGridVertical];


        UpdateHp();
        UpdateScore();
        UpdateTimeLeft();
        UpdateEnegy();
        UpdateShieldView();

        nowIdx = -2;
        //UpdateActions();
    }


    public void UpdateActionCd()
    {
        for (int i = 0; i < ZhiboGameMode2.ActionNum; i++)
        {
            OperatorView vv = view.Actions[i];
            if (i >= gameMode.state.PresetActions.Count || gameMode.state.PresetActions[i] == null)
            {
                return;
            }
            vv.CDMask.fillAmount = gameMode.state.PresetActions[i].CdLeft / gameMode.state.PresetActions[i].Cd;
        }
    }

    public void UpdateActions()
    {
        for(int i = 0; i < ZhiboGameMode2.ActionNum; i++)
        {
            OperatorView vv = view.Actions[i];
            if (i>=gameMode.state.PresetActions.Count || gameMode.state.PresetActions[i] == null)
            {
                return;
            }
            vv.Title.text = gameMode.state.PresetActions[i].Name;
            vv.Picture.sprite = mResLoader.LoadResource<Sprite>("CardImage/" + gameMode.state.PresetActions[i].PictureUrl);
        }
        {
            if(gameMode.state.Passive != null)
            {
                OperatorView vv = view.Passive;
                vv.Title.text = gameMode.state.Passive.Name;
                vv.Picture.sprite = mResLoader.LoadResource<Sprite>("CardImage/" + gameMode.state.Passive.PictureUrl);
            }
        }
    }


    public void UpdateHp()
    {
        view.Hp.value = gameMode.state.Hp * 1.0f / gameMode.state.MaxHp;
        view.HpValue.text = gameMode.state.Hp +"/"+gameMode.state.MaxHp;
    }


    public void UpdateTargetScore()
    {
        view.ScoreTarget.text = "Target: " + gameMode.state.TargetScore;
    }
    public void UpdateScore()
    {
        float nowScore = gameMode.state.Score;
        view.ScoreValue.text = (int)nowScore +"";
    }


    public void UpdateTimeLeft()
    {
        view.TimeLeft.text = gameMode.state.TimeLeft + "";
    }

    public void UpdateEnegy()
    {
        view.EnegyBar.fillAmount = gameMode.state.Enegy * 1.0f / gameMode.state.MaxEnegy;
        view.EnegyValue.text = (int)gameMode.state.Enegy+"";
    }






    public override void BindView()
    {
        view.TimeLeft = root.Find("TimeLeft").GetComponentInChildren<Text>();

        view.DanmuField = (RectTransform)(root.Find("DanmuField"));
        view.GoodLayer = view.DanmuField.Find("Good") as RectTransform;
        view.BadLayer = view.DanmuField.Find("Bad") as RectTransform;


        view.ScoreValue = root.Find("Score").Find("Value").GetComponent<Text>();
        view.ScoreTarget = root.Find("Score").Find("Target").GetComponent<Text>();

        view.Hp = root.Find("Hp").GetComponent<Slider>();
        view.HpValue = root.Find("Hp").Find("Value").GetComponent<Text>();

        view.ShieldBg = root.Find("Hp").Find("ShieldBg").GetComponent<Image>();
        view.ShieldFront = root.Find("Hp").Find("ShieldFront").GetComponent<Image>();
        view.ShieldTimeLeft = root.Find("Hp").Find("ShieldFront").Find("TimeValue").GetComponent<Text>();

        view.EnegyBar = root.Find("EnegyBar").Find("Image").GetComponent<Image>();
        view.EnegyValue = root.Find("EnegyBar").Find("Value").GetComponent<Text>();

        Transform skillDetailTr = root.Find("SkillDetail");
        {
            SkillDetailPanel vv = new SkillDetailPanel();
            vv.BindView(skillDetailTr);
            view.skillDetail = vv;
            vv.root.gameObject.SetActive(false);
        }

        Transform Actions = root.Find("Options");

        view.Actions.Clear();
        for (int i = 0; i < ZhiboGameMode2.ActionNum; i++)
        {
            Transform child = Actions.GetChild(i);
            OperatorView vv = new OperatorView();
            vv.BindView(child);
            view.Actions.Add(vv);
        }
        Transform passive = root.Find("Passive");
        {
            OperatorView vv = new OperatorView();
            vv.BindView(passive);
            view.Passive = vv;
        }

    }


    public void UpdateShieldTimer()
    {
        view.ShieldTimeLeft.text = (int)gameMode.state.ArmorTimer + "";
    }

    public void UpdateShieldView()
    {
        if(gameMode.state.ArmorTimer <= 0)
        {
            view.ShieldFront.gameObject.SetActive(false);
            view.ShieldBg.gameObject.SetActive(false);
        }
        else
        {
            view.ShieldBg.gameObject.SetActive(true);
            view.ShieldFront.gameObject.SetActive(true);
            view.ShieldTimeLeft.text = (int)gameMode.state.ArmorTimer + "";
        }
    }

    public void ShowDanmuEffect(Vector3 pos)
    {
        GameObject go = mResLoader.Instantiate("Zhibo/Effect", root);
        go.transform.position = pos;
        float time = (view.ScoreValue.transform.position - go.transform.position).magnitude / 10f;
        DOTween.To
            (
                () => go.transform.position,
                (x) => go.transform.position = x,
                view.ScoreValue.transform.position,
                time
            ).OnComplete(delegate ()
            {
                mResLoader.ReleaseGO("Zhibo/Effect", go);
            });
    }


    public void ShowDamageAmountEffect(Vector3 pos, int value)
    {
        GameObject go = mResLoader.Instantiate("ZhiboMode2/DamageNumber", root);
        Text t = go.GetComponentInChildren<Text>();
        if(value >= 0)
        {
            t.text = "+" + value;
            t.color = Color.green;
        }
        else
        {
            t.text = value + "";
            t.color = Color.red;
        }

        go.transform.position = pos;
        float time = 1.5f;
        Vector3 TargetPos = pos + Vector3.up * time * 1f;
        DOTween.To
            (
                () => go.transform.position,
                (x) => go.transform.position = x,
                TargetPos,
                time
            ).OnComplete(delegate ()
            {
                mResLoader.ReleaseGO("ZhiboMode2/DamageNumber", go);
            });

    }



    public override void RegisterEvent()
    {
        for(int i=0;i< ZhiboGameMode2.ActionNum; i++)
        {
            int idx = i;
            GameObject toBind = view.Actions[i].ActiveButton.gameObject;
            DragEventListener listener = toBind.GetComponent<DragEventListener>();
            if (listener == null)
            {
                listener = toBind.AddComponent<DragEventListener>();
                listener.OnClickEvent += delegate {
                    UseAbility(idx);
                };
                listener.PointerEnterEvent += delegate {

                    ShowSkillDetail(idx);
                };
                listener.PointerExitEvent += delegate {

                    HideSkillDetail();
                };
            }
        }

        {
            GameObject toBind = view.Passive.ActiveButton.gameObject;
            DragEventListener listener = toBind.GetComponent<DragEventListener>();
            if (listener == null)
            {
                listener = toBind.AddComponent<DragEventListener>();
                listener.PointerEnterEvent += delegate {

                    ShowSkillDetail(-1);
                };
                listener.PointerExitEvent += delegate {

                    HideSkillDetail();
                };
            }
        }
    }

    public void UpdateSkillDetailCD()
    {
        if(nowIdx == -2)
        {
            return;
        }
        if(nowIdx == -1)
        {
            view.skillDetail.CdLeft.text = "就绪";
            return;
        }

        if ((int)gameMode.state.PresetActions[nowIdx].CdLeft > 0)
        {
            view.skillDetail.CdLeft.text = (int)gameMode.state.PresetActions[nowIdx].CdLeft + "";
        }
        else
        {
            view.skillDetail.CdLeft.text = "就绪";
        }


    }

    public void ShowSkillDetail(int idx)
    {
        if(idx < -1 || idx >= ZhiboGameMode2.ActionNum)
        {
            return;
        }
        nowIdx = idx;
        ZhiboMode2Skill toSHow;
        if (idx == -1)
        {
            toSHow = gameMode.state.Passive;
        }
        else
        {
            toSHow = gameMode.state.PresetActions[idx];

        }
        if (toSHow == null)
        {
            return;
        }
        view.skillDetail.SkillName.text = toSHow.Name;
        view.skillDetail.Desp.text = toSHow.Desp;
        view.skillDetail.CdLeft.text = toSHow.CdLeft+"";
        view.skillDetail.Cost.text = string.Format("消耗 {0} 点能量\n冷却时间 {1}s",toSHow.EnegyCost,toSHow.Cd);


        view.skillDetail.root.gameObject.SetActive(true);

        if(idx == -1)
        {
            view.skillDetail.root.position = view.Passive.root.position + new Vector3(0.3f, 0.4f, 0);
        }
        else
        {
            view.skillDetail.root.position = view.Actions[idx].root.position + new Vector3(0.3f, 0.4f, 0);
        }

        UpdateSkillDetailCD();

    }

    public void HideSkillDetail()
    {
        view.skillDetail.root.gameObject.SetActive(false);
        nowIdx = -2;
    }


    public override void Tick(float dTime)
    {
        //dTime = dTime * gameMode.spdRate;
        //TimerPerSec += dTime;
        //bool triggerSec = false;
        //if (TimerPerSec > 1f)
        //{
        //    TimerPerSec -= 1f;
        //    triggerSec = true;
        //}
    }





    public void ChangeTurnTime(float value)
    {
        //view.TurnTimeValue.text = (int)value+"";
        view.TimeLeft.text = (int)value + "";
    }

    public DanmuMode2 GenDanmu(bool isBad)
    {
        List<int> DanmuSlots = new List<int>();


        for (int i = 2; i < numOfGridVertical - 2; i++)
        {
            if (Time.time - preDanmuTime[i] > MinDanmuInterval)
            {
                DanmuSlots.Add(i);
            }
        }
        int gridY;

        if (DanmuSlots.Count == 0)
        {
            gridY = Random.Range(2, numOfGridVertical - 2);

        }
        else
        {
            gridY = DanmuSlots[Random.Range(0, DanmuSlots.Count)];
        }

        preDanmuGrid = gridY;

        float posY = gridY * 1.0f / numOfGridVertical * height;

        preDanmuTime[gridY] = Time.time;

        GameObject danmuGo = mResLoader.Instantiate("ZhiboMode2/Danmu2");

        DanmuMode2 danmu = danmuGo.GetComponent<DanmuMode2>();


        danmu.init(gameMode.GetDanmuContent(), isBad, gameMode);
        danmuGo.transform.SetParent(view.DanmuField, false);
        danmu.rect.anchoredPosition = new Vector3(width + 30, -posY, 0);

        return danmu;
    }




    public void UseAbility(int idx)
    {
        gameMode.UseAbility(idx);
        UpdateSkillDetailCD();
    }


    public SuperDanmuMode2 ShowSuperDanmu(int type)
    {

        GameObject danmuGo = mResLoader.Instantiate("ZhiboMode2/SuperDanmu", view.BadLayer);
        SuperDanmuMode2 danmu = danmuGo.GetComponent<SuperDanmuMode2>();

        int randY = Random.Range(-100,-height+100);


        danmu.init(gameMode.GetSuperDanmuContent(),(eSuperDanmuType)type, gameMode);
        danmu.rect.anchoredPosition = new Vector3(width, randY, 0);
        return danmu;
    }







}
