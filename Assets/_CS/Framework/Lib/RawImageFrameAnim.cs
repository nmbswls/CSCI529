using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class RawImageFrameAnim : MonoBehaviour
{

    public int row;
    public int column;
    public int frameNum;
    public int frameRate;
    private float interval;
    private float w;
    private float h;

    private RawImage image;
    private float timer;
    private int frameIdx;

    private void Start()
    {
        image = GetComponent<RawImage>();
        if (frameRate < 0)
        {
            frameRate = 0;
        }
        if(frameRate == 0)
        {
            interval = 0;
        }
        else
        {
            interval = 1.0f / frameRate;
        }

        w = 1.0f / column;
        h = 1.0f / row;

        timer = 0;
        frameIdx = 0;
        if(frameNum > row * column)
        {
            frameNum = row * column;
        }
    }
    private void Update()
    {
        Tick(Time.deltaTime);
    }
    void Tick(float dTime)
    {
        if(interval > 0)
        {
            timer += dTime;
            if (timer > interval)
            {
                timer -= interval;
                frameIdx = (frameIdx + 1) % frameNum;
                Rect rect = new Rect(frameIdx % column * w, frameIdx / column * h, w, h);
                image.uvRect = rect;
            }
        }
    }

    
    
}
