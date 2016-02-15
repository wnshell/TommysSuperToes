using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Keeps track of if there is currently input or not
public enum InputState
{
	INPUT,
	NOINPUT
}

//Keeps track of what part of the attack tommy's foot is in
public enum AttackState
{
	CHARGING,
	SHOOTING,
	SLOWING,
	RETURNING,
	NORMAL
}

public class Foot : MonoBehaviour {

	//sets up the inputState variable. 
	//can set inputState by inputState = InputState.INPUT (or another state)
	public InputState 	_inputState;
	public InputState	inputState
	{
		get {return _inputState;}
		set 
		{
			if (_inputState == value) return;
			_inputState = value;
			switch(_inputState)
			{
			case InputState.INPUT:
				break;
			case InputState.NOINPUT:
				break;
			default:
				break;
			}
		}
	}

	//sets up the attackState variable
	//can set attackState by attackState = AttackState.CHARGING (or another state)
	public AttackState 	_attackState;
	public AttackState	attackState
	{
		get {return _attackState;}
		set 
		{
			if (_attackState == value) return;
			_attackState = value;
			switch(_attackState)
			{
			case AttackState.NORMAL:
				break;
			case AttackState.CHARGING:
				break;
			case AttackState.SHOOTING:
				originalShotPos = footPos;
				rb.velocity = transform.right * shotSpeedCurrent;
				break;
			case AttackState.SLOWING:
				//print ("slow spot: " + Vector2.Distance(originalShotPos, footPos));
				deceleration = CalculateDeceleration();
				//print ("deceleration: " + deceleration);
				break;
			case AttackState.RETURNING:
				distanceTraveled = 0;
				if (ricochetPoints.Count > 0)
				{
					returnTarget = ricochetPoints[ricochetPoints.Count - 1];
				}
				else 
				{
					returnTarget.x = originalShotPos.x;
					returnTarget.y = originalShotPos.y;
					returnTarget.z = transform.position.z;
				}
				Vector3 dir = transform.position - returnTarget;
				float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

				break;
			default:
				break;
			}
		}
	}

	//CHARGING
	public float		maxChargeRadius; //how far back you can pull the foot before the charge maxes out
	public float		selectFootRadius; //how close you must click to the foot to grab it

	//SHOOTING AND RETURNING
	public float		maxShotDistance; //the maximum distance the foot can travel
	public float		startSlowingDistance; //after how much distance should the foot start to decelerate
	public float		shotSpeedOriginal; //the original speed of the foot after firing it
	private float		shotSpeedCurrent; //the current speed of the foot
	public float		returnAccelRate; //how quickly the foot accelerates when returning to Tommy
	private Vector2		originalShotPos; //where the foot was positioned at the beginning of the shot
	private float		attackStrength; //the charge converted to a decimal [0, 1]
	private float		deceleration; //by how much the foot decelerates at the tip of the attack
	private List<Vector3> ricochetPoints; //the positions where the foot ricochets
	private Vector3		returnTarget;
	private float		distanceTraveled;

	//AIMING
	public GameObject	line; //the line direction indicator prefab
	private ShotPath	shotPath; //the line direction object
	

	//GENERAL
	private Rigidbody2D	rb; //the foot's rigidbody2D component
	private Vector2 	mousePos; //where the mouse is each frame
	private Vector2 	footPos; //where the foot is each fram
	private CombatController	combat;	//the script that keeps track of combat
	public LayerMask	collisionMask;
	private Transform	tommy;



	// Use this for initialization
	void Start () 
	{
		//initialize states and variables
		inputState = InputState.NOINPUT;
		attackState = AttackState.NORMAL;
		rb = GetComponent<Rigidbody2D>();
		shotSpeedCurrent = shotSpeedOriginal;
		combat = GameObject.Find("CombatController").GetComponent<CombatController>();
		shotPath = line.GetComponent<ShotPath>();
		tommy = transform.parent;
		ricochetPoints = new List<Vector3>(); 
	}

