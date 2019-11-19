using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FightDanmuJiesunView : BaseView
{
    public Button OKBtn;
    public Text text;
}

public class FightDanmuJiesuanUI : UIBaseCtrl<BaseModel, FightDanmuJiesunView>
{

    //public ZhiboGameMode gameMode;
    public override void Init()
    {
        //gameMode = GameMain.GetInstance().GetModule<CoreManager>().GetGameMode() as ZhiboGameMode;
    }

    public override void PostInit()
    {

    }
    public override void BindView()
    {
        view.OKBtn = root.Find("OK").GetComponent<Button>();
        view.text = root.Find("Text").GetComponent<Text>();
    }
    public override void RegisterEvent()
    {
        base.RegisterEvent();
        view.OKBtn.onClick.AddListener(delegate {
            ZhiboGameMode2 gameMode = GameMain.GetInstance().GetModule<CoreManager>().GetGameMode() as ZhiboGameMode2;
            Debug.Log(gameMode.mUICtrl == null);
            mUIMgr.CloseCertainPanel(gameMode.mUICtrl);
            mUIMgr.CloseCertainPanel(this);
            GameMain.GetInstance().GetModule<CoreManager>().ChangeScene("Main");
        });
    }

    public void SetContent(string bonusString)
    {
        view.text.text = bonusString;
    }
}