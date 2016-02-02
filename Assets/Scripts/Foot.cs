using UnityEngine;
using System.Collections;

public enum InputState
{
	INPUT,
	NOINPUT
}

public enum AttackState
{
	CHARGING,
	SHOOTING,
	RETURNING,
	NORMAL
}

public class Foot : MonoBehaviour {

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
				ChargeIndicatorColor(Color.green);
				break;
			case AttackState.CHARGING:
				break;
			case AttackState.SHOOTING:
				originalShotPos = footPos;
				break;
			case AttackState.RETURNING:
				break;
			default:
				break;
			}
		}
	}

	//CHARGING
	public float		maxChargeRadius;
	public float		selectFootRadius;
	private GameObject	chargeIndicator;

	//SHOOTING AND RETURNING
	public float		maxShotDistance;
	public float		shotSpeedOriginal;
	private float		shotSpeedCurrent;
	public float		shotDecelRate;
	public float		shotAccelRate;
	private Vector2		originalShotPos;
	private float		attackStrength;

	//GROWING
	private float		minGrowScale;
	public float		maxGrowScale;
	public float		growRate;

	//GENERAL
	private Rigidbody2D	rb;
	private Vector2 	mousePos;
	private Vector2 	footPos;
	
	void Awake () {
		chargeIndicator = transform.Find ("Point").gameObject;
	}

	// Use this for initialization
	void Start () 
	{
		inputState = InputState.NOINPUT;
		attackState = AttackState.NORMAL;
		rb = GetComponent<Rigidbody2D>();
		shotSpeedCurrent = shotSpeedOriginal;
		ChargeIndicatorColor (Color.green);
		minGrowScale = transform.localScale.x;
	}

	void GetInput ()
	{
		if (inputState == InputState.NOINPUT)
		{
			if (Input.GetMouseButtonDown(0)) //0 for left mouse button, 1 for right, 2 for middle
			{
				inputState = InputState.INPUT;
				UpdateMousePos();
				if (attackState == AttackState.NORMAL && Vector2.Distance(mousePos, footPos) <= selectFootRadius)
				{
					attackState = AttackState.CHARGING;
				}
			}
		}

		if (inputState == InputState.INPUT)
		{
			UpdateMousePos();

			if (Input.GetMouseButtonUp(0))
			{
				inputState = InputState.NOINPUT;

				if (attackState == AttackState.CHARGING)
				{
					if (attackStrength == -1) 
					{
						attackState = AttackState.NORMAL;
						transform.rotation = Quaternion.identity;
					}
					else
					{
						attackState = AttackState.SHOOTING;
					}
				}
			}
		}

	}
	
	// Update is called once per frame
	void Update () 
	{

		footPos = new Vector2 (transform.position.x, transform.position.y);
		GetInput();

		if (attackState == AttackState.CHARGING)
		{
			RotateFoot();
			CalculateAttackStrength();
			DecideChargeIndicatorColor();
		}
		else if (attackState == AttackState.SHOOTING)
		{
			ShootFoot();
		}
		else if (attackState == AttackState.RETURNING)
		{
			ReturnFoot();
		}

		if (Input.GetKey(KeyCode.Space))
		{
			QuickGrow();
		}
		if (Input.GetKey (KeyCode.X))
		{
			QuickShrink();
		}
	}

	void UpdateMousePos()
	{
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, -10.0f));
		mousePos = new Vector2 (mouseWorldPos.x, mouseWorldPos.y);
	}

	void RotateFoot() 
	{
		Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
		diff.Normalize();
		
		float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

		rot_z = diff.x <= 0 ? rot_z - 180 : rot_z;

		transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
	}

	//returns a number 0-1 to represent to attack strength, 0 being no power, 1 being full power
	void CalculateAttackStrength() 
	{
		float distanceFromTouchToFoot = Vector2.Distance(mousePos, footPos);

		float strength = distanceFromTouchToFoot > maxChargeRadius ? 1 : distanceFromTouchToFoot / maxChargeRadius;
		attackStrength = mousePos.x > transform.position.x ? -1 : strength;

	}

	void ShootFoot() 
	{
		if (Vector2.Distance(footPos, originalShotPos) >= maxShotDistance * attackStrength * attackStrength)
		{
			shotSpeedCurrent = Mathf.Lerp(shotSpeedCurrent, 0, shotDecelRate * Time.deltaTime);
		}

		rb.velocity = transform.right * shotSpeedCurrent;

		if (shotSpeedCurrent < .1f)
		{
			attackState = AttackState.RETURNING;
		}
	}

	void ReturnFoot()
	{
		shotSpeedCurrent = Mathf.Lerp (shotSpeedCurrent, shotSpeedOriginal, shotAccelRate * Time.deltaTime);
		rb.velocity = -transform.right * shotSpeedCurrent;

		if (footPos == originalShotPos || footPos.x < originalShotPos.x)
		{
			Vector3 returnPosition = new Vector3(originalShotPos.x, originalShotPos.y, -1);
			transform.position = returnPosition;

			rb.velocity = Vector2.zero;
			transform.rotation = Quaternion.identity;
			shotSpeedCurrent = shotSpeedOriginal;
			attackState = AttackState.NORMAL;
		}
	}

	void DecideChargeIndicatorColor()
	{
		if (attackStrength < .333f)
			ChargeIndicatorColor(Color.green);
		else if (attackStrength < .666f)
			ChargeIndicatorColor(Color.yellow);
		else
			ChargeIndicatorColor(Color.red);
	}

	void ChargeIndicatorColor(Color c)
	{
		chargeIndicator.GetComponent<Renderer>().material.color = c;
	}

	void QuickGrow()
	{
		if (transform.localScale.x < maxGrowScale)
		{
			Vector3 growIncrement = transform.localScale;
			growIncrement.x = growIncrement.y = growIncrement.x + growRate;
			transform.localScale = growIncrement;
		}
	}

	void QuickShrink()
	{
		if (transform.localScale.x > minGrowScale)
		{
			Vector3 shrinkIncrement = transform.localScale;
			shrinkIncrement.x = shrinkIncrement.y = shrinkIncrement.x - growRate;
			transform.localScale = shrinkIncrement;
		}
	}

}
