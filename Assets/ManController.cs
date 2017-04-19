using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ManController : MonoBehaviour {

	private Rigidbody2D r;
	private GameObject camera;

	//设定人物的朝向（改变transform的scale的x值，为负数则水平翻转）
	public int direction = 1;
	//定义角色的初始速度
	public float speed = 0.0f;

	// enemy对象
	public Enemy tmp;
    private Animator animator;
	// 按键检测，防止长按，每次只检测一个完整的按下和释放
	private bool pressing = false;
	// Use this for initialization
	// 初始化的时候获取Animator对象
	void Start () {
        animator = GetComponent<Animator>();
		r = GetComponent<Rigidbody2D>();
		camera = GameObject.Find("Camera");
	}
		
	// Update is called once per frame
	// 每帧更新对象
	void Update () {

		if (camera.transform.position.x != transform.position.x)
			camera.transform.position = new Vector3 (transform.position.x, camera.transform.position.y, camera.transform.position.z);

		//Debug.Log (r.velocity);
		//移动
		//移动的水平分量（Raw只取整数）
		float moveHorizontal = Input.GetAxisRaw("Horizontal");
		//移动的垂直分量
		float moveVertical = Input.GetAxisRaw("Vertical");
		//如果在水平方向上有移动
		//Debug.Log(moveHorizontal);

		if (transform.position.y - transform.position.z * Mathf.Tan (3.14159f / 3) > 0.01f) {
			if (moveHorizontal != 0) {
				//根据获取的值的符号来决定人物的朝向，注意转换成int类型之后，direction的取值为-1或1
				direction = (int)moveHorizontal;
				//r.velocity = new Vector2 (moveHorizontal * 0.8f, r.velocity.y);
				if ((r.velocity.x - moveHorizontal * 2.0f <= 0 && moveHorizontal > 0)||
					(r.velocity.x - moveHorizontal * 2.0f >= 0 && moveHorizontal < 0))
					r.AddForce (new Vector2 (moveHorizontal * 2.0f, 0.0f));
				    gameObject.GetComponent<PolygonCollider2D>().enabled = false;
			}
		} else {
			if (moveHorizontal != 0) {
				//根据获取的值的符号来决定人物的朝向，注意转换成int类型之后，direction的取值为-1或1
				direction = (int)moveHorizontal;
				r.velocity = new Vector2 (moveHorizontal * 4.0f, r.velocity.y);
			} else {
				r.velocity = new Vector2 (0.0f, r.velocity.y);
			}

			//计算移动向量，注意两条轴之间存在投影比例关系
			if (moveVertical != 0) {
				Vector3 movement = new Vector3 (0.0f, moveVertical * Time.deltaTime * Mathf.Tan (3.14159f / 3), moveVertical * Time.deltaTime);
				//更新角色当前的位置
				transform.position = transform.position + movement;
			}
		}

			//更改scale属性，若x值小于0，则进行水平翻转
		transform.localScale = new Vector3 (direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
			


		//跳跃下落逻辑
		//注意计算的时候y轴和z轴的坐标之间存在投影转换关系（乘上tan比例系数）
		if (transform.position.y - transform.position.z * Mathf.Tan (3.14159f / 3) > 0.01f) {
			/*//重力影响，v = at
			speed += 0.98f * Time.deltaTime;
			//计算新的位置
			Vector3 newp = transform.position;
			//下落的时候y轴位置的更新规则
			newp.y -= speed * Time.deltaTime;
			//更新位置
			transform.position = newp;*/
			r.gravityScale = 0.5f;
		} else {
			//判断落地
			Vector3 newp = transform.position;
			newp.y = newp.z * Mathf.Tan (3.14159f / 3);
			speed = 0.0f;
			transform.position = newp;
			r.gravityScale = 0.0f;
			r.velocity = new Vector2(r.velocity.x, 0.0f);

		}

		//获取键盘事件
		if (!pressing)
		{
	        bool trigger_space = Input.GetKeyDown("space");
			bool trigger_j = Input.GetKeyDown (KeyCode.J);//斜抛和竖直上抛 斜抛要考虑水平速度

			if (trigger_j) {
				//跳跃上升逻辑
				if (transform.position.y - transform.position.z * Mathf.Tan (3.14159f / 3) < 0.01f)
				{
					//transform.position = transform.position + new Vector3 (0, 0.8f, 0);
					r.velocity = new Vector2(r.velocity.x, r.velocity.y + 2.0f);
					animator.SetTrigger ("jump");
					gameObject.GetComponent<PolygonCollider2D>().enabled = false;
				}
			}

			//如果检测到键盘事件
	        if (trigger_space)
	        {
				//防止每一帧都长按
				pressing = true;
	            animator.SetTrigger("A");//为当前GameObject绑定的Animator激活一个名为A的Trigger
				
	        }//OK

		}
		else pressing = false;

	}

	public void attack(float distance)
	{
		r.velocity = new Vector2 (direction * 100.0f, r.velocity.y);
		Vector2 start, end;
		start = new Vector2 (transform.position.x, transform.position.y);
		//终点方向根据direction决定
		end = start + (new Vector2 (distance, 0)) * direction;
		//在直线检测的时候先关闭碰撞体，防止直线将其弹飞
		gameObject.GetComponent<PolygonCollider2D>().enabled = false;
		//直线检测, 从start开始到end之间返回第一个带有collider Component的对象封装的RaycastHIt2d对象
		RaycastHit2D ret = Physics2D.Linecast(start, end, 0x01, 0, 2);//最后三个参数分别是图层掩码，最小，最大深度
		//重新开启碰撞体
		gameObject.GetComponent<PolygonCollider2D>().enabled = true;
		if (ret.transform != null)
		{
			// 击中了！
			animator.speed = 0.2f;
			StartCoroutine (Pause(0.2f));//短暂的停顿
			tmp = ret.transform.gameObject.GetComponent<Enemy>();
			if (tmp != null) {
				tmp.beAtked (5);
			}
		}
	}


	public void attack_end ()
	{
		r.velocity = new Vector2 (0, r.velocity.y);
	}

	IEnumerator Pause(float duration)
	{
		yield return new WaitForSeconds(duration);
		animator.speed = 1.0f;
	}
		
	protected void OnTriggerEnter2D(Collider2D c)
	{
		Vector2 velTemp = new Vector2 (r.velocity.x*Time.deltaTime, 1.0f);
		//r.velocity = new Vector2(0.0f, 0.0f);
		animator.SetTrigger ("collision");
		r.velocity = velTemp;
		//r.AddForce (velTemp);
		Debug.Log (r.velocity);
		//r.AddForce (5.0f, 5.0f);
	}
}