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

	public List<Transform>  jumpPoints;
	public int				jumpPointNumber;
	public int				currentJumpPoint;
	public float			jumpSpeed;
	private TextMesh		turnCounter;
	private CombatController combat;
	public ParticleSystem	explosion;

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

	void JumpForward()
	{
		transform.position = Vector3.MoveTowards(transform.position, jumpPoints[currentJumpPoint].position, jumpSpeed * Time.deltaTime);
		if (transform.position == jumpPoints[currentJumpPoint].position)
		{
			jumpState = JumpState.GROUNDED;
		}
	}

	void Knockback()
	{
		transform.position = Vector3.MoveTowards(transform.position, jumpPoints[currentJumpPoint].position, jumpSpeed * 2 * Time.deltaTime);
		if (transform.position == jumpPoints[currentJumpPoint].position)
		{
			jumpState = JumpState.GROUNDED;
		}
	}

	
	void OnTriggerEnter2D (Collider2D coll)
	{
		if (jumpState == JumpState.GROUNDED &&
		    coll.gameObject.tag == "Foot" && coll.gameObject.GetComponent<Foot>().attackState == AttackState.SHOOTING)
		{
			Foot foot = coll.gameObject.GetComponent<Foot>();
			Rigidbody2D footRB = coll.gameObject.GetComponent<Rigidbody2D>();

			if (footRB.velocity.magnitude / foot.shotSpeedOriginal <= .3f)
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
