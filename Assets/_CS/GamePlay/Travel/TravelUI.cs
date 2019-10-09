using UnityEngine;
using System.Collections;


public class TravelView:BaseView
{

}

public class TravelModel:BaseModel
{

}

public class TravelUI : UIBaseCtrl<TravelModel, TravelView>
{

    IResLoader mResLoader;
    public TravelGameMode gameMode;

    public override void Init()
    {
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        gameMode = GameMain.GetInstance().GetModule<CoreManager>().GetGameMode() as TravelGameMode;
    }


    public override void PostInit()
    {
    }
}
