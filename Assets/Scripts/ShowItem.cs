using UnityEngine;
using System.Collections;

public class ShowItem : MonoBehaviour {

	public GameObject item;
	public GameObject text;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		print ("triggered");
		item.SetActive (true);
	}

	void OnTriggerExit2D(Collider2D other) {
		item.SetActive (false);
		text.SetActive (false);
	}

	void Button() {
		text.SetActive (!text.active);
		print ("I need an item!");
	}
}
