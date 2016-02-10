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

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "Interact")
        {
            cur = coll.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        cur = null;
    }

    public void ButtonPressed()
    {
        if (cur == null)
            return;
        cur.SendMessage("Button");
    }
}
