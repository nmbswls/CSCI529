using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZhiboBuff : MonoBehaviour
{

    private static float FlashInterval = 0.5f;

    ZhiboGameMode gameMode;
    public string buffId;
    public int buffLevel;
    public float totalLastTime;
    public float leftTime;

    private static float basicAlpht = 0.8f;
    private Image icon;
    public void Init(string buffId, int buffLevel, float totalLastTime, ZhiboGameMode gameMode)
    {
        this.buffId = buffId;
        this.buffLevel = buffLevel;
        this.totalLastTime = totalLastTime;
        this.leftTime = totalLastTime;
        this.gameMode = gameMode;

        BindView();
    }


    public void BindView()
    {
        icon = GetComponentInChildren<Image>();
    }

    public void Tick(float dTime)
    {
        leftTime -= dTime;
        if(leftTime < 3f)
        {
            if (leftTime > 0)
            {
                icon.color = GetFlashingColor();
            }
        }
    }

    public Color GetFlashingColor()
    {
        float a = Mathf.Abs(1 - (leftTime - (int)(leftTime / FlashInterval) * FlashInterval) / FlashInterval * 2);
        return new Color(1, 1, 1, a* basicAlpht);
    }

}
