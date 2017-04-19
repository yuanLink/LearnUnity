using UnityEngine;
using System.Collections;
using LuaInterface;

public struct ItemsType  {
	public string ID;
	public GameObject tile;
	public int number;
	public bool use_direction;
	public LuaFunction effect;
	public string name;
	public string comment;
	public float usingtimes;
	public int cur_usingtimes;
	public int fadingtime;
};

public struct EnemiesType  {
	public string ID;
	public GameObject tile;
	public LuaFunction effect;
	public float rate;
	public string name;
	public string comment;
};

public struct StoryType  {
	public string ID;
	public GameObject tile;
	public LuaFunction effect;
	public string name;
	public string comment;
};
