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

    public List<Vector3> all_objects_loc_list;

    // Use this for initialization
    void Start()
    {

        cubeLength = cube.transform.GetComponent<Renderer>().bounds.size.x;
        all_objects_loc_list = new List<Vector3>();

        int index = Random.Range(0, 4);
        int remote_loc = 90;
        int[,] remote_loc_list = new int[,] { { remote_loc, remote_loc }, { -remote_loc, remote_loc }, { remote_loc, -remote_loc }, { -remote_loc, remote_loc } };
        distinationObject.transform.position = new Vector3(remote_loc_list[index, 0], 2.5f, remote_loc_list[index, 1]);
  
        all_objects_loc_list.Add(distinationObject.transform.position);
        all_objects_loc_list.Add(target.position);

        // Debug.Log("length: " + cubeLength);
        
        for(int i = 0;i < num; i++)
        {
            float x;
            float z;

            
            while (true)
            {
                x = Random.Range(-95, 95);
                z = Random.Range(-95, 95);
                bool flag = true;
                
                foreach(Vector3 v in all_objects_loc_list)
                {
                    if (Mathf.Abs(x-v.x) >= Mathf.Sqrt(2)*cubeLength ||
                        Mathf.Abs(z - v.z) >= Mathf.Sqrt(2) * cubeLength
                        )
                    {
                        continue;
                    }
                    if ((x - v.x)* (x - v.x) + (z - v.z) * (z - v.z) >= 2*cubeLength* cubeLength)
                    {
                        continue;
                    }
                    flag = false;
                    break;
                }
                
                if (flag)
                {
                    break;
                }
            }

            Vector3 instantiate_loc = new Vector3(x, 2.5f, z);
            Instantiate(
                cube, 
                instantiate_loc, 
                Quaternion.identity
             );
            all_objects_loc_list.Add(instantiate_loc);
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
