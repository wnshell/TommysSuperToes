using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public enum JumpState
{
	GROUNDED,
	FORWARD,
	KNOCKBACK
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
				turnCounter.text = currentJumpPoint.ToString ();
				break;
			case JumpState.KNOCKBACK:
				currentJumpPoint += 2;
				currentJumpPoint = Mathf.Clamp(currentJumpPoint, 0, jumpPointNumber - 1);
				turnCounter.text = currentJumpPoint.ToString ();
				break;
			default:
				break;
			}
		}
	}

	public List<Transform>  jumpPoints; //a list of empty Transforms for the ninja to jump to
	public int				jumpPointNumber; //the total number of jump points for this ninja
	public int				currentJumpPoint; //which jump point the ninja is currently on
	public float			jumpSpeed; //the speed to jump between positions
	private TextMesh		turnCounter; //the number that indicates how many spots the ninja is from Tommy
	private CombatController combat; //the combat script that keeps track of the combat flow
	public ParticleSystem	explosion; //the particle system that makes the ninja explode

	// Use this for initialization

	void Start () 
	{
		//initialize variables
		jumpState = JumpState.GROUNDED;
		jumpPointNumber = jumpPoints.Count;
		currentJumpPoint = jumpPointNumber - 1;
		for (int i = 0; i < jumpPointNumber; i += 1)
		{
			Vector3 temp = jumpPoints[i].position;
			temp.z = transform.position.z;
			jumpPoints[i].position = temp;
		}
		turnCounter = GetComponentInChildren<TextMesh>();
		turnCounter.text = currentJumpPoint.ToString ();

		combat = GameObject.Find("CombatController").GetComponent<CombatController>();
	}
	
	// Update is called once per frame
	void Update () 
	{

		if (combat.turn == TurnState.ENEMYSTART)
		{
			jumpState = JumpState.FORWARD;
		}
		if (jumpState == JumpState.FORWARD)
		{
			JumpForward();
		}
		else if (jumpState == JumpState.KNOCKBACK)
		{
			Knockback();
		}
	}

	//move Ninja to next jump point
	void JumpForward()
	{
		transform.position = Vector3.MoveTowards(transform.position, jumpPoints[currentJumpPoint].position, jumpSpeed * Time.deltaTime);
		if (transform.position == jumpPoints[currentJumpPoint].position)
		{
			jumpState = JumpState.GROUNDED;
		}
	}

	//move ninja to a previous jump point
	void Knockback()
	{
		transform.position = Vector3.MoveTowards(transform.position, jumpPoints[currentJumpPoint].position, jumpSpeed * 2 * Time.deltaTime);
		if (transform.position == jumpPoints[currentJumpPoint].position)
		{
			jumpState = JumpState.GROUNDED;
		}
	}

	//when hit by the foot or Tommy (not currently working, which is fine; easy fix)
	void OnTriggerEnter2D (Collider2D coll)
	{
		if (jumpState == JumpState.GROUNDED &&
		    coll.gameObject.tag == "Foot" && coll.gameObject.GetComponent<Foot>().attackState == AttackState.SHOOTING)
		{
			Foot foot = coll.gameObject.GetComponent<Foot>();
			Rigidbody2D footRB = coll.gameObject.GetComponent<Rigidbody2D>();

			if (footRB.velocity.magnitude / foot.shotSpeedOriginal <= .3f || true)
			{
				Instantiate(explosion, transform.position, Quaternion.identity);
				Destroy(this.gameObject);
			} 
			else
			{
				jumpState = JumpState.KNOCKBACK;
			}
		}
		else if (coll.gameObject.tag == "Player")
		{
			jumpState = JumpState.KNOCKBACK;
		}
	}
}
