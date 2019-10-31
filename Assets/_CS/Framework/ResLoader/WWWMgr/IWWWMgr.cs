using UnityEngine;
using System.Collections;

public enum WWWType
{
    DEFAULT,
    IMAGE_1,
    QUEST_INFO,
    DATABIN,
}

public class WWWFileInfo
{
    public string url;
    public WWWType type;
    public string localPath = string.Empty;
    public long ExpiredTimestamp;
    public string md5;
}

public interface IWWWRequestHandle
{
    IWWWRequestHandle Get();
    IWWWRequestHandle Abort();

    IWWWRequestHandle SetType(WWWType type);
    IWWWRequestHandle SetUrl(string url);

    WWWFileInfo GetFileInfo();

    IWWWRequestHandle SetSuccessCallback(WWWMgr.RequestWWWEventDelegate callback);

    IWWWRequestHandle TrySuccessCallback();

}

public interface IWWWMgr
{
    T TryLoadFromCache<T>(WWWType type, string url) where T : Object;

    IWWWRequestHandle BuildRequest(WWWType type, string url);

    void Update(float dTime);

}