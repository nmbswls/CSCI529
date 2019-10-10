using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MaskView : BaseView
{
    public Image Mask;
}

public class ModelMask : UIBaseCtrl<BaseModel, MaskView>
{


    public override void BindView()
    {
        view.Mask = root.GetComponent<Image>();
    }

    public override void RegisterEvent()
    {
        ClickEventListerner listener = root.gameObject.GetComponent<ClickEventListerner>();
        if (listener == null)
        {
            listener = root.gameObject.AddComponent<ClickEventListerner>();
            listener.OnClickEvent += delegate (PointerEventData eventData) {
                mUIMgr.CloseFirstPanel();
            };
        }
    }

}
