using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuProcess : MonoBehaviour {

	// CG段

	public bool isCGPlaying = true;
	public Sprite CG1, CG2, CG3;
	public Image CGImage;
	public Text textContent;
	UIEffect effectManager;
	private GameObject CGUI, TextUI, CGCanvas;
	private int CGProcess;
	private float cur_time = 0;

	// Menu段
	public int choose_index = 0;
	public Text startC, modC, exitC;

	private bool pressed = false;

	void Awake()
	{
		CGUI = GameObject.Find ("CG");
		TextUI = GameObject.Find ("Text");
		CGCanvas = GameObject.Find ("CGDisplay");
		effectManager = GetComponent<UIEffect> ();
		CGImage = CGUI.GetComponent<Image> ();
		textContent = TextUI.GetComponent<Text> ();
		startC = GameObject.Find ("Start").GetComponent<Text> ();
		modC = GameObject.Find ("Mod").GetComponent<Text> ();
		exitC = GameObject.Find ("Exit").GetComponent<Text> ();
		CGProcess = 0;
	}

	// Use this for initialization
	void Start () {
		isCGPlaying = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<ModManagerC> ().isOpenning)
			return;
		if (isCGPlaying) {
			cur_time += Time.deltaTime;
			CGPlay ();

			if (Input.GetKey (KeyCode.Space)) {
				pressed = true;
				CGProcess = 10;
			} else
				pressed = false;
			return;
		}

		startC.text = " Start";
		modC.text = " Mod Manager";
		exitC.text = " Exit";

		if (choose_index == 0) {
			startC.text = ">Start";
		} else if (choose_index == 1) {
			modC.text = ">Mod Manager";
		}else{
			exitC.text = ">Exit Manager";
		}

		int horizontal;
		horizontal = (int) (Input.GetAxisRaw ("Horizontal"));

		if (horizontal > 0) {
			if (!pressed) {
				choose_index += horizontal;
				if (choose_index >= 3)
					choose_index -= horizontal;
				pressed = true;
			}
		} else if (horizontal < 0) {
			if (!pressed) {
				choose_index += horizontal;
				if (choose_index < 0)
					choose_index -= horizontal;
				pressed = true;
			}
		} else if (Input.GetKey (KeyCode.Space)) {
			if (!pressed) {
				if (choose_index == 0) {
					SceneManager.LoadScene ("Main");
				} else if (choose_index == 1) {
					GetComponent<ModManagerC> ().Open ();
				}else {
					Application.Quit ();
				}
				pressed = true;
			}
		} else {
			pressed = false;
		}


	}

	void CGPlay()
	{
		if (CGProcess == 0) { // 第一幕

			CGImage.sprite = CG1;
			CGImage.color = Color.black;
			effectManager.ImageFade (CGImage, 1, 1.0f);
			textContent.text = "";
			effectManager.PrintText (textContent, "February 23rd,2020.\nA military base in west America.", 0.01f);
			++CGProcess;
			cur_time = 0;
		} else if (CGProcess == 1) {
			if (cur_time <= 3.0f || effectManager.isFading || effectManager.isPrint) {
				return;
			}
			textContent.text = "";
			effectManager.PrintText (textContent, "According to some report,a secretive research was in progress here.\nHowever no none knows more details...", 0.01f);
			++CGProcess;
			cur_time = 0;
		} else if (CGProcess == 2) {
			if (cur_time <= 3.0f || effectManager.isFading || effectManager.isPrint) {
				return;
			}
			effectManager.ImageFade (CGImage, -1, 1.0f);
			textContent.text = "";
			++CGProcess;
			cur_time = 0;
		} else if (CGProcess == 3) {
			if (cur_time <= 2.0f || effectManager.isFading || effectManager.isPrint) {
				return;
			}
			effectManager.PrintText (textContent, "Until one day.", 0.01f);
			++CGProcess;
			cur_time = 0;
		} else if (CGProcess == 4) {
			if (cur_time <= 1.0f || effectManager.isFading || effectManager.isPrint) {
				return;
			}
			CGImage.sprite = CG2;
			CGImage.color = Color.white;
			effectManager.Shake (CGUI, 2.0f, 0.1f);
			++CGProcess;
			cur_time = 0;
		} else if (CGProcess == 5) {
			if (cur_time <= 1.0f || effectManager.isPrint || effectManager.isShaking) {
				return;
			}
			textContent.text = "";
			effectManager.PrintText (textContent, "A huge explosion occured,which was considered as an accident.\n", 0.01f);
			++CGProcess;
			cur_time = 0;
		} else if (CGProcess == 6) {
			if (cur_time <= 3.0f || effectManager.isFading || effectManager.isPrint) {
				return;
			}
			textContent.text = "";
			effectManager.PrintText (textContent, "The explosion seriously destroy the base.Something was released...\n", 0.01f);
			++CGProcess;
			cur_time = 0;
		} else if (CGProcess == 7) {
			if (cur_time <= 1.0f || effectManager.isFading || effectManager.isPrint) {
				return;
			}
			effectManager.ImageFade (CGImage, -1, 1.0f);
			++CGProcess;
			cur_time = 0;
		} else if (CGProcess == 8) {
			if (cur_time <= 2.0f || effectManager.isFading || effectManager.isPrint) {
				return;
			}
			CGImage.sprite = CG3;
			CGImage.color = Color.black;
			effectManager.ImageFade (CGImage, 1, 1.0f);
			++CGProcess;
			cur_time = 0;
		} else if (CGProcess == 9) {
			textContent.text = "";
			effectManager.PrintText (textContent, "Something extremly terrible...\n", 0.01f);
			++CGProcess;
			cur_time = 0;
		} else if (CGProcess == 10) {
			if (cur_time <= 2.0f || effectManager.isFading || effectManager.isPrint) {
				return;
			}
			textContent.text = "";
			effectManager.ImageFade (CGImage, -1, 1.0f);
			++CGProcess;
		} else if (CGProcess == 11) {
			if (effectManager.isFading || effectManager.isPrint) {
				return;
			}
			CGCanvas.SetActive (false);
			isCGPlaying = false;
		}
	}
}
