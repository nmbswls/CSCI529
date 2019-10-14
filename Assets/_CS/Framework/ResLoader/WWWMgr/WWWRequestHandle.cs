using UnityEngine;
using System.Collections;

public class WWWRequestHandle : IWWWRequestHandle
{
    public WWWMgr mWWWMgr;
    public WWWType Type;
    public string Url;


    public WWWMgr.RequestWWWEventDelegate SuccessCallback;
    public WWWMgr.RequestWWWEventDelegate FailCallback;

    public WWWRequestHandle(WWWMgr wwwMgr)
    {
        //mId = wwwMgr.GetIncreaseId();
        Init(wwwMgr);
    }

    private void Init(WWWMgr wwwMgr)
    {
        mWWWMgr = wwwMgr;
    }

    public IWWWRequestHandle Abort()
    {
        throw new System.NotImplementedException();

        mWWWMgr.Abort(this);
    }

    public IWWWRequestHandle Get()
    {

        return mWWWMgr.SendRequest(this);
    }

    public WWWFileInfo GetFileInfo()
    {
        throw new System.NotImplementedException();
    }

    public IWWWRequestHandle SetSuccessCallback(WWWMgr.RequestWWWEventDelegate callback)
    {
        SuccessCallback = callback;
        return this;
    }

    public IWWWRequestHandle SetFailCallback(WWWMgr.RequestWWWEventDelegate callback)
    {
        FailCallback = callback;
        return this;
    }

    public IWWWRequestHandle SetType(WWWType type)
    {
        Type = type;
        return this;
    }

    public IWWWRequestHandle SetUrl(string url)
    {
        Url = url;
        return this;
    }

    public IWWWRequestHandle TrySuccessCallback()
    {
        if (SuccessCallback != null)
        {
            SuccessCallback(this);

        }
        return this;
    }

    public IWWWRequestHandle TryFailCallback()
    {
        if (FailCallback != null)
        {
            FailCallback(this);

        }
        return this;
    }
}
