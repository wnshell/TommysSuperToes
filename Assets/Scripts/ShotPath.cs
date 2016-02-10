using UnityEngine;
using System.Collections;

public class ShotPath : MonoBehaviour {

	public LineRenderer 	line;
	Vector3 				start, end;
	// Use this for initialization
	void Start () {
		line = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		line.SetColors(Color.white, Color.blue);
	}

	public void SetStartPos(Vector3 start_line)
	{
		start = start_line;
		line.SetPosition(0, start);
	}
	public void SetEndPos(Vector3 end_line)
	{
		end = end_line;
		end.z = start.z;
		line.SetPosition(1, end);
	}
	public void HideLine()
	{
		SetEndPos(start);
	}
}
