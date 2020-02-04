using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ZhiboDanmuView
{
    public GameObject Enter;
    public EmojiText word;
    public Text UserName;
    public Image UserProfile;

    public RectTransform root;

    public void BindView(RectTransform root)
    {
        this.root = root;
        Enter = root.Find("Enter").gameObject;
        UserProfile = Enter.transform.Find("Profile_1").GetComponent<Image>();
        UserName = Enter.transform.Find("Text").GetComponent<Text>();
        word = root.Find("Wenzi").GetComponent<EmojiText>();

    }
    public void SetAsEnter(string name)
    {
        Enter.SetActive(true);
        word.gameObject.SetActive(false);
        root.sizeDelta = new Vector2(root.sizeDelta.x, 100);
        UserName.text = name + "进入直播间!";
    }
    public void SetAsNormal(string content)
    {
        Enter.SetActive(false);
        word.gameObject.SetActive(true);
        word.text = content;
        //Debug.Log(word.preferredHeight);
        root.sizeDelta = new Vector2(root.sizeDelta.x, word.preferredHeight);

    }
}
public class ZhiboDanmuMgr
{
    public float BaseRefreshFreq = 0.0000001f;

    public IResLoader mResLoader;

    public List<ZhiboDanmuView> danmuSlots = new List<ZhiboDanmuView>();
    public int NowDanmuCount = 0;
    public static int MAX_DANMU_COUNT = 40;

    private float timer = 0;

    public ZhiboGameMode gameMode;

    public int importantDanmu = 0;
    public Queue<ZhiboLittleTV> impDanmuTarget = new Queue<ZhiboLittleTV>(); //存放 目标地点

    public ScrollRect danmuSR;
    private RectTransform danmuContent;

    private List<string> nowFengxiang = null;
    private float fengxiangLeftTime = 0f;

    public ZhiboDanmuMgr(ZhiboGameMode gameMode)
    {
        this.gameMode = gameMode;
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();

        InitUI();
    }

    public void InitUI()
    {
        danmuSR = gameMode.mUICtrl.GetDanmuRoot();
        danmuContent = danmuSR.content;

        //for(int i = 0; i < MAX_DANMU_COUNT; i++)
        //{
        //    GameObject go = mResLoader.Instantiate("Zhibo/Danmu", danmuRoot);
        //    ZhiboDanmuView dmView = new ZhiboDanmuView();
        //    dmView.BindView((RectTransform)go.transform);
        //    danmuSlots.Add(dmView);
        //}
        ClearDanmu();
    }

    public void ClearDanmu()
    {
        foreach(Transform go in danmuContent)
        {
            GameObject.Destroy(go.gameObject);
        }
        danmuSlots.Clear();
        //for (int i = 0; i < MAX_DANMU_COUNT; i++)
        //{
        //    danmuSlots[i].root.gameObject.SetActive(false);
        //}
    }

    public void UseCardWithTags(string tagString)
    {
        if(tagString == null || tagString == string.Empty)
        {
            return;
        }
        string[] tags = tagString.Split(',');
        List<string> fengiang = new List<string>(tags);
        AddFengxiang(fengiang);
    }
    public void AddFengxiang(List<string> fengxaing)
    {
        this.nowFengxiang = new List<string>(fengxaing);
        fengxiangLeftTime = 7.5f;
    }

    public void Tick(float dTime)
    {
        timer += dTime;
        if(nowFengxiang != null)
        {
            fengxiangLeftTime -= dTime;
            if(fengxiangLeftTime <= 0)
            {
                nowFengxiang = null;
                fengxiangLeftTime = 0;
            }
        }
        if (importantDanmu == 0)
        {
            float refreshFreq = BaseRefreshFreq;
            if (nowFengxiang != null)
            {
                refreshFreq *= 2.5f;
            }
            if (timer >= 1 / refreshFreq)
            {
                timer -= 1 / refreshFreq;
                //if(timer < 0)
                //{
                //    timer = 0;
                //}
                NextDanmu();
            }
        }
        else
        {
            
            if (timer >= 0.3f)
            {
                timer -= 0.3f;
                NextDanmu();
            }
        }

    }

