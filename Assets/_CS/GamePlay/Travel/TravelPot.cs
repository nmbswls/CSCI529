using UnityEngine;
using System.Collections;

public class TravelPot : MonoBehaviour
{
    SpriteRenderer image;

    bool selected = false;

    TravelGameMode gm;
    public void Init(TravelGameMode gm)
    {
        this.gm = gm;
        image = GetComponent<SpriteRenderer>();
        RegisterEvent();

    }


    public void RegisterEvent()
    {
        {
            ClickableEventlistener2D listener = gameObject.GetComponent<ClickableEventlistener2D>();
            if(listener == null)
            {
                listener = gameObject.AddComponent<ClickableEventlistener2D>();
                listener.ClickEvent += delegate {
                    gm.ChoosePot(this);
                };
                Debug.Log(listener.hasClickEvent());
            }

        }
    }

    public void Selected()
    {
        image.color = Color.red;
    }

    public void CancelSelect()
    {
        image.color = Color.white;
    }
}
