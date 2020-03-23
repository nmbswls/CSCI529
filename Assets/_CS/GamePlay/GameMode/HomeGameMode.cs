using UnityEngine;
using System.Collections;

public class HomeGameMode : GameModeBase
{

    IUIMgr UImgr;

    public override void Init(GameModeInitData initData)
    {
        UImgr = GameMain.GetInstance().GetModule<UIMgr>();
        UImgr.ShowPanel("HomeMenuCtrl");

    }

    public override void Tick(float dTime)
    {

    }
}
