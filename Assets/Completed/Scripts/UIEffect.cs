using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIEffect : MonoBehaviour {

	public static UIEffect instance;
	// 淡入淡出特效
	public bool isFading = false;
	public int fadingDirection = 0;
	public float fadingSpeed = 0;
	public Image fadingObj = null;
	public Color startColor;
	public float process;

	// 震动特效
	public bool isShaking = false;
	public GameObject shakingObj = null;
	public float shakingRange;
	public int shakeProcess;
	public float shakeTime;
	private float time;
	private Vector3[] trans = new Vector3[]{new Vector3(0,1,0),new Vector3(0,-2,0),new Vector3(1,2,0),
		new Vector3(-2,-2,0),new Vector3(0,2,0),new Vector3(2,-2,0),new Vector3(-1,1,0)};

	// 打字机特效
	public bool isPrint = false;
	public Text printObj = null;
	public string printText;
	public float printTime;
	public int oncePrint;
	private int printProcess;
	private float curTime;


	// Use this for initialization
	void Start () {
		//ImageFade (GameObject.Find ("CG").GetComponent<Image> (), -1, 0.5f);
		//Shake(GameObject.Find("CG"), 2.0f, 0.1f);
		//PrintText(GameObject.Find("Text").GetComponent<Text>(), "The story will be displayed here.", 0.05f);
		instance = this;
	}

	// Update is called once per frame
	void Update () {
		if (isFading) {
			if (fadingDirection > 0) {
				fadingObj.color = Color.Lerp (startColor, Color.white, (process += fadingSpeed * Time.deltaTime));
				if (fadingObj.color == Color.white)
					isFading = false;
			} else if (fadingDirection < 0) {
				fadingObj.color = Color.Lerp (startColor, Color.black, (process += fadingSpeed * Time.deltaTime));
				if (fadingObj.color == Color.black)
					isFading = false;
			} else {
				isFading = false;
			}
		}

		if (isPrint) {
			curTime += Time.deltaTime;
			if (printProcess >= printText.Length) {
				isPrint = false;
				return;
			}
			if (curTime > printTime) {
				for (int i = 0; i < oncePrint && printProcess < printText.Length; ++printProcess, ++i)
					printObj.text += printText [printProcess];
				curTime = 0;
			}
		}
			
		if (isShaking) {
			time += Time.deltaTime;
			if (shakeProcess >= trans.Length) {
				isShaking = false;
				return;
			}
			if (time > shakeTime) {
				shakingObj.transform.position = shakingObj.transform.position + trans [shakeProcess] * shakingRange;
				++shakeProcess;
				time = 0;
			}
		}
	}

	public void ImageFade(Image obj, int direction, float speed) // 淡入淡出特效
	{
		if (isFading)
			return;
		isFading = true;
		fadingDirection = direction;
		fadingSpeed = speed;
		fadingObj = obj;
		startColor = fadingObj.color;
		process = 0;
	}

	public void Shake(GameObject obj, float range, float shaketime) // 震动特效
	{
		if (isShaking)
			return;
		isShaking = true;
		shakingObj = obj;
		shakingRange = range;
		shakeProcess = 0;
		time = 0;
		shakeTime = shaketime;
	}

	public void PrintText(Text obj, string showText, float showTime, int onceShow = 1) // 打字机特效
	{
		if (isPrint)
			return;
		isPrint = true;
		printObj = obj;
		printText = showText;
		printTime = showTime;
		oncePrint = onceShow;
		printProcess = 0;
		curTime = 0.0f;
	}
}
