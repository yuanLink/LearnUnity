using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine.UI;

public class BagSystem : MonoBehaviour {

	public static BagSystem instance = null;

	public GameObject gameCanvas;
	public GameObject bagCanvas; // 背包画布
	public GameObject[] itemUI;  // 物品项
	public GameObject itemUIModel; // 物品项模板
	public int itemColumns;
	public GameObject chooseBoxUI;	// 选择框
	public GameObject displayUI; // 展示框
	public Text itemName, // 物品名UI
				itemNum, // 物品数量UI
				itemComment; // 物品描述UI

	[HideInInspector] public List<ItemsType> bagContent = new List<ItemsType>();
	private bool pressed = false,
				spacepressed = false;

	private bool _isOpening = false; // 背包是否打开中
	public bool isOpening{
		get { return _isOpening; }
	}
	private bool _isHided = false;
	private int _currentIndex = 0; // 当前选中物品序号
	public int currentIndex{
		get { return _currentIndex; }
	}


	private int direction = -1; // 方向，0为上，1为下，2为左，3为右 
	private bool direction_choosing = false;
	private LuaFunction direction_effect = null;
	private int direction_index = -1;
	private GameObject directionUI = null;
	public GameObject cPlayer;
	private Animator direction_animator = null;

	void Awake ()
	{
		//Check if there is already an instance of BagSystem
		if (instance == null)
			//if not, set it to this.
			instance = this;
		//If instance already exists:
		else if (instance != this)
			//Destroy this, this enforces our singleton pattern so there can only be one instance of BagSystem.
			Destroy (gameObject);

		//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
		DontDestroyOnLoad (gameObject);
	}

