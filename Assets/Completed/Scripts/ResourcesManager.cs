using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

public struct ElementIndex{
	public int first, second;
}

public class ResourcesManager : MonoBehaviour {

	public static ResourcesManager instance = null;
	private LuaState luasupport;

	public string configfilename; // lua配置文件名

	[HideInInspector] public List<EnemiesType> enemyPool = new List<EnemiesType>(), bossPool = new List<EnemiesType>();
	[HideInInspector] public List<EnemiesType>[] enemiesPool = new List<EnemiesType>[2];
	[HideInInspector] public List<ItemsType> foodPool = new List<ItemsType>(), normalItemPool = new List<ItemsType>(),
	rareItemPool = new List<ItemsType>(), environmentPool = new List<ItemsType>();
	public bool[] rareItemShow;
	[HideInInspector] public List<ItemsType>[] itemsPool = new List<ItemsType>[4];
	[HideInInspector] public List<StoryType> normalStoryPool= new List<StoryType>(), rareStoryPool= new List<StoryType>();
	[HideInInspector] public List<StoryType>[] storiesPool = new List<StoryType>[2];


	/*[HideInInspector] public List<GameObject> floorTiles,
							wallTiles,
							foodTiles,
							bossTiles,
							outerWallTiles;	

	[HideInInspector] public List<LuaFunction> effectFuncs;*/

