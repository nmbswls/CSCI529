using UnityEngine;
using System.Collections;

public class TravelPot : MonoBehaviour
{
    SpriteRenderer image;

    bool selected = false;
    public void Start()
    {
        image = GetComponent<SpriteRenderer>();
        RegisterEvent();
    }

    public void RegisterEvent()
    {
        {
            ClickableEventlistener2D listener = gameObject.AddComponent<ClickableEventlistener2D>();
            listener.ClickEvent += delegate {
                selected = !selected;
                if (selected)
                {
                    image.color = Color.red;
                }
                else
                {
                    image.color = Color.gray;
                }


            };
        }
    }
}
