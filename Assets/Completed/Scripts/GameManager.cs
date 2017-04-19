using UnityEngine;
using System.Collections;

namespace Completed
{
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.

	
	public class GameManager : MonoBehaviour
	{
		private struct FadingItem{
			public ElementProperty obj;
			public int fadingTime;
		};

		public int columns, rows;
		public bool firstLoaded = true;

		public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
		public float turnDelay = 0.1f;							//Delay between each Player turn.
		public int playerFoodPoints = 100;						//Starting value for Player food points.
		public int playerAtk = 7;
		public int PlayerDef = 0;
		public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
		[HideInInspector] public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.
		
		
		private Text levelText;									//Text to display current level number.
		private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
		public BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.
		private ResourcesManager resourceScript;
		private ControllerPage controllerScript;
		private int level = 1;									//Current level number, expressed in game as "Day 1".
		private List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
		private List<FadingItem> fadinglist;
		private bool enemiesMoving;								//Boolean to check if enemies are moving.
		private bool doingSetup = true;							//Boolean to check if we're setting up board, prevent Player from moving during setup.
		
		
		
		//Awake is always called before any Start functions
		void Awake()
		{
			//Check if instance already exists
			if (instance == null)
				
				//if not, set instance to this
				instance = this;
			
			//If instance already exists and it's not this:
			else if (instance != this)
				
				//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
				Destroy(gameObject);	
			
			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);
			
			//Assign enemies to a new List of Enemy objects.
			enemies = new List<Enemy>();
			fadinglist = new List<FadingItem> ();
			
			//Get a component reference to the attached BoardManager script
			boardScript = GetComponent<BoardManager>();
			resourceScript = GetComponent<ResourcesManager>();
			controllerScript = GetComponent<ControllerPage> ();
			columns = boardScript.columns;
			rows = boardScript.rows;

			resourceScript.Initialize ();

			//Call the InitGame function to initialize the first level 
			InitGame();
		}
			
		
		//This is called each time a scene is loaded.
		//重载场景时自动调用
		void OnLevelWasLoaded(int index)
		{
			//Add one to our level number.

			//Call InitGame to initialize our level.
			if (!firstLoaded) {
				level++;
				InitGame ();
			}
		}
		
		//Initializes the game for each level.
		void InitGame()
		{
			//While doingSetup is true the player can't move, prevent player from moving while title card is up.
			doingSetup = true;
			
			//Get a reference to our image LevelImage by finding it by name.
			levelImage = GameObject.Find("LevelImage");
			
			//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
			levelText = GameObject.Find("LevelText").GetComponent<Text>();
			
			//Set the text of levelText to the string "Day" and append the current level number.
			levelText.text = "Day " + level;
			
			//Set levelImage to active blocking player's view of the game board during setup.
			levelImage.SetActive(true);
			
			//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
			//延迟调用方法
			Invoke("HideLevelImage", levelStartDelay);
			
			//Clear any Enemy objects in our List to prepare for next level.
			enemies.Clear();
			fadinglist.Clear();

			BagSystem.instance.Initialize ();
			controllerScript.initialize ();

			//Call the SetupScene function of the BoardManager script, pass it current level number.
			boardScript.SetupScene(level);
			
		}
		
		
		//Hides black image used between levels
		void HideLevelImage()
		{
			//Disable the levelImage gameObject.
			levelImage.SetActive(false);
			
			//Set doingSetup to false allowing player to move again.
			doingSetup = false;
		}

		void Start()
		{
			/*ElementIndex i;
			i.first = 0;i.second = 0;
			BagSystem.instance.AddItem (i);
			i.first = 0;i.second = 1;
			BagSystem.instance.AddItem (i);
			BagSystem.instance.AddItem (resourceScript.GetItemIndex ("WYS_01"));
			BagSystem.instance.AddItem (resourceScript.GetItemIndex ("WYS_02"));
			BagSystem.instance.AddItem (resourceScript.GetItemIndex ("WYS_03"), 10);
			BagSystem.instance.AddItem (resourceScript.GetItemIndex ("WYS_04"), 3);
			BagSystem.instance.AddItem (resourceScript.GetItemIndex ("WYS_05"));
			BagSystem.instance.AddItem (resourceScript.GetItemIndex ("WYS_06"));*/
		}
		
		//Update is called every frame.
		void Update()
		{
			//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
			if(playersTurn || enemiesMoving || doingSetup)
				
				//If any of these are true, return and do not start MoveEnemies.
				return;
			
			//Start moving enemies.

			//检测敌人是否死亡
			for (int i = 0; i < enemies.Count; i++)
				if (enemies [i].HP <= 0) {
					StartCoroutine(enemies [i].die()); // 设置敌人组件失效
					enemies.RemoveAt (i); // 移除敌人
					--i;
				}

			//处理腐坏序列
			for (int i = 0; i < fadinglist.Count; i++) {
				if (fadinglist [i].fadingTime <= 0) {
					StartCoroutine (fadinglist [i].obj.Broken ());
					fadinglist.RemoveAt (i);
					--i;
					continue;
				}
				FadingItem fi = fadinglist[i];
				fi.fadingTime -= 1;
				fadinglist [i] = fi;
			}

			StartCoroutine (MoveEnemies ());
		}
		
		//Call this to add the passed in Enemy to the List of Enemy objects.
		public int AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
			return enemies.Count - 1;
		}

		public int AddItemToFaing(ElementProperty script, int fadingtime)//添加腐坏序列
		{
			FadingItem temp;
			temp.obj = script;
			temp.fadingTime = fadingtime;
			fadinglist.Add (temp);
			return fadinglist.Count - 1;
		}
		
		
		//GameOver is called when the player reaches 0 food points
		public void GameOver()
		{
			//Set levelText to display number of levels passed and game over message
			levelText.text = "After " + level + " days, you starved.";
			
			//Enable black background image gameObject.
			levelImage.SetActive(true);
			
			//Disable this GameManager.
			enabled = false;
		}

		public void AddEnemyByID (string ID)
		{
			ElementIndex t = ResourcesManager.instance.GetEnemyIndex (ID);
			try{
				EnemiesType obj = ResourcesManager.instance.enemiesPool [t.first] [t.second];
				List<EnemiesType> l = new List<EnemiesType> ();
				l.Add (obj);
				boardScript.LayoutObjectAtRandom_Enemy (l, 1, 1);
			}catch{
				return;
			}
		}
		
		//Coroutine to move enemies in sequence.
		IEnumerator MoveEnemies()
		{
			//While enemiesMoving is true player is unable to move.
			enemiesMoving = true;
			
			//Wait for turnDelay seconds, defaults to .1 (100 ms).
			yield return new WaitForSeconds(turnDelay);
			
			//If there are no enemies spawned (IE in first level):
			if (enemies.Count == 0) 
			{
				//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
				yield return new WaitForSeconds(turnDelay);
			}
			
			//Loop through List of Enemy objects.
			for (int i = 0; i < enemies.Count; i++)
			{
				//Call the MoveEnemy function of Enemy at index i in the enemies List.
				enemies[i].MoveEnemy ();
				
				//Wait for Enemy's moveTime before moving next Enemy, 
				yield return new WaitForSeconds(0.1f / enemies.Count);
			}
			//Once Enemies are done moving, set playersTurn to true so player can move.
			playersTurn = true;
			
			//Enemies are done moving, set enemiesMoving to false.
			enemiesMoving = false;
		}
	}
}

