using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LuaInterface;

public class ModManagerC : MonoBehaviour {

	public bool isOpenning = false;

	public string configfilename = "config.lua";
	LuaState luasupport;
	public GameObject ModManagerUI;
	public Text output;
	public List<string> modFilename = new List<string>();
	public List<bool> modEnable = new List<bool>();
	LuaTable bundles;

	private int choose_index = 0;
	private bool pressed = false;

	void Awake()
	{
		luasupport = new LuaState ();
		ModManagerUI = GameObject.Find ("ModManager");
		output = GameObject.Find ("ModOutput").GetComponent<Text> ();
		ModManagerUI.SetActive (false);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!isOpenning)
			return;
		if (Input.GetKey (KeyCode.Escape)) {
			ModManagerUI.SetActive (false);
			isOpenning = false;
		}

		int vertical;
		vertical = (int) (Input.GetAxisRaw ("Vertical"));

		if (vertical > 0) {
			if (!pressed) {
				pressed = true;
				choose_index -= vertical;
				if (choose_index < 0)
					choose_index += vertical;
				Print ();
			}
		} else if (vertical < 0) {
			if (!pressed) {
				pressed = true;
				choose_index -= vertical;
				if (choose_index >= modFilename.Count)
					choose_index += vertical;
				Print ();
			}
		} else if (Input.GetKey(KeyCode.Space)){
			if (!pressed) {
				pressed = true;
				modEnable [choose_index] = !modEnable [choose_index];
				WriteConfig ();
				Open ();
			}
		}else{
			pressed = false;
		}
	}

	void LoadModList()
	{
		modFilename.Clear ();
		string path = Application.streamingAssetsPath + "/" + configfilename;
		WWW tl = new WWW ("file://" + path);
		while (!tl.isDone); 
		luasupport.DoString(tl.text);
		bundles = luasupport.GetTable ("bundles_name");

		string[] ret = Directory.GetFiles (Application.streamingAssetsPath + "/bundles/");
		for (int i = 0; i < ret.Length; ++i) {
			string bundlepath = "file://" + ret[i];
			WWW loader = new WWW(bundlepath); // 加载bundle
			AssetBundle bundle = loader.assetBundle;
			if (bundle == null)
				continue;
			TextAsset menufile = bundle.LoadAsset<TextAsset> ("bundle_list"); // 加载bundle列表
			bundle.Unload(false);
			if (menufile != null) {
				string tmp = "";
				int j;
				for (j = ret [i].Length - 1; j >= 0; --j)
					if (ret [i] [j] == '/')
						break;
				++j;
				for (; j < ret [i].Length; ++j)
					tmp += ret [i] [j];
				modFilename.Add (tmp);
			}
		}

		foreach (string filename in bundles.Values) {
			for (int i =0;i < modFilename.Count;++i)
				if (filename == modFilename[i]) {
					modFilename.RemoveAt (i);
				}
		}
	}

	void Print()
	{
		output.text = "Mod Manager\n" +
			"The list of mod:\n";
		for (int i =0;i < modFilename.Count;++i)
		{
			if (i == choose_index)
				output.text += ">" + modFilename [i];
			else
				output.text += " " + modFilename [i];
			if (modEnable[i])
				output.text += "(Enabled)\n";
			else
				output.text += "(Disabled)\n";
		}
	}

	void WriteConfig()
	{
		FileStream r = new FileStream(Application.streamingAssetsPath + "/" + configfilename, FileMode.Create);
		StreamWriter sr = new StreamWriter (r);
		sr.WriteLine ("bundles_name = {");
		for (int i = 0, j = 0; i < modFilename.Count; ++i) {
			if (modEnable[i]) {
				if (j != 0)
					sr.WriteLine (",");
				string s = "\"" + modFilename [i] + "\"";
				sr.Write (s);
				++j;
			}
		}
		sr.Write ("};");
		sr.Close ();
		sr.Dispose ();
	}

	public void Open()
	{
		pressed = true;
		isOpenning = true;
		ModManagerUI.SetActive (true);
		LoadModList ();
		modEnable.Clear ();
		output.text = "Mod Manager\n" +
		"The list of mod:\n";

		foreach (string filename in modFilename) {
			modEnable.Add (false);
		}

		foreach (string filename in bundles.Values) {
			modFilename.Insert (0, filename);
			modEnable.Insert (0, true);
		}

		Print ();
	}
}
