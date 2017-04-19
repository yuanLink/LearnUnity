using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputTextSc : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnSubmit(string text)
	{
		ControllerPage.instance.Submit (GetComponent<InputField>().text);
	}
}
