using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform target;
    public float distance = 10.0f;                  //设置距离
    public float height = 5.0f;                     //设置高度
    public float offsetAngleY = 0;                  //观察角度offset
    float heightDamping = 2.0f;
    float rotationDamping = 3.0f;

    public GameObject cube;
    private int num = 100; 

    public GameObject distinationObject;

    float cubeLength;

    int[,] map;

    // 传入的x,z 是中心点坐标
    Vector3 mapToPoint(int i, int j) {
        return new Vector3((i-10)*10,2.5f,(j-9)*10);
    }

    // 接收物体的x, z值
    void pointFillMap(float x, float z) {
        // 将中心坐标转换为左上角的坐标
        int point_x = (int)(x - cubeLength/2);
        int point_z = (int)(z + cubeLength/2);
        map[point_x/10+10, point_z/10+9] = 1;
    }

    // Use this for initialization
    void Start()
    {

        // 获取各个物体的大小
        cubeLength = cube.transform.GetComponent<Renderer>().bounds.size.x;

        Vector3 size = target.transform.GetComponent<BoxCollider>().bounds.size;
        Debug.Log("size: " + size);

        // 初始化map, 一律统计方块左上角的点
        map = new int[20,20];

        // 对于目标节点， 在地图的四个角的任意一个角上
        int index = Random.Range(0, 4);
        
        int[,] remote_loc_list = new int[,] {
           {0,0},{0,19},{19,0},{19,19}
        };

        distinationObject.transform.position = mapToPoint(remote_loc_list[index,0], remote_loc_list[index, 1]);
        target.transform.position = mapToPoint(remote_loc_list[3-index,0], remote_loc_list[3-index, 1]);

        map[remote_loc_list[index,0], remote_loc_list[index, 1]] = 1;
        map[remote_loc_list[3-index,0], remote_loc_list[3-index, 1]] = 1;
        

        // 生成静态障碍物
        for(int i = 0;i < num; i++)
        {
            int x;
            int z;
            
            while (true)
            {
                x = Random.Range(0, 19);
                z = Random.Range(0, 19);
                if(map[x,z] == 0) break;
            }

            Instantiate(
                cube, 
                mapToPoint(x,z), 
                Quaternion.identity
            );
            map[x,z] = 1;
        }

        // 生成动态障碍物
        int maxLen = 0;
        int max_i, max_j;
        for(int i = 0;i<20;i++) {
            int len = 0;
            bool flag = false;
            for(int j = 0 ; j<20;j++) {
                if(map[i,j] == 0){
                    if(!flag) continue;
                    if(len <= maxLen) {
                        len = 0;
                    }
                    else {
                        maxLen = len;
                        max_i = i;
                        max_j = j;
                    }
                }
                flag = true;
                len++;
            }
            if(len > maxLen) {
                maxLen = len;
                max_i = i;
                max_j = 19;
            }
        }
    }

    void Update()
    {
        
    }
 
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
