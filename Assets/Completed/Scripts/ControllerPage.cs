using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LuaInterface;

public class ControllerPage : MonoBehaviour {

	public static ControllerPage instance;

	public GameObject ControllerCavans;
	public Text inputBox, outputBox;


	public bool isScripting = false;
	private LuaState luasupport;
	private string pre_code = "luanet.load_assembly('UnityEngine');\n" +
		"GameObject = luanet.import_type('UnityEngine.GameObject');\n" +
		"Player = luanet.import_type('Player');\n" +
		"Enemy = luanet.import_type('Enemy');\n" +
		"BagSystem = luanet.import_type('BagSystem');\n" +
		"GameManager = luanet.import_type('Completed.GameManager');\n" +
		"local bag = BagSystem.instance;\n" +
		"local player = GameObject.Find(\"Player\"):GetComponent(\"Player\");\n" +
		"local game = GameManager.instance;";

	void Awake(){
		luasupport = new LuaState ();
		//Check if there is already an instance
		if (instance == null)
			//if not, set it to this.
			instance = this;
		//If instance already exists:
		else if (instance != this)
			//Destroy this, this enforces our singleton pattern so there can only be one instance.
			Destroy (gameObject);

		//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
		DontDestroyOnLoad (gameObject);
	}

	// Use this for initialization
	void Start () {
		
	}

	public void initialize()
	{
		ControllerCavans = GameObject.Find ("Controller");
		inputBox = GameObject.Find ("InputText").GetComponent<Text>();
		outputBox = GameObject.Find ("OutputBox").GetComponent<Text>();

		ControllerCavans.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (!isScripting)
			return;
		if (Input.GetKey (KeyCode.Escape)) {
			isScripting = false;
			outputBox.text = "Lua System\n";
			ControllerCavans.SetActive (false);
		}
	}

	public void Open()
	{
		isScripting = true;
		ControllerCavans.SetActive (true);
	}

	public void Submit(string text)
	{
		string code = pre_code + text;
		outputBox.text += ">" + text + "\n";
		try{
		luasupport.DoString (code);
		}catch(LuaException e){
			outputBox.text += "LuaState:"+e.Message+"\n";
		}
	}
}
