using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    //what the background position is
    //Updated each frame you move
    public Vector2 backPos = new Vector2(0, 2.75f);


    public static GameManager S;
    void Awake()
    {
        S = this;
    }
	// Use this for initialization
	void Start () {
        //vital for keeping the GameManager in each scene
        DontDestroyOnLoad(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        //Swicthing scenes when tab is pressed, until we implment something else
	    if(Input.GetKeyDown(KeyCode.Tab))
        {
            //switches between overworld and battle scene
            if(Application.loadedLevel == 1)
            {
                Application.LoadLevel(2);
            }
            else
            {
                Application.LoadLevel(1);
            }
        }
	}
}
