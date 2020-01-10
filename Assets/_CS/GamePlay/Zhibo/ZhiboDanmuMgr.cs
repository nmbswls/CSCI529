using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ZhiboDanmuView
{
    public Text Wenzi;
    public RectTransform root;

    public void BindView(RectTransform root)
    {
        this.root = root;
        Wenzi = root.Find("Wenzi").GetComponent<Text>();
    }
}
public class ZhiboDanmuMgr
{
    public float RefreshFreq = 1f;

    public IResLoader mResLoader;

    public List<ZhiboDanmuView> danmuSlots = new List<ZhiboDanmuView>();
    public int NowDanmuCount = 0;
    public static int MAX_DANMU_COUNT = 20;

    private float timer = 0;

    public ZhiboGameMode gameMode;

    public int importantDanmu = 0;

    public ZhiboDanmuMgr(ZhiboGameMode gameMode)
    {
        this.gameMode = gameMode;
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();

        InitUI();
    }

    public void InitUI()
    {
        Transform root = gameMode.mUICtrl.GetDanmuRoot();

        for(int i = 0; i < MAX_DANMU_COUNT; i++)
        {
            GameObject go = mResLoader.Instantiate("Zhibo/Danmu", root);
            ZhiboDanmuView dmView = new ZhiboDanmuView();
            dmView.BindView((RectTransform)go.transform);
            danmuSlots.Add(dmView);
        }
        ClearDanmu();
    }

    public void ClearDanmu()
    {
        for (int i = 0; i < MAX_DANMU_COUNT; i++)
        {
            danmuSlots[i].root.gameObject.SetActive(false);
        }
    }

    public void Tick(float dTime)
    {
        timer += dTime;
        if(importantDanmu == 0)
        {
            if (timer >= 1 / RefreshFreq)
            {
                timer -= 1 / RefreshFreq;
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
            danmuSlots[NowDanmuCount].Wenzi.text = "new danmu"+Time.time;
            danmuSlots[NowDanmuCount].root.gameObject.SetActive(true);
            NowDanmuCount++;
        }
        else
        {
            for (int i = 0; i < danmuSlots.Count-1; i++)
            {
                danmuSlots[i].Wenzi.text = danmuSlots[i + 1].Wenzi.text;
                danmuSlots[i].Wenzi.color = danmuSlots[i + 1].Wenzi.color;
            }
            danmuSlots[NowDanmuCount-1].Wenzi.text = "new danmu"+ Time.time;
        }

        danmuSlots[NowDanmuCount - 1].Wenzi.color = Color.white;

        if (importantDanmu > 0)
        {
            danmuSlots[NowDanmuCount - 1].Wenzi.color = Color.red;
            gameMode.mUICtrl.ShowDanmuEffect(danmuSlots[NowDanmuCount - 1].root.transform.position);
            Debug.Log("shjot");
            importantDanmu -= 1;
        }

    }

    public void ShowImportantDanmu(int count)
    {
        importantDanmu += count;
        timer = 0;
    }

}