    public void NextDanmu()
    {
        if(NowDanmuCount < MAX_DANMU_COUNT)
        {

            GameObject go = mResLoader.Instantiate("Zhibo/Danmu", danmuContent);
            ZhiboDanmuView dmView = new ZhiboDanmuView();
            dmView.BindView((RectTransform)go.transform);
            danmuSlots.Add(dmView);

            dmView.SetAsNormal(GenDanmuContent());
            NowDanmuCount++;


        }
        else
        {

            ZhiboDanmuView first = danmuSlots[0];
            danmuSlots.RemoveAt(0);

            first.root.SetAsLastSibling();

            danmuSlots.Add(first);

            first.SetAsNormal(GenDanmuContent());


        }
        Canvas.ForceUpdateCanvases();
        danmuSlots[NowDanmuCount - 1].word.color = Color.white;

        if (importantDanmu > 0)
        {
            danmuSlots[NowDanmuCount - 1].SetAsEnter("heipi");
            ZhiboLittleTV targetLittleTv = impDanmuTarget.Dequeue();
            //danmuSlots[NowDanmuCount - 1].word.color = Color.red;
            gameMode.mUICtrl.ShowNewReqEffect(danmuSlots[NowDanmuCount - 1].root.transform.position, targetLittleTv.GetPivotPos());
            targetLittleTv.Show(0.3f);
            //Debug.Log("shjot");
            importantDanmu -= 1;
        }

        danmuSR.verticalNormalizedPosition = 0;
    }
    private Dictionary<string, List<string>> DanmuTagDict = new Dictionary<string, List<string>>();
    List<string> danmuContents = new List<string>() {"暗示打撒", "暗示打撒大a", "暗示打撒大撒发所asdd大多", "暗示打撒asd大撒发所大多", "暗示打asdd撒大撒发所大多", };
    List<string> gifts = new List<string>() {"e0", "e1", "e2", "e3", "e4", "e5"};
    private string GenDanmuContent()
    {
        float rand = Random.value;
        string user = GenRandomUserName();
        string danmu = "";
        if(rand < 0.3f)
        {
            danmu += user;
            danmu += ":";
            danmu += GenRandomDanmu();
        }
        else if(rand < 0.7f)
        {
            danmu += user;
            danmu += ":";
            danmu += GenRandomDanmu();
            ///danmu += "\n";
            //danmu += "tail";
        }
        else
        {
            danmu += user;
            danmu += "送出了";
            string gift = gifts[Random.Range(0,gifts.Count)];
            danmu += "["+gift+"]";
        }

        return danmu;
    }

    private string GenRandomDanmu()
    {
        if(nowFengxiang == null)
        {
            return danmuContents[Random.Range(0, danmuContents.Count)];
        }
        else
        {
            List<string> pool = new List<string>();
            for(int i = 0; i < nowFengxiang.Count; i++)
            {
                if (!DanmuTagDict.ContainsKey(nowFengxiang[i]))
                {
                    //防止重复查必定没有的，应该将键保存
                    LoadDanmuContent(nowFengxiang[i]);
                }
                pool.AddRange(DanmuTagDict[nowFengxiang[i]]);
                
            }
            if(pool.Count == 0)
            {
                return "主播碉堡了";
            }
            string ret = pool[Random.Range(0,pool.Count)];
            return ret;
        }
    }
    private string GenRandomUserName()
    {
        int i = Random.Range(0,1000);
        string a = string.Format("{0:000}", i);
        return "user" + a;
    }

    public void ShowImportantDanmu(int count, List<ZhiboLittleTV> vector3s)
    {
        if(count != vector3s.Count)
        {
            Debug.Log("刷新数目不相符");
            return;
        }
        importantDanmu += count;
        for(int i = 0; i < count; i++)
        {
            impDanmuTarget.Enqueue(vector3s[i]);
        }
        timer = 0;
    }

    private void LoadDanmuContent(string tag)
    {
        if (DanmuTagDict.ContainsKey(tag))
        {
            return;
        }
        TextAsset ta = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<TextAsset>("DanmuCfg/"+tag, false);
        if(ta == null)
        {
            return;
        }
        string[] lines = ta.text.Split('\n');
        DanmuTagDict[tag] = new List<string>();
        foreach (string line in lines)
        {
            if(line == "")
            {
                continue;
            }
            DanmuTagDict[tag].Add(line);
        }
    }

}
