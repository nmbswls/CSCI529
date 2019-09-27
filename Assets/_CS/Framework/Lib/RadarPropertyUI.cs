using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;


public class RadarPropertyUI : MonoBehaviour
{

	Texture2D m_texture;
	int[] pixels;


	public int NumPoint = 5;
	List<Image> points = new List<Image>();
	float maxV = 30;
	float minV = 0;

	float radius = 60;

	InnerNpolyDrawer innerG;

	void Start(){
		
		//Setup ();
		//SetPointValues (new int[]{5,9,20,5,9});
	}

	public void Setup(){

		points.Clear ();

		foreach (Transform child in transform.GetChild(0)) {
			points.Add (child.GetComponent<Image>());
		}

		innerG = GetComponentInChildren<InnerNpolyDrawer> ();

	}

	private float ClampValue(float v){
		float newV = v;
		if (newV > maxV) {
			newV = maxV;
		}
		if (newV < minV)
		{
			newV = minV;
		}
		return newV;
	}

	public void SetPointValues(int[] values){
		if (values.Length != NumPoint) {
			return;
		}



		Vector2[] positions = new Vector2[values.Length];

		for (int i = 0; i < values.Length; i++) {
			var angle = i*2*Mathf.PI/NumPoint;
			Vector2 diffVec = new Vector2 (radius * Mathf.Sin (angle) * ClampValue(values[i]) / maxV, radius * Mathf.Cos (angle) * ClampValue(values[i]) / maxV);
			points [i].rectTransform.anchoredPosition = diffVec;
			positions [i] = diffVec;
		}
		if (innerG != null) {
			innerG.SetShownValue (positions);
		}

	}
		
}