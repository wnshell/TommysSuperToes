using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TurnState
{
	TOMMY,
	ENEMY,
	KNOCKBACK,
	OVERWORLD
}

public class TurnController : MonoBehaviour {

	//static public TurnController S;

	public TurnState _turn;
	public TurnState turn
	{
		get {return _turn;}
		set 
		{
			if (_turn == value) return;
			_turn = value;
			switch(_turn)
			{
			case TurnState.TOMMY:
				break;
			case TurnState.ENEMY:
				break;
			case TurnState.KNOCKBACK:
				break;
			case TurnState.OVERWORLD:
				break;
			default:
				break;
			}
		}
	}

	public List<Ninja> ninjaList;

	void Awake () {
		TommysTurn();
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (turn == TurnState.ENEMY && NinjasDoneMoving())
		{
			TommysTurn();
		}
	}

	public void TommysTurn()
	{
		turn = TurnState.TOMMY;
	}
	public void EnemysTurn()
	{
		turn = TurnState.ENEMY;
	}

	bool NinjasDoneMoving()
	{
		for (int i = 0; i < ninjaList.Count; i += 1)
		{
			if (ninjaList[i].jumpState != JumpState.GROUNDED)
			{
				return false;
			}
		}
		return true;
	}






}
