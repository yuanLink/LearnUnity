using UnityEngine;
using System.Collections;

public enum ElementType {
	food,
	item,
	story
};

public class ElementProperty : MonoBehaviour {

	public float brokenTime = 1.0f;
	public ElementType type;
	public string ID;
	public ElementIndex index;

	// Use this for initialization

	void Start () {
		if (type == ElementType.food || type == ElementType.item) {
			index = ResourcesManager.instance.GetItemIndex (ID);
			ItemsType f = ResourcesManager.instance.itemsPool [index.first] [index.second];
			if (f.fadingtime > 0) {
				Completed.GameManager.instance.AddItemToFaing (this, f.fadingtime);
			}
		} else {
			index = ResourcesManager.instance.GetStoriesIndex (ID);
		}
	}

	public IEnumerator Broken()
	{
		Animator at = gameObject.GetComponent<Animator> ();
		if (at != null) {
			at.SetTrigger ("ItemBroken"); // 播放损坏动画
		}
		yield return new WaitForSeconds (brokenTime);
		gameObject.SetActive (false);
	}

	public void DoEffect(Completed.MovingObject target)
	{
		ResourcesManager.instance.itemsPool [index.first] [index.second].effect.Call (target);
		gameObject.SetActive (false);
	}

	public void Destroy()
	{
		gameObject.SetActive (false);
	}
}
