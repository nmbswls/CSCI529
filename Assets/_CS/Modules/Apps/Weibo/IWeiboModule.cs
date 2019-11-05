using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IWeiboModule : IModule
{
    int GetCurrentTurnShuaTime();
    void ReduceShuaTime();
    string randomTime();
}
