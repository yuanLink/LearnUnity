using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

namespace Completed
	
{
	
	public class BoardManager : MonoBehaviour
	{
		// Using Serializable allows us to embed a class with sub properties in the inspector.
		[Serializable]
		public class Count
		{
			public int minimum; 			//Minimum value for our Count class.
			public int maximum; 			//Maximum value for our Count class.
			
			
			//Assignment constructor.
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}
		
		
		public int columns = 8; 										//Number of columns in our game board.
		public int rows = 8;											//Number of rows in our game board.
		public Count wallCount = new Count (5, 9);						//Lower and upper limit for our random number of walls per level.
		public Count foodCount = new Count (1, 5);						//Lower and upper limit for our random number of food items per level.
		public GameObject exit;											//Prefab to spawn for exit.
		public GameObject[] floorTiles;									//Array of floor prefabs.
		public GameObject[] wallTiles;									//Array of wall prefabs.
		public GameObject[] foodTiles;									//Array of food prefabs.
		public GameObject[] enemyTiles;									//Array of enemy prefabs.
		public GameObject[] outerWallTiles;								//Array of outer tile prefabs.
		
		private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.
		private bool isGetRare = false;

		void Awake()
		{
		}
		
		
		//Clears our list gridPositions and prepares it to generate a new board.
		void InitialiseList ()
		{
			//Clear our list gridPositions.
			gridPositions.Clear ();
			
			//Loop through x axis (columns).
			for(int x = 1; x < columns-1; x++)
			{
				//Within each column, loop through y axis (rows).
				for(int y = 1; y < rows-1; y++)
				{
					//At each index add a new Vector3 to our list with the x and y coordinates of that position.
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
				
		}
		
		
		//Sets up the outer walls and floor (background) of the game board.
		void BoardSetup ()
		{
			//Instantiate Board and set boardHolder to its transform.
			boardHolder = new GameObject ("Board").transform;
			
			//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
			for(int x = -1; x < columns + 1; x++)
			{
				//Loop along y axis, starting from -1 to place floor or outerwall tiles.
				for(int y = -1; y < rows + 1; y++)
				{
					//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
					GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
					
					//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
					if(x == -1 || x == columns || y == -1 || y == rows)
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					
					//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
					GameObject instance =
						Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					
					//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
					instance.transform.SetParent (boardHolder);
				}
			}
		}
		
		
		//RandomPosition returns a random position from our list gridPositions.
		//返回一块未被占用的坐标
		Vector3 RandomPosition ()
		{
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);
			
			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];
			
			//Remove the entry at randomIndex from the list so that it can't be re-used.
			gridPositions.RemoveAt (randomIndex);
			
			//Return the randomly selected Vector3 position.
			return randomPosition;
		}
		
		
		//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);
			
			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();
				
				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
				
				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}

