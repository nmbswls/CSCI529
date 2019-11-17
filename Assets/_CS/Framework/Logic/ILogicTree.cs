using UnityEngine;
using System.Collections;

public interface ILogicTree : IModule
{

	LogicNode ConstructFromString (string str);
}

