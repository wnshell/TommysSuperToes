using UnityEngine;
using System.Collections;

public class FootFollow : MonoBehaviour {

	public Transform	foot;
	public Transform	tommy;
	private float		footToTommyDistance;
	private Vector3 	originalCombatPosition;
	private Vector3 	footOffset;
	private bool		followingFoot = false;

	void Awake () {
		Application.targetFrameRate = 30;
	}

	// Use this for initialization
	void Start () {
		originalCombatPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (followingFoot == false)
		{
			KeepFootInView();
		}

		else 
		{
			FollowFoot();
		}
	}

	void KeepFootInView()
	{
		if (foot.position.x >= transform.position.x || foot.position.y >= transform.position.y)
		{
			followingFoot = true;
			footOffset = foot.position - transform.position;
			footToTommyDistance = Vector3.Distance(foot.position, tommy.position);
		}
		else 
			followingFoot = false;
	}

	void FollowFoot()
	{
		if (Vector3.Distance(foot.position, tommy.position) <= footToTommyDistance)
		{
			transform.position = originalCombatPosition;
			followingFoot = false;
		}

		else
		{
		Vector3 newPos = foot.position - footOffset;
		transform.position = newPos;
		}
	}
}
