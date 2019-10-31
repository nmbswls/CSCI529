using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ISpeEventMgr : IModule
{

    List<SpecialEvent> CheckEvent();

    void RemoveListener(string eventId);

    void AddListener(string eventId);
}

