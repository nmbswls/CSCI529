using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Security.Cryptography;

public class WWWMgr : IWWWMgr
{

    public delegate void RequestWWWEventDelegate(WWWRequestHandle handler);

    private AssetPool mAssetPool;

    protected Dictionary<string, WWWFileInfo> mFileInfoMap = new Dictionary<string, WWWFileInfo>();
    private static ulong mIncreaseId = 0;
    //private ITime mTime;


    protected Dictionary<string, WWWRequestHandle> mRequestHandleMap = new Dictionary<string, WWWRequestHandle>();
    protected WWWRequestHandleQueue mRequestHandleQueue = new WWWRequestHandleQueue();

    protected Dictionary<string, UnityWebRequest> mWWWObjMap = new Dictionary<string, UnityWebRequest>();

    private WWWRequestHandle[] mPool;
    private int PoolSize = 5;


    public WWWMgr(AssetPool pool)
    {
        Init(pool);
        mPool = new WWWRequestHandle[PoolSize];
    }


    private void Init(AssetPool pool)
    {
        mAssetPool = pool;

        //CheckAllCacheFiles();
    }

    private void UpdateProcess(float dTime)
    {
        //ProcessNew();
    }


    //用来遍历
    //private int CurPoolOffset;
    //protected WWWRequestHandle mWorkingRequest;
    //protected UnityWebRequest mWorkingWww;


    public void Update(float dTime)
    {
        for (int i=0; i < PoolSize; i++)
        {
            WWWRequestHandle mRequestHanlde = mPool[i];
            UnityWebRequest mUnityWebReq = null;
            if (mRequestHanlde != null)
            {
                if (mWWWObjMap.TryGetValue(GetHash(mRequestHanlde.Type, mRequestHanlde.Url), out mUnityWebReq))
                {
                    if(mUnityWebReq.isNetworkError || mUnityWebReq.isHttpError)
                    {
                        mRequestHanlde.TryFailCallback();
                        mPool[i] = null;
                        mWWWObjMap.Remove(GetHash(mRequestHanlde.Type, mRequestHanlde.Url));
                    }
                    else if(mUnityWebReq.isDone)
                    {
                        mRequestHanlde.TrySuccessCallback();
                        Debug.Log(mUnityWebReq.downloadHandler.text);
                        GetHash(mRequestHanlde.Type, mRequestHanlde.Url);
                        //缓存
                        mPool[i] = null;
                    }

                }
                else
                {
                    Debug.Log("no unity web obj found");
                    mRequestHanlde.TryFailCallback();
                    mPool[i] = null;
                }
            }
            else
            {
                ProcessNew(i);
            }

        }
    }

    private void ProcessNew(int poolIdx)
    {
        WWWRequestHandle curHandle = mRequestHandleQueue.Fetch();
        if (curHandle != null)
        {
            string url = curHandle.Url;
            string hash = GetHash(curHandle.Type, curHandle.Url);
            if (mRequestHandleMap.ContainsKey(hash))
            {
                //log
                return;
            }

            if (HasResource(curHandle.Type, curHandle.Url))
            {
                curHandle.TrySuccessCallback();
            }

            UnityWebRequest mWorkingWww = UnityWebRequest.Get(url);
            mWorkingWww.SendWebRequest();

            mWWWObjMap.Add(hash, mWorkingWww);

            mRequestHandleMap.Add(hash, curHandle);
            mPool[poolIdx] = curHandle;
        }
    }

    public IWWWRequestHandle BuildRequest(WWWType type, string url)
    {
        WWWRequestHandle requestHandle;

        requestHandle = new WWWRequestHandle(this);
        requestHandle.SetType(type);
        requestHandle.SetUrl(url);

        return requestHandle;
    }

    public WWWRequestHandle SendRequest(WWWRequestHandle handle)
    {
        string hash = GetHash(handle.Type, handle.Url);
        if (mRequestHandleMap.ContainsKey(hash))
        {
            mRequestHandleMap[hash] = handle;
        }
        mRequestHandleQueue.Push(handle);
        return handle;
    }

    public WWWRequestHandle Abort(WWWRequestHandle handle)
    {
        //string hash = GetHash(handle.Type, handle.Url);
        //if (mRequestHandleMap.ContainsKey(hash))
        //{
        //    mRequestHandleMap.Remove(hash);
        //}

        return handle;
    }

    public T TryLoadFromCache<T>(WWWType type, string url) where T : UnityEngine.Object
    {
        if (mAssetPool != null)
        {
            T t = mAssetPool.Get<T>(url);
            if (t != null)
            {
                return t;
            }
        }

        if (HasResource(type, url))
        {
            WWWFileInfo fileInfo = GetFileInfo(type, url);
            if (fileInfo != null)
            {
                string filePath = fileInfo.localPath;

                byte[] bytes = GameUtils.ReadAllBytesFromFile(filePath);

                if (typeof(T) == typeof(Texture2D))
                {
                    Texture2D tex = GameUtils.GetTextureFromBytes(bytes);

                    tex.Apply(false, true);

                    if (tex != null && mAssetPool != null)
                    {
                        tex.name = url;
                        mAssetPool.AddAsset(url, tex);
                    }

                    return tex as T;
                }

            }
        }

        return default(T);
    }

    public bool HasResource(WWWType type, string url)
    {
        string hash = GetHash(type, url);

        if (mFileInfoMap == null)
        {
            return false;
        }
        if (!mFileInfoMap.ContainsKey(hash))
        {
            return false;
        }
        if (mFileInfoMap[hash] == null)
        {
            return false;
        }
        if (!IsPathExists(mFileInfoMap[hash].localPath))
        {
            return false;

        }

        if (!CheckMd5(type, url))
        {
            return false;
        }

        if (IsExpired(type, url))
        {
            return false;
        }

        return true;
    }

    public bool CheckMd5(WWWType type, string url)
    {
        WWWFileInfo fileIfo = GetFileInfo(type, url);

        if (fileIfo == null)
        {
            return false;
        }
        string nowMd5 = GetFileMd5(fileIfo.localPath);

        return string.Compare(nowMd5, fileIfo.md5, System.StringComparison.OrdinalIgnoreCase) == 0;
    }

    public static bool IsPathExists(string fullpath)
    {
        if (fullpath == null)
        {
            return false;
        }

        return File.Exists(fullpath) || Directory.Exists(fullpath);
    }

    private bool IsExpired(WWWType type, string url)
    {
        string hash = GetHash(type, url);
        if (!mFileInfoMap.ContainsKey(hash))
        {
            return true;
        }
        if (mFileInfoMap[hash].ExpiredTimestamp > GameMain.GetInstance().GetTime())
        {
            return false;
        }
        return true;
    }


    public WWWFileInfo GetFileInfo(WWWType type, string url)
    {
        string hash = GetHash(type, url);
        if (!mFileInfoMap.ContainsKey(hash))
        {
            return null;
        }
        return mFileInfoMap[hash];
    }

    public static string GetHash(WWWType type, string url)
    {
        return GetMd5(type.ToString() + '_' + url);
    }

    public static string GetFileMd5(string filePath)
    {
        try
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open))
            {
                return GetMd5(file);
            }
        }
        catch (Exception ex)
        {
            ;
        }
        return string.Empty;
    }


    public static string GetMd5(string content, bool upper = true)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(content);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }

    public static string GetMd5(Stream fileStream, bool upper = true)
    {
        return "";
    }
}