	void Awake ()
	{
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

	public void Initialize()
	{
		enemiesPool [0] = enemyPool;
		enemiesPool [1] = bossPool;
		itemsPool [0] = foodPool;
		itemsPool [1] = normalItemPool;
		itemsPool [2] = rareItemPool;
		itemsPool [3] = environmentPool;
		storiesPool [0] = normalStoryPool;
		storiesPool [1] = rareStoryPool;

		string path = Application.streamingAssetsPath + "/" + configfilename;
		WWW tl = new WWW ("file://" + path);
		while (!tl.isDone); 
		luasupport.DoString(tl.text);
		LuaTable bundles = luasupport.GetTable ("bundles_name");
		if (bundles == null)
			return;
		foreach(string filename in bundles.Values)
		{
			LoadBundle (filename);
		}
		rareItemShow = new bool[rareItemPool.Count];
		for (int i = 0;i < rareItemShow.Length;++i) {
			rareItemShow [i] = false;
		}
	}

	bool LoadBundle(string filename)
	{
		LuaState luamenu= new LuaState ();
		string path = "file://" + Application.streamingAssetsPath + "/bundles/" + filename;
		WWW loader = new WWW(path); // 加载bundle
		//while (!loader.isDone); 
		AssetBundle bundle = loader.assetBundle;
		if (bundle == null)
			return false;
		//string[] a = bundle.GetAllAssetNames ();
		TextAsset menufile = bundle.LoadAsset<TextAsset> ("bundle_list"); // 加载bundle列表
		if (menufile == null)
			return false;
		luamenu.DoString (menufile.text);
		LuaTable list = luamenu.GetTable ("list"); // 获取资源属性表
		if (list == null)
			return false;
		foreach(LuaTable tuple in list.Values) // 分类读取表中的资源
		{
			double type0 = (double)tuple ["type"];
			float type = (float)type0;
			if (type == 0 || type == 1) { // 类型为0意味普通敌人，为1意味着Boss
				
				EnemiesType impo;
				impo.name = (string)tuple ["name"];
				impo.tile = bundle.LoadAsset<GameObject> (impo.name);
				impo.comment = (string)tuple ["comment"];
				impo.ID = (string)tuple ["ID"];
				if ((bool)tuple ["withSkill"]) {
					impo.effect = luamenu.GetFunction ((string)tuple ["effect"]);
					double rt = (double)tuple ["rate"];
					impo.rate = (float)rt;
				} else {
					impo.effect = null;
					impo.rate = 0;
				}
				//impo.tile.tag = enemyPool.Count.ToString();

				if (type == 0) {
					enemyPool.Add (new EnemiesType ());
					enemyPool [enemyPool.Count - 1] = impo;// 添加进普通敌人池
				} else {
					bossPool.Add (new EnemiesType ());
					bossPool [bossPool.Count - 1] = impo;// 添加进Boss池
				}
			} else if (type == 2 || type == 3 || type == 4 || type == 7) {// 类型为2意味食物，类型为3意味普通道具，类型为4意味稀有道具，类型为7意味地形效果

				ItemsType impo;
				impo.cur_usingtimes = 0;
				impo.number = 0;
				impo.name = (string)tuple ["name"];
				impo.tile = bundle.LoadAsset<GameObject> (impo.name);
				impo.comment = (string)tuple ["comment"];
				impo.ID = (string)tuple ["ID"];
				impo.effect = luamenu.GetFunction ((string)tuple ["effect"]);
				double utt = (double)tuple ["using_times"];
				impo.usingtimes = (float)utt;
				utt = (double)tuple ["fading_time"];
				impo.fadingtime = (int)utt;
				if (tuple ["use_direction"] != null) {
					impo.use_direction = (bool)tuple ["use_direction"];
				} else {
					impo.use_direction = false;
				}

				if (type == 2) {
					foodPool.Add (new ItemsType ());
					foodPool [foodPool.Count - 1] = impo;
				} else if (type == 3) {
					normalItemPool.Add (new ItemsType ());
					normalItemPool [normalItemPool.Count - 1] = impo;
				} else if (type == 4) {
					rareItemPool.Add (new ItemsType ());
					rareItemPool [rareItemPool.Count - 1] = impo;
				} else {
					environmentPool.Add (new ItemsType ());
					environmentPool [environmentPool.Count - 1] = impo;
				}
			} else if (type == 5 || type == 6) {// 类型为2普通剧情，类型为3意味稀有剧情
				StoryType impo;

				impo.name = (string)tuple ["name"];
				impo.tile = bundle.LoadAsset<GameObject> (impo.name);
				impo.comment = (string)tuple ["comment"];
				impo.ID = (string)tuple ["ID"];
				impo.effect = luamenu.GetFunction ((string)tuple ["effect"]);

				if (type == 5) {
					normalStoryPool.Add (new StoryType ());
					normalStoryPool [normalStoryPool.Count - 1] = impo;
				} else {
					rareStoryPool.Add (new StoryType ());
					rareStoryPool [rareStoryPool.Count - 1] = impo;
				}
			}
		}
		bundle.Unload (false);// 卸载bundle
		return true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public ElementIndex GetEnemyIndex(string ID)//通过ID获得当前敌人资源索引
	{
		ElementIndex t;
		t.first = t.second = -1;
		for (int j = 0;j < 2;++j)
			for(int i = 0;i < enemiesPool[j].Count;++i){
				if (ID == enemiesPool [j] [i].ID) {
					t.first = j;
					t.second = i;
					return t;
				}
			}
		return t;
	}

	public ElementIndex GetItemIndex(string ID)//通过ID获得当前道具资源索引
	{
		ElementIndex t;
		t.first = t.second = -1;
		for (int j = 0;j < 4;++j)
			for(int i = 0;i < itemsPool[j].Count;++i){
				if (ID == itemsPool [j] [i].ID) {
					t.first = j;
					t.second = i;
					return t;
				}
			}
		return t;
	}
		
	public ElementIndex GetStoriesIndex(string ID)//通过ID获得当前剧情资源索引
	{
		ElementIndex t;
		t.first = t.second = -1;
		for (int j = 0;j < 2;++j)
			for(int i = 0;i < storiesPool[j].Count;++i){
				if (ID == storiesPool [j] [i].ID) {
					t.first = j;
					t.second = i;
					return t;
				}
			}
		return t;
	}
}
