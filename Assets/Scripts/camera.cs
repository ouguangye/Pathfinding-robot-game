using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public float distanceUp = 4f;//相机与目标的竖直高度参数
    public float distanceAway = 20f;//相机与目标的水平距离参数
    public float smooth = 3f;//位置平滑移动插值参数值
    public float camDepthSmooth = 20f;

    //���������
    public Transform followObject;
    //�������λ��
    Vector3 followVector;


    // ��¡�ľ�̬�ϰ���
    public GameObject cube;
    private int num = 100; // ����

    // Ŀ�ĵ�
    public GameObject distinationObject;

    // cube�Ĵ�С
    float cubeLength;

    // ��¼�����������������
    public List<Vector3> all_objects_loc_list;

    // Use this for initialization
    void Start()
    {

        followVector = this.transform.position - followObject.position;

        cubeLength = cube.transform.GetComponent<Renderer>().bounds.size.x;
        all_objects_loc_list = new List<Vector3>();

       // ������� �յ㵽 �������ҵ��ĸ�����ı�Ե����
        int index = Random.Range(0, 4);
        int remote_loc = 90;
        int[,] remote_loc_list = new int[,] { { remote_loc, remote_loc }, { -remote_loc, remote_loc }, { remote_loc, -remote_loc }, { -remote_loc, remote_loc } };
        distinationObject.transform.position = new Vector3(remote_loc_list[index, 0], 2.5f, remote_loc_list[index, 1]);
  
        all_objects_loc_list.Add(distinationObject.transform.position);
        all_objects_loc_list.Add(followObject.position);

        Debug.Log("length: " + cubeLength);
        
        for(int i = 0;i < num; i++)
        {
            float x;
            float z;

            // �ҳ�x, z�ĺ��ʵ����ֵ
            while (true)
            {
                x = Random.Range(-95, 95);
                z = Random.Range(-95, 95);
                bool flag = true;
                // ���� all_objeccts_loc_list, �жϸ�����ϴεĵ��Ƿ�Ϸ�
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
                // ��������������� ��ô��ֱ��break; ���򣬼���ѭ�����ҵ����ʵ������
                if (flag)
                {
                    break;
                }
            }

            // ��¡����
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
        // 鼠标轴控制相机的远近
        if ((Input.mouseScrollDelta.y < 0 && Camera.main.fieldOfView >= 3) || Input.mouseScrollDelta.y > 0 && Camera.main.fieldOfView <= 80)
        {
            Camera.main.fieldOfView += Input.mouseScrollDelta.y * camDepthSmooth * Time.deltaTime;
        }
    }
 
    void LateUpdate()
    {
        //计算出相机的位置
        Vector3 disPos = followObject.position + Vector3.up * distanceUp - followObject.forward * distanceAway;
 
        transform.position = Vector3.Lerp(transform.position, disPos, Time.deltaTime * smooth);
        //相机的角度
        transform.LookAt(followObject.position);
    }
}