		public void LayoutObjectAtRandom_Enemy (List<EnemiesType> tileArray, int minimum, int maximum) //随机生成敌人
		{
			if (tileArray.Count == 0)
				return;
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);

			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();

				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Count)].tile;

				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}

		void LayoutObjectAtRandom_Item (List<ItemsType> tileArray, int minimum, int maximum) //随机生成道具
		{
			if (tileArray.Count == 0)
				return;
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);

			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();

				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Count)].tile;

				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}

		void LayoutObjectAtRandom_RareItem (List<ItemsType> tileArray, int minimum, int maximum) //随机生成道具
		{
			if (tileArray.Count == 0)
				return;
			List<int> cur = new List<int> ();
			for (int i = 0; i < tileArray.Count; ++i) {
				if (!ResourcesManager.instance.rareItemShow [i])
					cur.Add (i);
			}
			if (minimum > cur.Count)
				minimum = maximum = cur.Count;
			if (maximum > cur.Count)
				maximum = cur.Count;
			int objectCount = Random.Range (minimum, maximum+1);
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();
				int order = Random.Range (0, cur.Count);

				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[cur[order]].tile;
				ResourcesManager.instance.rareItemShow [cur [order]] = true;

				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}

		void LayoutObjectAtRandom_Story (List<StoryType> tileArray, int minimum, int maximum) //随机生成剧情
		{
			if (tileArray.Count == 0)
				return;
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);

			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();

				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Count)].tile;

				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}

		public bool LayoutObject(GameObject tile, Vector3 position, Quaternion rotation)
		{
			if (tile == null)
				return false;

			if (checkBlock (position.x, position.y) != null)
				return false;

			Vector3 final_pos = new Vector3 (Mathf.Round(position.x), Mathf.Round(position.y), 0.0f);
			if (final_pos.x >= columns || final_pos.x < 0 || final_pos.y >= rows || final_pos.y < 0)
				return false;
			//Physics2D.BoxCast
			Instantiate(tile, final_pos, rotation);
			return true;
		}

		public bool LayoutObject(ElementIndex index, Vector3 position)
		{
			GameObject tile = ResourcesManager.instance.itemsPool[index.first][index.second].tile;

			if (tile == null)
				return false;

			if (checkBlock (position.x, position.y) != null)
				return false;

			Vector3 final_pos = new Vector3 (Mathf.Round(position.x), Mathf.Round(position.y), 0.0f);
			if (final_pos.x >= columns || final_pos.x < 0 || final_pos.y >= rows || final_pos.y < 0)
				return false;
			//Physics2D.BoxCast
			Instantiate(tile, final_pos, Quaternion.identity);
			return true;
		}

		public GameObject checkBlock(double x, double y)
		{
			Vector2 start = new Vector2 (Mathf.Round ((float)x), Mathf.Round((float)y));
			Vector2 end = new Vector2 (start.x + 0.1f, start.y + 0.1f);
			if (start.x >= (GameManager.instance.columns) || start.x < 0 || start.y >= (GameManager.instance.rows) || start.y < 0)
				return null;

			RaycastHit2D hit = Physics2D.Linecast (start, end);

			if (hit.transform == null)
				return null;
			return hit.transform.gameObject;
		}

		
		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetupScene (int level)
		{
			/*if (enemyTiles [0] == null) {
				EnemiesType impor = ResourcesManager.instance.enemyPool[0];
				enemyTiles [0] = impor.tile;
				enemyTiles [1] = ResourcesManager.instance.enemyPool[1].tile;
			}*/
			if (level % 2 == 0) {
				if (columns <= 20) {
					columns += 1;
				}
				if (rows <= 20) {
					rows += 1;
				}
			}


			//Creates the outer walls and floor.
			BoardSetup ();
			
			//Reset our list of gridpositions.
			InitialiseList ();


			//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.

			int min, max;
			//墙生成算法
			min = columns * rows / 10;
			max = columns * rows / 7;

			LayoutObjectAtRandom (wallTiles, min, max);
			
			//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
			//LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
			//食物生成算法
			min = columns * rows * (int)Mathf.Log(level+1, 2f) / 64;
			max = columns * rows * (int)Mathf.Log (level + 1, 2f) / 64 * 5;
			if (max > (columns * rows / 10)) {
				max = columns * rows / 10;
				min = max / 5;
			}
			LayoutObjectAtRandom_Item(ResourcesManager.instance.foodPool, min,  max);

			if (level % 6 != 0) {
				//普通物品生成算法
				min = 0;
				if (max < 4)
					max = 1;
				else
					max /= 4;
				LayoutObjectAtRandom_Item (ResourcesManager.instance.normalItemPool, min, max);

				//稀有物品生成算法
				max = 1000;
				min = (level % 4 == 0 ? 500 : 10);
				if (level != 1 && level % 6 == 1) {
					if (isGetRare)
						isGetRare = false;
					else
						min = 1000;
				}
				int n = Random.Range (0, max);
				if (n < min) {
					isGetRare = true;
					LayoutObjectAtRandom_RareItem (ResourcesManager.instance.rareItemPool, 1, 1);
				}
			}
			
			//Determine number of enemies based on current level number, based on a logarithmic progression
			int enemyCount = columns * rows * (int)Mathf.Log(level, 2f) / 60;

			if (level % 6 == 0)
				enemyCount = columns * rows / 11;
			
			//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom_Enemy (ResourcesManager.instance.enemyPool, enemyCount, enemyCount);

			max = 1000;
			min = (level % 4 == 0 ? 500 : 10);
			int ra = Random.Range (0, max);
			if (ra < min) {
				LayoutObjectAtRandom_Enemy (ResourcesManager.instance.bossPool, 1, 1);
			}

			
			//Instantiate the exit tile in the upper right hand corner of our game board
			Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);

		}
	}
}
