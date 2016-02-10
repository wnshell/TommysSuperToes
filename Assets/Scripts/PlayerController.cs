using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public GameObject cur = null;

    public static PlayerController S;
    void Awake()
    {
        S = this;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //If the player collides with an interactable object, saves it in cur
    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "Interact")
        {
            cur = coll.gameObject;
        }
    }

    //If you are no longer in the range of an object, remove it from cur
    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Interact")
        {
            cur = null;
        }
    }

    //lets Cur know that the button was pushed
    //Dosen't do anything if cur == null, i.e. you are not near an object
    public void ButtonPressed()
    {
        if (cur == null)
            return;
        cur.SendMessage("Button");
    }
}