	public void Initialize()
	{
		itemName = GameObject.Find ("ItemName").GetComponent<Text> ();
		itemNum = GameObject.Find ("ItemNum").GetComponent<Text> ();
		itemComment = GameObject.Find ("Comment").GetComponent<Text> ();

		bagCanvas = GameObject.Find ("Canvas_item");
		itemColumns = 4;
		itemUI = new GameObject[12];
		for (int i = 0; i < 12; ++i) {
			itemUI [i] = GameObject.Find ("Item" + (i + 1));
		}
		chooseBoxUI = GameObject.Find ("chooseBox");
		displayUI = GameObject.Find ("DisplayItem");
		gameCanvas = GameObject.Find ("Canvas");
		directionUI = GameObject.Find ("UIDirection");
		direction_animator = directionUI.GetComponent<Animator> ();
		directionUI.SetActive (false);
		cPlayer = GameObject.Find ("Player");

		bagCanvas.SetActive (false);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update(){
		if (!isOpening)
			return;
		if (_isHided && (!direction_choosing))
			return;


		int horizontal = 0;  	//Used to store the horizontal move direction.
		int vertical = 0;		//Used to store the vertical move direction.

		//Check if we are running either in the Unity editor or in a standalone build.
		#if UNITY_STANDALONE || UNITY_WEBPLAYER

		//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
		horizontal = (int) (Input.GetAxisRaw ("Horizontal"));

		//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
		vertical = (int) (Input.GetAxisRaw ("Vertical"));

		//Check if moving horizontally, if so set vertical to zero.
		if(horizontal != 0)
		{
			vertical = 0;
		}
		//Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		//Check if Input has registered more than zero touches
		if (Input.touchCount > 0)
		{
		//Store the first touch detected.
		Touch myTouch = Input.touches[0];

		//Check if the phase of that touch equals Began
		if (myTouch.phase == TouchPhase.Began)
		{
		//If so, set touchOrigin to the position of that touch
		touchOrigin = myTouch.position;
		}

		//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
		else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
		{
		//Set touchEnd to equal the position of this touch
		Vector2 touchEnd = myTouch.position;

		//Calculate the difference between the beginning and end of the touch on the x axis.
		float x = touchEnd.x - touchOrigin.x;

		//Calculate the difference between the beginning and end of the touch on the y axis.
		float y = touchEnd.y - touchOrigin.y;

		//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
		touchOrigin.x = -1;

		//Check if the difference along the x axis is greater than the difference along the y axis.
		if (Mathf.Abs(x) > Mathf.Abs(y))
		//If x is greater than zero, set horizontal to 1, otherwise set it to -1
		horizontal = x > 0 ? 1 : -1;
		else
		//If y is greater than zero, set horizontal to 1, otherwise set it to -1
		vertical = y > 0 ? 1 : -1;
		}
		}

		#endif //End of mobile platform dependendent compilation section started above with #elif

		if (horizontal == 0 && vertical == 0)
			pressed = false;
		if (!Input.GetKey (KeyCode.Space))
			spacepressed = false;

		if (direction_choosing) {
			if (!directionUI.activeSelf)
				directionUI.SetActive (true);
			if (horizontal > 0) {
				pressed = true;
				direction = 3;
				//direction_animator.SetTrigger ("Right");
				Vector3 dir = new Vector3 (cPlayer.transform.position.x + 1,
					cPlayer.transform.position.y,
					cPlayer.transform.position.z);
				directionUI.transform.position =  dir;
				directionUI.transform.rotation = Quaternion.Euler (new Vector3(0, 0, -90));
				
			}else if (horizontal < 0) {
				pressed = true;
				direction = 2;
				//direction_animator.SetTrigger ("Left");
				Vector3 dir = new Vector3 (cPlayer.transform.position.x - 1,
					cPlayer.transform.position.y,
					cPlayer.transform.position.z);
				directionUI.transform.position =  dir;
				directionUI.transform.rotation = Quaternion.Euler (new Vector3(0, 0, 90));
			}else if (vertical > 0) {
				pressed = true;
				direction = 0;
				//direction_animator.SetTrigger ("Up");
				Vector3 dir = new Vector3 (cPlayer.transform.position.x,
					cPlayer.transform.position.y + 1,
					cPlayer.transform.position.z);
				directionUI.transform.position =  dir;
				directionUI.transform.rotation = Quaternion.Euler (new Vector3(0, 0, 0));
			} else if (vertical < 0) {
				pressed = true;
				direction = 1;
				//direction_animator.SetTrigger ("Down");
				Vector3 dir = new Vector3 (cPlayer.transform.position.x,
					cPlayer.transform.position.y - 1,
					cPlayer.transform.position.z);
				directionUI.transform.position =  dir;
				directionUI.transform.rotation = Quaternion.Euler (new Vector3(0, 0, 180));
			}

			if (Input.GetKey (KeyCode.Space) && (!spacepressed)) {
				directionUI.SetActive (false);
				spacepressed = true;
				direction_choosing = false;
				object[] ret = direction_effect.Call (direction);
				if (ret != null) {
					bool retb = (bool)ret[0];
					if (!retb) {//使用失败
						Show();
						return;
					}
				}

				ItemsType tmp = bagContent [direction_index];

				if (tmp.cur_usingtimes > 0) { // 尚有损耗度
					tmp.cur_usingtimes -= 1;
					if (tmp.cur_usingtimes > 0) // 使用后尚有损耗度
						bagContent [direction_index] = tmp;
					else { // 使用后没有损耗度
						tmp.cur_usingtimes = (int)tmp.usingtimes;
						tmp.number -= 1;
						if (tmp.number <= 0)
							bagContent.RemoveAt (direction_index);
						else
							bagContent [direction_index] = tmp;
					}
				} else { // 使用次数无限

				}
				Close ();
				Completed.GameManager.instance.playersTurn = false;
			}

			return;
		}

		if (!pressed) {
			if (horizontal != 0) {
				pressed = true;
				int nextIndex = horizontal + currentIndex;
				if (nextIndex >= 0 && nextIndex < bagContent.Count) {
					_currentIndex = nextIndex;
					UpdateShow ();
				}
			} else if (vertical != 0) {
				pressed = true;
				int nextIndex = (-1 * vertical) * itemColumns + currentIndex;
				if (nextIndex >= 0 && nextIndex < bagContent.Count) {
					_currentIndex = nextIndex;
					UpdateShow ();
				}
			}
		}

		if (Input.GetKey (KeyCode.Space) && (!spacepressed)) {
			spacepressed = true;
			UseItem (currentIndex); // 使用物品
		} else if (Input.GetKey (KeyCode.Escape)) {
			Close (); // 关闭背包
		}
	}

	public void Open()
	{
		_isOpening = true;
		_isHided = false;
		_currentIndex = 0;
		gameCanvas.SetActive (false);
		bagCanvas.SetActive (true);
		UpdateShow ();
	}

	public void Close()
	{
		_isOpening = false;
		_isHided = false;
		_currentIndex = 0;
		gameCanvas.SetActive (true);
		bagCanvas.SetActive (false);
	}

	private void Hide ()
	{
		_isHided = true;
		bagCanvas.SetActive (false);
	}

	private void Show()
	{
		_isHided = false;
		bagCanvas.SetActive (true);
	}

	public void UseItem(int index)
	{
		if (index < 0 || index >= bagContent.Count)
			return;

		Hide ();
		if (bagContent [index].use_direction) {
			direction = 0;
			directionUI.SetActive (true);
			Vector3 dir = new Vector3 (cPlayer.transform.position.x,
				              cPlayer.transform.position.y + 1,
				              cPlayer.transform.position.z);
			directionUI.transform.position =  dir;
			direction_animator.SetTrigger ("Up");
			direction_choosing = true;
			direction_effect = bagContent [index].effect;
			direction_index = index;
			return;
		}

		object[] ret = bagContent [index].effect.Call ();
		if (ret != null) {
			bool retb = (bool)ret[0];
			if (!retb) {//使用失败
				Show();
				return;
			}
		}

		ItemsType tmp = bagContent [index];

		if (tmp.cur_usingtimes > 0) { // 尚有损耗度
			tmp.cur_usingtimes -= 1;
			if (tmp.cur_usingtimes > 0) // 使用后尚有损耗度
				bagContent [index] = tmp;
			else { // 使用后没有损耗度
				tmp.cur_usingtimes = (int)tmp.usingtimes;
				tmp.number -= 1;
				if (tmp.number <= 0)
					bagContent.RemoveAt (index);
				else
					bagContent [index] = tmp;
			}
		} else { // 使用次数无限
			
		}
		Close ();
		Completed.GameManager.instance.playersTurn = false;
	}

	public void UpdateShow()//更新背包UI
	{
		if (currentIndex < 0 || currentIndex >= bagContent.Count) { // 背包中没有物品
			_currentIndex = 0;
			chooseBoxUI.SetActive (false);
			displayUI.GetComponent<Image> ().sprite = itemUIModel.GetComponent<Image> ().sprite;
			itemName.text = "";
			itemNum.text = "";
			itemComment.text = "";
			for (int i =0;i < itemUI.Length;++i)
				itemUI[i].GetComponent<Image> ().sprite = itemUIModel.GetComponent<Image> ().sprite;
			return;
		}

		int cur_pagestart = currentIndex - currentIndex % itemUI.Length; //当前页面的第一项物品序号
		for (int i = cur_pagestart, j = 0; i < cur_pagestart + itemUI.Length; ++i, ++j) {
			if (i < bagContent.Count) {
				itemUI [j].GetComponent<Image> ().sprite = bagContent [i].tile.GetComponent<SpriteRenderer> ().sprite;
			} else {
				itemUI[j].GetComponent<Image> ().sprite = itemUIModel.GetComponent<Image> ().sprite;
			}
		}
		chooseBoxUI.SetActive (true);
		chooseBoxUI.transform.position = itemUI [currentIndex - cur_pagestart].transform.position;
		displayUI.GetComponent<Image> ().sprite = itemUI [currentIndex - cur_pagestart].GetComponent<Image> ().sprite;
		itemName.text = bagContent [currentIndex].name;
		itemNum.text = "x" + bagContent [currentIndex].number;
		if (bagContent [currentIndex].usingtimes > 1) {
			itemNum.text += "(" + bagContent [currentIndex].cur_usingtimes + ")";
		}
		itemComment.text = bagContent [currentIndex].comment;
	}

	public bool AddItem(ElementIndex itemIndex, int number = 1) // 添加物品到背包
	{
		if (itemIndex.first < 0 || itemIndex.second < 0 || number < 0)
			return false;
		ItemsType item = ResourcesManager.instance.itemsPool [itemIndex.first] [itemIndex.second];
		for (int i = 0; i < bagContent.Count; ++i) {
			if (bagContent [i].ID == item.ID) {
				if (item.usingtimes <= 0) // 不允许叠加无限耐久度的物品
					return true;
				ItemsType tmp = bagContent [i];
				tmp.number += number;
				tmp.cur_usingtimes = (int)tmp.usingtimes;
				bagContent [i] = tmp;
				return true;
			}
		}
		item.number = number;
		item.cur_usingtimes = (int)item.usingtimes;
		bagContent.Add (item);
		return true;
	}

	public bool ReduceItem(ElementIndex itemIndex, int number = 1, bool notBellowZero = false) // 减少物品数量
	{
		if (itemIndex.first < 0 || itemIndex.second < 0 || number < 0)
			return false;
		ItemsType item = ResourcesManager.instance.itemsPool [itemIndex.first] [itemIndex.second];
		for (int i = 0; i < bagContent.Count; ++i) {
			if (bagContent [i].ID == item.ID) {
				ItemsType tmp = bagContent [i];
				tmp.number -= number;
				if (tmp.number == 0)
					bagContent.RemoveAt (i);
				else if (tmp.number < 0) {
					if (notBellowZero) {
						return false;
					} else {
						bagContent.RemoveAt (i);
					}
				} else {
					bagContent [i] = tmp;
				}
				return true;
			}
		}
		return false;
	}

	public bool ReduceItemByID(string ID, int number = 1, bool notBellowZero = false)
	{
		return ReduceItem (ResourcesManager.instance.GetItemIndex (ID), number, notBellowZero);
	}

	public bool AddItemByID(string ID, int number = 1)
	{
		return AddItem (ResourcesManager.instance.GetItemIndex (ID), number);
	}
		
}
