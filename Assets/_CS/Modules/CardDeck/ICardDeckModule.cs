using UnityEngine;
using System.Collections;

public interface ICardDeckModule : IModule
{
	void GainNewCard(string cid);
}
