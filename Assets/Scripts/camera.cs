using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    // 地图大小
    public int mapsize=20;
    
    // 障碍物数量
    private int obstacleNum; 

    // 相机相关参数
    public Transform target;
    public float distance = 10.0f;                  //设置距离
    public float height = 5.0f;                     //设置高度
    public float offsetAngleY = 0;                  //观察角度offset
    float heightDamping = 2.0f;
    float rotationDamping = 3.0f;

    // 静态障碍物预设体
    public GameObject cube;
    // 动态障碍物 预设体
    public GameObject movingCube;
    // 墙体 预设体
    public GameObject wallCube;

    // 终点
    public GameObject distinationObject;

    // 障碍物的长度大小
    private float cubeLength;

    public RobotMap robot_map;

    void createHorizontalMoveObstract() {
        Vector3 res =  robot_map.findLongestPathFromHorizontal();
        GameObject m_cube = Instantiate(
            movingCube,
            robot_map.mapToPoint((int)(res[0]),(int)(res[1]-res[2]+1)),  
            Quaternion.identity
        );
        m_cube.GetComponent<movingObstacle>().endPoint = robot_map.mapToPoint((int)res[0],(int)res[1]);
    }

    void createVerticalMoveObstract() {
        Vector3 res =  robot_map.findLongestPathFromVertical();
        // Debug.Log("len: " + maxLen);
        GameObject m_cube = Instantiate(
            movingCube,
            robot_map.mapToPoint((int)(res[1]-res[2]+1),(int)(res[0])),  
            Quaternion.identity
        );
        m_cube.GetComponent<movingObstacle>().endPoint = robot_map.mapToPoint((int)res[1],(int)res[0]);
    }

    void buildWall() {
        // 上
        for(int i = -1;i<=mapsize;i++) {
            GameObject w_cube = Instantiate(
                wallCube, 
                robot_map.mapToPoint(i,mapsize), 
                Quaternion.identity
            );
            w_cube.transform.parent = wallCube.transform.parent;
        }
        // 下 
        for(int i = -1;i<=mapsize;i++) {
            Instantiate(
                wallCube, 
                robot_map.mapToPoint(i,-1), 
                Quaternion.identity
            );
        }
        // 左
        for(int i = 0;i<mapsize;i++) {
            Instantiate(
                wallCube, 
                robot_map.mapToPoint(-1,i), 
                Quaternion.identity
            );
        }
        // 右
        for(int i = 0;i<mapsize;i++) {
            Instantiate(
                wallCube, 
                robot_map.mapToPoint(mapsize,i), 
                Quaternion.identity
            );
        }
    }

    void buildStatiObstacles() {
        for(int i = 0;i < obstacleNum; i++)
        {
            int x;
            int z;
            
            while (true)
            {
                x = Random.Range(0, mapsize);
                z = Random.Range(0, mapsize);
                if(robot_map.map[x,z] == 0) break;
            }

            Instantiate(
                cube, 
                robot_map.mapToPoint(x,z), 
                Quaternion.identity
            );
            robot_map.map[x,z] = 1;
        }
    }

    // Use this for initialization
    void Start()
    {

        mapsize = globalVariable.mapsize;

        // 获取各个物体的大小
        cubeLength = cube.transform.GetComponent<Renderer>().bounds.size.x;
        Debug.Log("cube size: " + cubeLength);
        Vector3 size = target.transform.GetComponent<BoxCollider>().bounds.size;
        Debug.Log("robot size: " + size);

        // 初始化map
        robot_map = new RobotMap(mapsize, (int)cubeLength);
        robot_map.map = new int[mapsize,mapsize];

        obstacleNum = mapsize*mapsize/10;

        // 对于目标节点， 在地图的四个角的任意一个角上
        int index = Random.Range(0, 4);
        int[,] remote_loc_list = new int[,] {
           {0,0},{0,mapsize-1},{mapsize-1,0},{mapsize-1,mapsize-1}
        };

        distinationObject.transform.position = robot_map.mapToPoint(remote_loc_list[index,0], remote_loc_list[index, 1]);
        target.transform.position = robot_map.mapToPoint(remote_loc_list[3-index,0], remote_loc_list[3-index, 1], target.transform.position.y);

        // 设置终点为1， 以及周围两个点也为1
        Debug.Log("destination point: " +  remote_loc_list[index,0] + " " + remote_loc_list[index, 1]);
        robot_map.map[remote_loc_list[index,0], remote_loc_list[index, 1]] = 3;
        robot_map.map[remote_loc_list[index,0] + (index < 2? 1: -1) , remote_loc_list[index, 1]] = 3;
        robot_map.map[remote_loc_list[index,0], remote_loc_list[index, 1] + (index % 2 == 0 ? 1: -1)] = 3;

        robot_map.map[remote_loc_list[3-index,0], remote_loc_list[3-index, 1]] = 3;
        

        // 生成墙
        buildWall();

        // 生成静态障碍物
        buildStatiObstacles();

        // 生成动态障碍物
        // Debug.Log("num: " + mapsize + " " + cubeLength + " " + (int)mapsize*mapsize/(cubeLength*cubeLength*4));
        for(int i=0;i<(int)mapsize*mapsize/(cubeLength*cubeLength*4);i++) {
            createHorizontalMoveObstract();
            createVerticalMoveObstract();
        }
    }

    void Update()
    {
        
    }
 
    // 相机追踪机制
    void LateUpdate()
    {
        if(!target)      //保护机制，为空直接结束
        {
            return;
        }

        float wantedRotationAngle = target.eulerAngles.y+offsetAngleY;     //调整相机观察的角度setAngleY
        float wantedHeight = target.position.y + height;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        transform.position = target.position;
        transform.position -= currentRotation * Vector3.forward * distance;
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        transform.LookAt(target);
    }
}
