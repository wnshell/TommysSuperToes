using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Vector2 backPos = new Vector2(0, 2.75f);


    public static GameManager S;
    void Awake()
    {
        S = this;
    }
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Tab))
        {
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
