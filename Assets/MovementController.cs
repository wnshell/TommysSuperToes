using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {

    private Vector2 mousePos;
    float squareSize = 1.5f;
    Vector2 buttonPos = new Vector2( -.14f, -3 );
    Vector2 leftPos = new Vector2(-4.37f, -3);
    Vector2 rightPos = new Vector2(4.06f, -3);



    public GameObject background;
    public float speed = 1;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        UpdateMousePos();

        if(Input.GetMouseButtonDown(0))
        {
            if(mousePos.x >= buttonPos.x - squareSize && mousePos.x <= buttonPos.x + squareSize
                && mousePos.y >= buttonPos.y - squareSize && mousePos.y <= buttonPos.y + squareSize)
            {
                ButtonPressed();
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (mousePos.x >= leftPos.x - squareSize && mousePos.x <= leftPos.x + squareSize
                && mousePos.y >= leftPos.y - squareSize && mousePos.y <= leftPos.y + squareSize)
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

    }

    void GoLeft()
    {
        Vector2 bpos = background.transform.position;
        bpos.x += speed * Time.deltaTime;
        background.transform.position = bpos;
    }

    void GoRight()
    {
        Vector2 bpos = background.transform.position;
        bpos.x -= speed * Time.deltaTime;
        background.transform.position = bpos;
    }

}
