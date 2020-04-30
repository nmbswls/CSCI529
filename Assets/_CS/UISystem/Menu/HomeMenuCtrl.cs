using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class HomeMenuModel:BaseModel{
}

public class HomeMenuView:BaseView{
	public Button NewGame;
    public Button LoadGame;
    public Button Setting;
    public Button Quit;


    public Transform SetPage;
    public Scrollbar BGMVolume;
    public Button Back;
    public Text VolumeNum;
}

public class HomeMenuCtrl : UIBaseCtrl<HomeMenuModel,HomeMenuView>
{

	public override void Init(){
		model = new HomeMenuModel ();
		view = new HomeMenuView ();

		mUIMgr = GameMain.GetInstance ().GetModule<UIMgr> ();

	}

	// Use this for initialization
	public override void BindView(){
		view.NewGame = root.Find("Setup").GetComponent<Button> ();
        view.LoadGame = root.Find("Login").GetComponent<Button>();
        view.Setting = root.Find("Set").GetComponent<Button>();
        view.Quit = root.Find("Quit").GetComponent<Button>();

        view.SetPage = root.Find("SetPage");
        view.BGMVolume = view.SetPage.Find("Scrollbar").GetComponent<Scrollbar>();
        view.Back = view.SetPage.Find("Back").GetComponent<Button>();
        view.VolumeNum = view.SetPage.Find("VolumeNum").GetComponent<Text>();
    }

    public override void RegisterEvent() {
        view.NewGame.onClick.AddListener(delegate () {
            mUIMgr.CloseCertainPanel(this);
            //mUIMgr.ShowPanel("StartNewGame");
            //跳过选人直接开始
            AdjustInitCtrl ctrl = mUIMgr.ShowPanel("AdjustPanel") as AdjustInitCtrl;
            ctrl.SetRoleId(0);
        });

        view.LoadGame.onClick.AddListener(delegate () {
            mUIMgr.CloseCertainPanel(this);
            //mUIMgr.ShowPanel("StartNewGame");
            //跳过选人直接开始
            AdjustInitCtrl ctrl = mUIMgr.ShowPanel("AdjustPanel") as AdjustInitCtrl;
            ctrl.SetRoleId(0);
        });

        view.Setting.onClick.AddListener(delegate () {
            //setting
            view.SetPage.gameObject.SetActive(true);
        });

        view.Quit.onClick.AddListener(delegate () {
            Debug.Log("Quit Game");
            Application.Quit();
        });

        view.Back.onClick.AddListener(delegate ()
        {
            view.SetPage.gameObject.SetActive(false);
        });

        view.BGMVolume.onValueChanged.AddListener(delegate
        {
            GameMain.GetInstance().AdjustVolume(view.BGMVolume.value);
            view.VolumeNum.text = view.BGMVolume.value * 100 + "";
        });
    }
}

