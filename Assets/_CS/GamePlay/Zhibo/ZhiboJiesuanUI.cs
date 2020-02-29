using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZhiboJiesunView : BaseView
{
    public Button OKBtn;
    public Text text;
    public Text fensi;
    public Text money;
}

public class ZhiboJiesuanUI : UIBaseCtrl<BaseModel, ZhiboJiesunView>
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
        view.fensi = root.Find("Fensi").GetComponent<Text>();
        view.money = root.Find("Money").GetComponent<Text>();
    }
    public override void RegisterEvent()
    {
        base.RegisterEvent();
        view.OKBtn.onClick.AddListener(delegate {
            ZhiboGameMode gameMode = GameMain.GetInstance().GetModule<CoreManager>().GetGameMode() as ZhiboGameMode;
            Debug.Log(gameMode.mUICtrl==null);
            mUIMgr.CloseCertainPanel(gameMode.mUICtrl);
            mUIMgr.CloseCertainPanel(this);

            MainGMInitData data = new MainGMInitData();
            data.isNextTurn = true;
            GameMain.GetInstance().GetModule<CoreManager>().ChangeScene("Main", data);
        });
    }

    public void SetContent(string bonusString)
    {
        view.text.text = bonusString;
        view.text.text += "粉丝累计:";
    }
    
    public void showFensi(int score)
    {
        view.fensi.text = score + "";
    }

    public void showMoney(int score)
    {
        view.money.text = score + "";
    }
}