	void GetInput ()
	{
		if (inputState == InputState.NOINPUT) //if there is no input
		{
			if (Input.GetMouseButtonDown(0)) //0 for left mouse button, 1 for right, 2 for middle
			{
				//if the mouse is now being pressed
				inputState = InputState.INPUT;
				UpdateMousePos();

				//if you are not attacking and are clicking within selectFootRadius
				if (attackState == AttackState.NORMAL && Vector2.Distance(mousePos, footPos) <= selectFootRadius)
				{
					attackState = AttackState.CHARGING;
				}
			}
		}

		//if there is input
		if (inputState == InputState.INPUT)
		{
			//update the mousePos variable
			UpdateMousePos();

			//if you release the left mouse button
			if (Input.GetMouseButtonUp(0))
			{
				//indicate there is no more input
				inputState = InputState.NOINPUT;

				//determine here if you should fire off the foot or not
				if (attackState == AttackState.CHARGING)
				{
					shotPath.HideLine();
					//attackStrength == -1 when you are trying to fire an illegal shot backward
					if (attackStrength == -1) 
					{
						//set back to normal
						attackState = AttackState.NORMAL;
						transform.rotation = Quaternion.identity;
					}
					else
					{
						//fire off the foot
						attackState = AttackState.SHOOTING;
					}
				}
			}
		}
	}



	void FixedUpdate () 
	{
		if (attackState == AttackState.NORMAL)
		{
		}
		
		else if (attackState == AttackState.CHARGING)
		{
			RotateFoot();
			CalculateAttackStrength();
			//SetShotPath();
			shotPath.Ricochet(transform.position, transform.right, maxShotDistance * attackStrength);
		}
		else if (attackState == AttackState.SHOOTING)
		{
			ShootFoot();
		}
		else if (attackState == AttackState.RETURNING)
		{
			ReturnFoot();
		}
		else if (attackState == AttackState.SLOWING)
		{
			SlowFoot();
		}
		RotationCheck();
	}

	void Update()
	{
		if (attackState == AttackState.SHOOTING || attackState == AttackState.SLOWING)
		{
			DistanceTraveled();
		}
		footPos = new Vector2 (transform.position.x, transform.position.y);
		
		if (combat.turn == TurnState.TOMMY)
		{
			GetInput();
		}
	}

