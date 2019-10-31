using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HintView : BaseView
{
    public Text Content;
}

public class HintModel : BaseModel
{
    public float left = 1.5f;
}

public class HintCtrl : UIBaseCtrl<HintModel, HintView>
{

    private static float FastSpeed = 480f;
    private static float LowSpeed = 80f;

    public void SetContent(string content)
    {
        view.Content.text = content;
    }

    public override void Init()
    {
        Zhiding = true;
    }

    public override void Tick(float dTime)
    {
        model.left -= dTime;
        if (model.left < 0)
        {
            mUIMgr.CloseHint(this);
        }else if (model.left < 0.4f)
        {
            root.transform.localPosition += Vector3.up * dTime * FastSpeed;
        }else if (model.left < 1f)
        {
            root.transform.localPosition += Vector3.up * dTime * LowSpeed;
        }
        else
        {
            root.transform.localPosition += Vector3.up * dTime * FastSpeed;
        }
    }

    public override void BindView()
    {
        view.Content = root.GetChild(0).GetComponent<Text>();
    }

}
