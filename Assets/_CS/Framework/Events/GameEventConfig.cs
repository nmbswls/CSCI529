using UnityEngine;
using System.Collections;

public enum EventRet
{
    Continue = 0,
    End, //mao pao chuan bo 终止
    Error,
}

public enum GameEventType
{
    TestEvent = 0,
    SystemType = 1,
    NetType,
}

public enum TestEventId
{
    Test1 = 1,
    Test2 = 2,
}
public enum NetEventId
{
    Test1 = 10,
    Test2 = 20,
}