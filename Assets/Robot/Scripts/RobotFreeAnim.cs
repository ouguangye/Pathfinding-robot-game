using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotFreeAnim : MonoBehaviour {

	// 决定使用何种方法
	public int mode = 0; // 0 -> APF, 1 -> AStar, 2 -> Mix
	int pathAcc = 1;

	// 使用人工势场法
	public APF apf;
	Vector3 tarDir = Vector3.zero;

	List<Point> path;
	List<Point> tarArea = new List<Point>();
	int index = 0;

	// 获取目标位置
	int added = 0;
	GameObject distination;

    // A* 下一个目标的位置
    private Vector3 nextDistination;

    // 判断机器人是否已经找到目标
    private bool isFinished = false;

	// 双相机对象
	public GameObject follow_camera;
	public GameObject vertical_camera;

    // 结束画面
    public GameObject panel;
	
	Vector3 rot;
	float rotSpeed = 40f;
	float period = 1;
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
		float distance = 10f;
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
		tarDir[1] = Vector3.SignedAngle(Vector3.forward, apf.calF(transform.position, distinationVector[0], blockVectors), Vector3.up);
		Debug.Log("当前欧拉角："+transform.eulerAngles);
		Debug.Log("目标角度："+tarDir);
	}

	// AStar部分
	void forwardOfAStar(){
		if(index == path.Count) return ;
        if(isFinished) return;
        anim.SetBool("Walk_Anim", true);
        transform.LookAt(nextDistination);
		transform.position = Vector3.MoveTowards(transform.position, nextDistination,normalSpeed*Time.deltaTime);
        if(transform.position == nextDistination) {
            ++index;
            nextDistination = new Vector3(path[index].x, transform.position.y, path[index].z);
            Debug.Log("index: "+index+"(" + nextDistination.x+", "+nextDistination.z); 
        }
	}

    void forward(float speed)
	{
        if(isFinished) return;
		anim.SetBool("Walk_Anim", true);
        transform.Translate(Vector3.forward*Time.deltaTime * speed);
	}

	// Mix部分
	void MixInit(){
		// 只选取路径当中的部分点作为中间目标点
		pathAcc = follow_camera.GetComponent<camera>().robot_map.mapsize;
		apf = new APF(pathAcc, follow_camera.GetComponent<camera>().robot_map.mapsize);
		for (int i = 0; i < path.Count;++i){
			if(i%(path.Count/pathAcc) == 0 && i != 0)
				distinationVector.Add(new Vector3(path[i].x, transform.position.y, path[i].z));
		}
		distinationVector.Add(new Vector3(path[path.Count-1].x, transform.position.y, path[path.Count-1].z));
	}

	void MixCheck(){
		int offset = 4;
		if(distinationVector.Count == 0) return ;
		if(transform.position.x > distinationVector[0].x - offset && transform.position.x < distinationVector[0].x + offset &&
		transform.position.z > distinationVector[0].z - offset && transform.position.z < distinationVector[0].z + offset){
			distinationVector.RemoveAt(0);
		}
		Debug.Log("temp distination is" + distinationVector[0]);
	}

	// 旋转
	void Rotation(){
		if(rot[1] != tarDir[1]){
			if(mode == 1)
				rot[1] = tarDir[1];
			else{
				if(Mathf.Abs(rot[1]-tarDir[1]) > 180f){
					rot[1] += rotSpeed * Time.fixedDeltaTime;
				} else {
					rot[1] -= rotSpeed * Time.fixedDeltaTime;
				}
			}
		}
	}

    IEnumerator findPathByAStarBefore() {
        ThreadManager threadManager = new ThreadManager(
            transform.position.x, 
            transform.position.z,
            distination.transform.position.x,
            distination.transform.position.z, 
            follow_camera.GetComponent<camera>().robot_map
        );
        threadManager.Start();
        while(!threadManager.isOver){
            yield return new WaitForEndOfFrame();
        }

        path = threadManager.getPath();

        added = 1;
		if(path.Count == 0) {
            Debug.Log("Wrong");
        }
        else {
            nextDistination = new Vector3(path[index].x, transform.position.y, path[index].z);
            Debug.Log("index: "+index+"(" + nextDistination.x+", "+nextDistination.z);
        }
        yield return null;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Destination"){
            anim.SetBool("Open_Anim", false);
            isFinished = true;
            panel.SetActive(true);
        }
    }



	// Use this for initialization
	void Awake(){
		anim = gameObject.GetComponent<Animator>();
		gameObject.transform.eulerAngles = rot;
		// normalSpeed = this.mode == 1? 0.4f:6f;
        normalSpeed = 6f;
	}


    void Start() {
        mode = globalVariable.mode;
    }

	// Update is called once per frame
	void Update(){
        CheckKey();
		gameObject.transform.eulerAngles = rot;

		if(added == 0){
            if(mode == 0) {
				distination = GameObject.FindGameObjectsWithTag("Destination")[0];
        		distinationVector.Add(new Vector3(distination.transform.position.x, 2.5f, distination.transform.position.z));
				apf = new APF(pathAcc, follow_camera.GetComponent<camera>().robot_map.mapsize);
                added = 2;
            }
			else {
				distination = GameObject.FindGameObjectsWithTag("Destination")[0];
				StartCoroutine("findPathByAStarBefore");	
			}
			return;
		}

        if(added == 1) {
            if(mode == 2) {
                MixInit();
                added = 2;
            }
        }
		
		if(mode == 0){
			Rotation();
			forward(normalSpeed);
			// 周期性使用射线检测前方物体
			if(time == 0){
				Look();
			}
			time += Time.deltaTime;
			if(time >= period){
				time = 0;
			}
		} else if(mode == 1) {
			forwardOfAStar();
			time += Time.deltaTime;
			if(time >= period){
				time = 0;
			}
		} else {
			MixCheck();
			Rotation();
			forward(normalSpeed);
			// 周期性使用射线检测前方物体
			if(time == 0){
				Look();
			}
			time += Time.deltaTime;
			if(time >= period){
				time = 0;
			}
		}
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
