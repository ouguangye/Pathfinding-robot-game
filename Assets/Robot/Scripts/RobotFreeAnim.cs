using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFreeAnim : MonoBehaviour {

	// 决定使用何种方法
	public int mode = 0; // 0 -> APF, 1 -> AStar, 2 -> Mix

	// 使用人工势场法
	public APF apf = new APF();
	Vector3 tarRot = Vector3.zero;

	// 使用A星
	public AStar astar;
	List<Point> path;
	List<Point> tarArea = new List<Point>();
	int index = 0;

	// 获取目标位置
	bool added = false;
	GameObject distination;

	// 双相机对象
	public GameObject follow_camera;
	public GameObject vertical_camera;
	
	Vector3 rot = Vector3.zero;
	float rotSpeed = 40f;
	Animator anim;
	float normalSpeed;

	// 目标位置
	List<Vector3> distinationVector = new List<Vector3>();

	// 障碍物位置
	List<Vector3> blockVectors = new List<Vector3>();

	// APF部分
	// 射线检测周期
	private float time = 0;

	// 射出射线检测，并画出射线
	void Lookaround(Quaternion eulerAngle){
		float distance = 40;
		RaycastHit hit;
		if(Physics.Raycast(transform.position, eulerAngle * transform.forward, out hit, distance))
			distance = Vector3.Distance(transform.position, hit.collider.gameObject.transform.position);
		Debug.DrawRay(transform.position, eulerAngle * transform.forward.normalized * distance, Color.green);
		if (hit.collider != null){
			this.blockVectors.Add(hit.point);
		}
	}

	void Look(){
		this.blockVectors.Clear();
		float lookAngle = 120f;
		float lookAcc = 70f;
		Lookaround(Quaternion.identity);
		float subAngle = (lookAngle/2)/lookAcc;
		for(int i = 0; i < lookAcc; ++i){
			Lookaround(Quaternion.Euler(0,-1*subAngle*(i+1),0));
			Lookaround(Quaternion.Euler(0,subAngle*(i+1),0));
		}
		tarRot[1] = Vector3.SignedAngle(Vector3.forward, apf.calF(transform.position, distinationVector[0], blockVectors), Vector3.up);
	}

	// AStar部分
	void Refresh(){
		if(index == path.Count) return ;
		Debug.Log("index: "+index+"(" + path[index].x+", "+path[index].z+")");
		transform.position = new Vector3(path[index].x, 2.5f, path[index++].z);
	}

	// Mix部分
	void MixInit(){
		// 只选取路径当中的部分点作为中间目标点
		int pathAcc = 10;
		for (int i = 0; i < path.Count;++i){
			if(i%(path.Count/pathAcc) == 0 && i != 0)
				distinationVector.Add(new Vector3(path[i].x, transform.position.y, path[i].z));
		}
	}

	void MixCheck(){
		int offset = 2;
		if(distinationVector.Count == 0) return ;
		if(transform.position.x > distinationVector[0].x - offset && transform.position.x < distinationVector[0].x + offset &&
		transform.position.z > distinationVector[0].z - offset && transform.position.z < distinationVector[0].z + offset){
			distinationVector.RemoveAt(0);
		}
		Debug.Log("temp distination is" + distinationVector[0]);
	}

	// 旋转
	void Rotation(){
		if(rot[1] != tarRot[1]){
			Debug.Log("need rotate");
			if(mode == 1)
				rot[1] = tarRot[1];
			else{
				if(Mathf.Abs(rot[1]-tarRot[1]) > 180f){
					rot[1] += rotSpeed * Time.fixedDeltaTime;
				} else {
					rot[1] -= rotSpeed * Time.fixedDeltaTime;
				}
			}

		}
	}

	// Use this for initialization
	void Awake(){
		anim = gameObject.GetComponent<Animator>();
		gameObject.transform.eulerAngles = rot;
		tarRot = rot;
		normalSpeed = this.mode == 1? 0.4f:6f;
	}

	// Update is called once per frame
	void Update(){
		if(!added){
			distination = GameObject.FindGameObjectsWithTag("Destination")[0];
			if(mode == 1){
				astar = new AStar();
				path = astar.getPath(new Point(transform.position.x, transform.position.z),
				new Point(distination.transform.position.x, distination.transform.position.z));
				if(path.Count == 0) Debug.Log("Wrong");
			}
			if(mode == 2){
				astar = new AStar();
				path = astar.getPath(new Point(transform.position.x, transform.position.z),
				new Point(distination.transform.position.x, distination.transform.position.z));
				if(path.Count == 0) Debug.Log("Wrong");
				MixInit();
			}
			distinationVector.Add(new Vector3(distination.transform.position.x, 2.5f, distination.transform.position.z));
			added = true;
		}
		CheckKey();
		gameObject.transform.eulerAngles = rot;
		if(mode == 0){
			Rotation();
			forward2(normalSpeed);
			// 周期性使用射线检测前方物体
			if(time == 0){
				Look();
			}
			time += Time.deltaTime;
			if(time >= 1){
				time = 0;
			}
		} else if(mode == 1) {
			if(time == 0)
				Refresh();
			time += Time.deltaTime;
			if(time >= 0.1){
				time = 0;
			}
		} else {
			MixCheck();
			Rotation();
			forward2(normalSpeed);
			// 周期性使用射线检测前方物体
			if(time == 0){
				Look();
			}
			time += Time.deltaTime;
			if(time >= 1){
				time = 0;
			}
		}
	}

	void forward2(float speed)
	{
		anim.SetBool("Walk_Anim", true);
        transform.Translate(Vector3.forward*Time.deltaTime * speed);
	}

	void CheckKey()
	{

		if (Input.GetKeyDown(KeyCode.Space))
        {
			if(follow_camera.activeInHierarchy)
            {
				follow_camera.SetActive(false);
				vertical_camera.SetActive(true);
            }
			else
            {
				follow_camera.SetActive(true);
				vertical_camera.SetActive(false);
			}
        }

		// Close
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			if (!anim.GetBool("Open_Anim"))
			{
				anim.SetBool("Open_Anim", true);
			}
			else
			{
				anim.SetBool("Open_Anim", false);
			}
		}
	}

}
