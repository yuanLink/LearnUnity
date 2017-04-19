using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

namespace Completed
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MovingObject
	{
		public int Atk = 7;						//人物反击值
		public int DEF = 0;							//人物防御值

		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
		public int pointsPerFood = 10;				//Number of points to add to player food points when picking up a food object.
		public int pointsPerSoda = 20;				//Number of points to add to player food points when picking up a soda object.
		public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
		public Text foodText;						//UI Text to display current player food total.
		public int moveSound1;				//1 of 2 Audio clips to play when player moves.
		public int moveSound2;				//2 of 2 Audio clips to play when player moves.
		public int eatSound1;					//1 of 2 Audio clips to play when player collects a food object.
		public int eatSound2;					//2 of 2 Audio clips to play when player collects a food object.
		public int drinkSound1;				//1 of 2 Audio clips to play when player collects a soda object.
		public int drinkSound2;				//2 of 2 Audio clips to play when player collects a soda object.
		public int gameOverSound;				//Audio clip to play when player dies.
		
		private Animator animator;					//Used to store a reference to the Player's animator component.
		private int food;							//Used to store player food points total during level.
		private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
		
		
		//Start overrides the Start function of MovingObject
		protected override void Start ()
		{
			//Get a component reference to the Player's animator component
			animator = GetComponent<Animator>();
			
			//Get the current food point total stored in GameManager.instance between levels.
			food = GameManager.instance.playerFoodPoints;
			Atk = GameManager.instance.playerAtk;
			DEF = GameManager.instance.PlayerDef;
			
			//Set the foodText to reflect the current player food total.
			foodText.text = "Food: " + food;
			
			//Call the Start function of the MovingObject base class.
			base.Start ();
		}
		
		
		//This function is called when the behaviour becomes disabled or inactive.
		private void OnDisable ()
		{
			//When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
			GameManager.instance.playerFoodPoints = food;
			GameManager.instance.playerAtk = Atk;
			GameManager.instance.PlayerDef = DEF;
		}
		
		
		private void Update ()
		{
			//If it's not the player's turn, exit the function.
			if(!GameManager.instance.playersTurn) return;
			if (BagSystem.instance.isOpening)
				return;
			if (ControllerPage.instance.isScripting)
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
			//Check if we have a non-zero value for horizontal or vertical
			if((!isMoving) &&( horizontal != 0 || vertical != 0))
			{
				//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
				//Pass in horizontal and vertical as parameters to specify the direction to move Player in.

				/*GameObject t = checkBlock (transform.position.x, transform.position.y+1);
				if (t != null)
					Debug.Log (t.name);*/

				AttemptMove<Wall> (horizontal, vertical);

			}

			if (Input.GetKeyUp (KeyCode.I)) {
				BagSystem.instance.Open ();
			}

			if (Input.GetKeyUp (KeyCode.Tab)) {
				ControllerPage.instance.Open();
			}
		}
		
		//AttemptMove overrides the AttemptMove function in the base class MovingObject
		//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			//Every time player moves, subtract from food points total.
			food--;
			
			//Update food text display to reflect current score.
			foodText.text = "Food: " + food;
			
			//Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
			base.AttemptMove <T> (xDir, yDir);
			
			//Hit allows us to reference the result of the Linecast done in Move.
			RaycastHit2D hit;
			
			//If Move returns true, meaning Player was able to move into an empty space.
			if (Move (xDir, yDir, out hit)) {
				//Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
				SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
			} else {//如果是敌人则近战攻击敌人
				Enemy tar_enemy = hit.transform.GetComponent<Enemy> ();
				if (tar_enemy != null && tar_enemy.HP > 0) {
					animator.SetTrigger ("playerChop");
					tar_enemy.BeAttack (Atk);
				}
			}
			
			//Since the player has moved and lost food points, check if the game has ended.
			CheckIfGameOver ();

			GameManager.instance.playersTurn = false;
		}
		
		
		//OnCantMove overrides the abstract function OnCantMove in MovingObject.
		//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
		protected override void OnCantMove <T> (T component)
		{
			//Set hitWall to equal the component passed in as a parameter.
			Wall hitWall = component as Wall;
			
			//Call the DamageWall function of the Wall we are hitting.
			hitWall.DamageWall (wallDamage);
			
			//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
			animator.SetTrigger ("playerChop");
		}
		
		
		//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
		private void OnTriggerEnter2D (Collider2D other)
		{
			//Check if the tag of the trigger collided with is Exit.
			if(other.tag == "Exit")
			{
				//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
				Invoke ("Restart", restartLevelDelay);
				
				//Disable the player object since level is over.
				enabled = false;
			}

			ElementProperty tar_script = other.transform.GetComponent<ElementProperty> ();
			if (tar_script != null && (tar_script.index.first >= 0 && tar_script.index.second >= 0)) {
				if (tar_script.type == ElementType.food){//人物捡到食物
					SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
					tar_script.DoEffect(this);
				} else if (tar_script.type == ElementType.item){//人物捡到道具
					BagSystem.instance.AddItem (tar_script.index);
					tar_script.Destroy ();
				}else{//人物触发剧情
					ResourcesManager.instance.storiesPool [tar_script.index.first] [tar_script.index.second].effect.Call (this);
				}
			}
			
			/*//Check if the tag of the trigger collided with is Food.
			else if(other.tag == "Food")
			{
				//Add pointsPerFood to the players current food total.
				food += pointsPerFood;
				
				//Update foodText to represent current total and notify player that they gained points
				foodText.text = "+" + pointsPerFood + " Food: " + food;
				
				//Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
				SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
				
				//Disable the food object the player collided with.
				other.gameObject.SetActive (false);
			}
			
			//Check if the tag of the trigger collided with is Soda.
			else if(other.tag == "Soda")
			{
				//Add pointsPerSoda to players food points total
				food += pointsPerSoda;
				
				//Update foodText to represent current total and notify player that they gained points
				foodText.text = "+" + pointsPerSoda + " Food: " + food;
				
				//Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
				SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
				
				//Disable the soda object the player collided with.
				other.gameObject.SetActive (false);
			}*/
		}
		
		
		//Restart reloads the scene when called.
		private void Restart ()
		{
			//Load the last scene loaded, in this case Main, the only scene in the game.
			GameManager.instance.firstLoaded = false;
			SceneManager.LoadScene("Main");
			//Application.LoadLevel (Application.loadedLevel);
		}
		
		
		//LoseFood is called when an enemy attacks the player.
		//It takes a parameter loss which specifies how many points to lose.
		public void LoseFood (int loss)
		{
			//Set the trigger for the player animator to transition to the playerHit animation.
			animator.SetTrigger ("playerHit");
			
			//Subtract lost food points from the players total.
			food -= loss;
			
			//Update the food display with the new total.
			foodText.text = "-"+ loss + " Food: " + food;
			
			//Check to see if game has ended.
			CheckIfGameOver ();
		}

		public void GetFood (int value)
		{
			if (value <= 0)
				return;
			food += value;
			foodText.text = "+" + value + " Food: " + food;
		}

		public void BeAttacked (int demage)
		{
			//Set the trigger for the player animator to transition to the playerHit animation.
			animator.SetTrigger ("playerHit");

			demage = demage - DEF;
			if (demage < 0)
				demage = 0;
			food -= demage;

			//Update the food display with the new total.
			foodText.text = "-"+ demage + " Food: " + food;

			//Check to see if game has ended.
			CheckIfGameOver ();
		}
		
		
		//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
		private void CheckIfGameOver ()
		{
			//Check if food point total is less than or equal to zero.
			if (food <= 0) 
			{
				//Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
				SoundManager.instance.PlaySingle (gameOverSound);
				
				//Stop the background music.
				SoundManager.instance.musicSource.Stop();
				
				//Call the GameOver function of GameManager.
				GameManager.instance.GameOver ();
			}
		}

		public GameObject checkBlock(double x, double y)
		{
			Vector2 start = new Vector2 (Mathf.Floor ((float)x), Mathf.Floor ((float)y));
			Vector2 end = new Vector2 (start.x + 0.5f, start.y + 0.5f);
			if (start.x >= (GameManager.instance.columns) - 1 || start.x < 1 || start.y >= (GameManager.instance.rows) - 1 || start.y < 1)
				return null;

			RaycastHit2D hit = Physics2D.Linecast (start, end);

			if (hit.transform == null)
				return null;
			return hit.transform.gameObject;
		}

		public Enemy CheckDirection(int direction)
		{
			Vector2 tmp = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y),
					dir;
			if (direction == 0) {
				dir = new Vector2 (0.0f, 1.0f);
			} else if (direction == 1) {
				dir = new Vector2 (0.0f, -1.0f);
			} else if (direction == 2) {
				dir = new Vector2 (-1.0f, 0.0f);
			} else if (direction == 3) {
				dir = new Vector2 (1.0f, 0.0f);
			} else {
				return null;
			}
			GetComponent<BoxCollider2D> ().enabled = false;	
			RaycastHit2D[] hit = Physics2D.RaycastAll (tmp, dir);
			GetComponent<BoxCollider2D> ().enabled = true;	
			foreach (RaycastHit2D ele in hit) {
				if (ele.transform == null)
					continue;
				ElementProperty t;
				if ((t = ele.transform.GetComponent<ElementProperty> ()) != null)
					continue;
				if (ele.transform.tag == "Enemy")
					return ele.transform.GetComponent<Enemy> ();
				else
					return null;
			}
			return null;
		}

		protected override void ChangeHPValue (int value)
		{
			if (value >= 0)
				GetFood (value);
			else
				BeAttacked (-value);
		}

		public void RaiseAtk(int value)
		{
			if (value <= 0)
				return;
			Atk += value;
		}

		public void ReduceAtk(int value)
		{
			if (value <= 0)
				return;
			Atk -= value;
			if (Atk < 0)
				Atk = 0;
		}

		public void RaiseDef(int value)
		{
			if (value <= 0)
				return;
			DEF += value;
		}

		public void ReduceDef(int value)
		{
			if (value <= 0)
				return;
			DEF -= value;
			if (DEF < 0)
				DEF = 0;
		}

		public int GetColumns()
		{
			return GameManager.instance.columns;
		}

		public int GetRows()
		{
			return GameManager.instance.rows;
		}
	}
}

