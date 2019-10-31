using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame : MonoBehaviour{

    private static MiniGame mInstance;

    public static MiniGame GetInstance()
    {
        if (mInstance == null)
        {
            Type type = typeof(MiniGame);
            MiniGame gameMain = (MiniGame)FindObjectOfType(type);
            mInstance = gameMain;
        }
        return mInstance;
    }

    public void Release()
    {
        mInstance = null;
    }


    public string info;
    public float spdRate = 1.0f;

    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {

    }

    void Update()
    {
        SomeTick(Time.deltaTime * spdRate);
    }


    public virtual void SomeTick(float dTime)
    {

    }
}
