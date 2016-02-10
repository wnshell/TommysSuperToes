using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {

    private Vector2 mousePos;

    //Used to figure out the hit box of the buttons
    //Created a square with size of 3x3 and covered up each of the buttons. the position of the square is the vectors below
    float squareSize = 1.5f;
    Vector2 buttonPos = new Vector2( -.14f, -3 );
    Vector2 leftPos = new Vector2(-4.37f, -3);
    Vector2 rightPos = new Vector2(4.06f, -3);

    float mapEdge = 13.5f;

    public GameObject background;

    //speed must be changed in the unity editor, at least for right now.
    public float speed = 5;

    // Use this for initialization
    void Start () {
        background.transform.position = GameManager.S.backPos;
	}
	
	// Update is called once per frame
	void Update () {

        //copied from Foot.cs
        UpdateMousePos();

        
        if(Input.GetMouseButtonDown(0)) //returns true if this is the first frame that the LMB was pressed, good for button presses
        {
            if(mousePos.x >= buttonPos.x - squareSize && mousePos.x <= buttonPos.x + squareSize
                && mousePos.y >= buttonPos.y - squareSize && mousePos.y <= buttonPos.y + squareSize) //Math to check to see if the mouse is inside a hit box for a button
            {
                ButtonPressed();
            }
        }

        if (Input.GetMouseButton(0)) //returns true if LMB is pressed in this frame, good for button holds
        {
            if (mousePos.x >= leftPos.x - squareSize && mousePos.x <= leftPos.x + squareSize
                && mousePos.y >= leftPos.y - squareSize && mousePos.y <= leftPos.y + squareSize)//More math
            {
                GoLeft();
            }
            if (mousePos.x >= rightPos.x - squareSize && mousePos.x <= rightPos.x + squareSize
                && mousePos.y >= rightPos.y - squareSize && mousePos.y <= rightPos.y + squareSize)
            {
                GoRight();
            }
        }
    }

    void UpdateMousePos()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10.0f));
        mousePos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
    }

    void ButtonPressed()
    {
        PlayerController.S.ButtonPressed();
    }

    void GoLeft()
    {
        //updates the background object's position
        //the player character is static, so GoLeft() moves the background right
        Vector2 bpos = background.transform.position;
        bpos.x += speed * Time.deltaTime;
        if (bpos.x > mapEdge)
            bpos.x = mapEdge;
        background.transform.position = bpos;
        GameManager.S.backPos = bpos;
    }

    void GoRight()
    {
        Vector2 bpos = background.transform.position;
        bpos.x -= speed * Time.deltaTime;
        if (bpos.x < -mapEdge)
        {
            bpos.x = -mapEdge;
        }
            
        background.transform.position = bpos;
        GameManager.S.backPos = bpos;
    }

}