	//updates the mousePos variable so you always knows where the mouse is on the screen
	void UpdateMousePos()
	{
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, -10.0f));
		mousePos = new Vector2 (mouseWorldPos.x, mouseWorldPos.y);
	}

	//rotates the foot so it is facing the opposite direction of the mouse.
	//Some of this code is taken from online
	void RotateFoot() 
	{
		Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
		diff.Normalize();
		
		float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

		rot_z = diff.x <= 0 ? rot_z - 180 : rot_z;

		transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
	}

	//sets attackStrength to a float 0-1 to represent to attack strength, 0 being no power, 1 being full power
	void CalculateAttackStrength() 
	{
		float distanceFromTouchToFoot = Vector2.Distance(mousePos, footPos);

		float strength = distanceFromTouchToFoot > maxChargeRadius ? 1 : distanceFromTouchToFoot / maxChargeRadius;
		attackStrength = mousePos.x > transform.position.x ? -1 : strength;

	}

	//once the foot has reached a certain point the state will change to SLOWING
	void ShootFoot() 
	{

		if (distanceTraveled >= attackStrength * maxShotDistance * startSlowingDistance)
		{
			//some debugging prints I'm using
//			print ("How far it should go: " + maxShotDistance * attackStrength);
//			print ("Where it is slowing down " + Vector2.Distance(footPos, originalShotPos));
//			print ("attackStrength: " + attackStrength);
//			print ("slowingDistance * attackStrength: " + startSlowingDistance * attackStrength);
			attackState = AttackState.SLOWING;
		}
	}

	//every frame subtracts from the foot's velocity until it hits near zero (Mathf.Epsilon)
	void SlowFoot()
	{
		//experimenting with some stuff
//		|| Vector2.Distance(footPos, originalShotPos) >= maxShotDistance * attackStrength
		if (shotSpeedCurrent <= Mathf.Epsilon)
		{
			attackState = AttackState.RETURNING;
		}
		else
		{
			shotSpeedCurrent -= deceleration;
			rb.velocity = transform.right * shotSpeedCurrent;
		}
	}

	//Remember those physics equations, like x = x0 + v * 2*a that shit? This is that shit.
	//based on how far there is left to travel and current velocity, this calculates the rate that the velocity should decelerate to 
	//get the rest of the distance and end at a velocity of 0
	float CalculateDeceleration()
	{
		return (((shotSpeedOriginal * shotSpeedOriginal) / (2 * (maxShotDistance - Vector2.Distance(footPos, originalShotPos)) * attackStrength)) * .04f);
	}

	//
	void ReturnFoot()
	{
		if (ricochetPoints.Count > 0)
		{
			//print ((transform.position - ricochetPoints[ricochetPoints.Count - 1]).magnitude);
			if ((transform.position - ricochetPoints[ricochetPoints.Count - 1]).magnitude < .05f)
			{
				ricochetPoints.RemoveAt(ricochetPoints.Count - 1);
				if (ricochetPoints.Count > 0)
				{
					returnTarget = ricochetPoints[ricochetPoints.Count - 1];
					Vector3 dir = transform.position - returnTarget;
					float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
					transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
				}
				else 
				{

					returnTarget.x = originalShotPos.x;
					returnTarget.y = originalShotPos.y;
					returnTarget.z = transform.position.z;
					Vector3 dir = transform.position - returnTarget;
					float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
					transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
				}
			}
		}

		shotSpeedCurrent = Mathf.Lerp (shotSpeedCurrent, shotSpeedOriginal * 2, returnAccelRate * Time.deltaTime);
		transform.position = Vector3.MoveTowards(transform.position, returnTarget, shotSpeedCurrent * Time.deltaTime);
		//rb.velocity = -transform.right * shotSpeedCurrent;

		//once the foot is back to its original position
		if ((footPos - originalShotPos).magnitude < .05f)
		{
			Vector3 returnPosition = new Vector3(originalShotPos.x, originalShotPos.y, -1);
			transform.position = returnPosition;

			//reset velocity
			rb.velocity = Vector2.zero;
			//reset foot rotation
			transform.rotation = Quaternion.identity;
			//reset shot speed
			shotSpeedCurrent = shotSpeedOriginal;
			//reset Tommy to his neutral state
			attackState = AttackState.NORMAL;

			//tell combat controller that tommy has finished his attack
			combat.TommyEnd();
		}
	}


	//set the size of the shot indicator
	void SetShotPath()
	{
		shotPath.SetStartPos(transform.position);
		shotPath.SetEndPos(transform.position + (transform.right * maxShotDistance * attackStrength));
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.layer == 8 && (attackState == AttackState.SHOOTING || attackState == AttackState.SLOWING))
		{
			ricochetPoints.Add(transform.position);
			Ray2D ray = new Ray2D(footPos, transform.right);
			Vector2 right = new Vector2(transform.right.x, transform.right.y);

			RaycastHit2D hit = Physics2D.Raycast(footPos, right, 100 /*Time.deltaTime * shotSpeedCurrent*/, collisionMask);
			if (hit.collider != null) {
				Vector2 ricochetDir = Vector2.Reflect(ray.direction, hit.normal);
				float rot = Mathf.Atan2(ricochetDir.y, ricochetDir.x) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3(0, 0, rot);
				rb.velocity = transform.right * shotSpeedCurrent;
			}
		}
	}

	//makes sure there is no rotation in the y or x directions
	void RotationCheck()
	{
		transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
	}

	void DistanceTraveled()
	{
		Vector2 newPosition = new Vector2(transform.position.x, transform.position.y);
		distanceTraveled += Vector2.Distance(newPosition, footPos);
	}



}
