using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{


    Dictionary<GameObject, Pool> prefabPoolMap = new Dictionary<GameObject, Pool>();

    private Dictionary<string, GameObject> prefabPaths = new Dictionary<string, GameObject>();
    public GameObject GetPoolPrefab(string path)
    {
        GameObject prefab = null;
        if (prefabPaths != null)
        {
            prefabPaths.TryGetValue(path, out prefab);
        }

        return prefab;
    }


	public void CachePoolPrefab(string path, GameObject prefab, bool force){
		if (path == string.Empty || prefab == null) {
			return;
		}
		if (!force && prefabPaths.ContainsKey (path)) {
			return;
		}
		prefabPaths [path] = prefab;
	}


    public const int MIN_POOL_SIZE = 3;


    class Pool
    {
        int nextId = 0;

        Stack<GameObject> inactive;
        GameObject prefab;

        public Pool(GameObject prefab, int initialQty)
        {
            this.prefab = prefab;
            inactive = new Stack<GameObject>(initialQty);
        }

        public GameObject Spawn(Vector3 pos, Quaternion rot, out bool isInstantiate)
        {
            GameObject obj;
            isInstantiate = false;
            if (inactive.Count == 0)
            {
                obj = GameObject.Instantiate(prefab, pos, rot) as GameObject;
                obj.name = "(" + "" + ")";
                isInstantiate = true;
            }
            else
            {

                obj = inactive.Pop();
                if (obj == null)
                {
                    return Spawn(pos, rot, out isInstantiate);
                }
            }

            obj.transform.position = pos;
            obj.transform.rotation = rot;

            if (!obj.activeSelf)
            {
                obj.SetActive(true);
            }
            return obj;
        }

        public void Release(GameObject o)
        {
            o.SetActive(false);
            inactive.Push(o);
        }
    }

    public void Release(GameObject prefab, GameObject obj)
    {
        if (prefab == null) return; 
        GeneratePool(prefab, MIN_POOL_SIZE);
        Pool targetPool = prefabPoolMap[prefab];
        targetPool.Release(obj);
    }



    public GameObject Spawn(string pathStr, out bool isInstantiate)
    {
        GameObject prefab = GetPoolPrefab(pathStr);
        if(prefab == null)
        {
            //res loader
            isInstantiate = false;
            return null;
        }
        GeneratePool(prefab, MIN_POOL_SIZE);
        Pool targetPool = prefabPoolMap[prefab];

        GameObject inst = targetPool.Spawn(Vector3.zero, Quaternion.identity, out isInstantiate);
        return inst;
    }


    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, out bool isInstantiate)
    {
        GeneratePool(prefab, MIN_POOL_SIZE);
        Pool targetPool = prefabPoolMap[prefab];

        GameObject inst = targetPool.Spawn(pos, rot, out isInstantiate);

        return inst;
    }

    public void GeneratePool(GameObject prefab = null, int qty = MIN_POOL_SIZE, string strPath = "")
    {
        if (prefab == null || prefabPoolMap.ContainsKey(prefab))
        {
            return;
        }
        prefabPoolMap[prefab] = new Pool(prefab, qty);
    }

}
