using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum JumpState
{
	GROUNDED,
	FORWARD,
	BACKWARD
}

public class Ninja : MonoBehaviour {

	public JumpState 		_jumpState;
	public JumpState		jumpState
	{
		get {return _jumpState;}
		set 
		{
			if (_jumpState == value) return;
			_jumpState = value;
			switch(_jumpState)
			{
			case JumpState.GROUNDED:
				break;
			case JumpState.FORWARD:
				currentJumpPoint -= 1;
				break;
			case JumpState.BACKWARD:
				currentJumpPoint += 1;
				currentJumpPoint = Mathf.Clamp(currentJumpPoint, 0, jumpPointNumber - 1);
				break;
			default:
				break;
			}
		}
	}

	public List<Transform>  jumpPoints;
	public int				jumpPointNumber;
	public int				currentJumpPoint;
	public float			jumpSpeed;

	// Use this for initialization

	void Start () 
	{
		jumpState = JumpState.GROUNDED;
		jumpPointNumber = jumpPoints.Count;
		currentJumpPoint = jumpPointNumber - 1;
		for (int i = 0; i < jumpPointNumber; i += 1)
		{
			Vector3 temp = jumpPoints[i].position;
			temp.z = transform.position.z;
			jumpPoints[i].position = temp;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.J))
		{
			jumpState = JumpState.FORWARD;
		}
		if (jumpState == JumpState.FORWARD)
		{
			ChangeJumpPoint();
		}
	}

	void ChangeJumpPoint()
	{
		transform.position = Vector3.MoveTowards(transform.position, jumpPoints[currentJumpPoint].position, jumpSpeed * Time.deltaTime);
		if (transform.position == jumpPoints[currentJumpPoint].position)
		{
			jumpState = JumpState.GROUNDED;
		}
	}

	void OnTriggerEnter2D (Collider2D coll)
	{
		if (coll.gameObject.tag == "Foot")
		{
			Foot foot = coll.gameObject.GetComponent<Foot>();
			Rigidbody2D footRB = coll.gameObject.GetComponent<Rigidbody2D>();

			if ((foot.attackState == AttackState.SHOOTING) 
			    && footRB.velocity.magnitude / foot.shotSpeedOriginal <= .3f)
			{
				print ("it's a hit");
			}
		}
	}
}
