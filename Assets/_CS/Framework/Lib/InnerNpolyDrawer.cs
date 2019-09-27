using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InnerNpolyDrawer : Graphic
{

	[Header("坐标轴显示属性")]
	public float lineWidth = 3;
	public Vector2 offset = new Vector2(0, 0);


	Vector2[] pixelPoints;
	bool needReDraw = false;



	public void SetShownValue(Vector2[] pixelPoints){

		this.pixelPoints = pixelPoints;
		needReDraw = true;
		this.SetVerticesDirty ();
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		SetData (vh);
	}

	/// <summary>
	/// 设置顶点属性
	/// </summary>
	private void SetData(VertexHelper vh)
	{
		
		if (!needReDraw) {
			return;
		}	

		List<UIVertex> targetVertexList = new List<UIVertex>();
		int triangleCount = pixelPoints.Length - 1;
		//三角形 构成
		for (int i = 0; i < triangleCount; i++)
		{
			for (int j = 0; j < 3; j++)     //三角形的三个点
			{
				UIVertex vertex = new UIVertex();
				if (j == 0)
				{
					vertex.position = Vector2.zero;
				}
				else
				{
					vertex.position = pixelPoints[i + j -1];

				}
				vertex.color = color;
				targetVertexList.Add(vertex);
			}
		}
		//最后一个三角形 051 
		for (int k = 0; k < 3; k++)
		{
			UIVertex vertex = new UIVertex();
			if (k == 0)
			{
				vertex.position = Vector2.zero;
			}
			else if (k == 1)
			{
				vertex.position = pixelPoints[0];
			}
			else if (k == 2)
			{
				vertex.position = pixelPoints[4];
			}
			vertex.color = color;

			targetVertexList.Add(vertex);
		}
		vh.AddUIVertexTriangleStream(targetVertexList);

		UIVertex[] verts = new UIVertex[4];
		for (int i = 0; i < verts.Length; i++)
		{
			verts[i].color = Color.black;
		}

		for (int i = 0; i < pixelPoints.Length; i++)
		{
			SetVerts(pixelPoints[i], pixelPoints[(i+1)%pixelPoints.Length], lineWidth, verts);
			vh.AddUIVertexQuad(verts);
		}
		needReDraw = false;
	}

	// 设置UI顶点数据
	void SetVerts(Vector2 _start, Vector2 _end, float _width, UIVertex[] _verts)
	{
		Vector2[] tmp = GetRect(_start, _end, _width);	 
		_verts[0].position = tmp[0];
		_verts[1].position = tmp[1];
		_verts[2].position = tmp[3];
		_verts[3].position = tmp[2];
	}

	// 获取两点组成的矩形边框, 起始点，终止点，宽度
	Vector2[] GetRect(Vector2 _start, Vector2 _end, float _width)
	{
		Vector2[] rect = new Vector2[4];
		Vector2 dir = GetHorizontalDir(_end - _start);	// 获取水平向右的向量
		rect[0] = _start + dir * _width;
		rect[1] = _start - dir * _width;
		rect[2] = _end + dir * _width;
		rect[3] = _end - dir * _width;
		return rect;
	}

	Vector2 GetHorizontalDir(Vector2 input){
		return new Vector2(input.y,-input.x).normalized;
	}

}

