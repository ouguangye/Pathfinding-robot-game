using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APF{
    public float att_size = 10f;
    public float att_g;
    public float rep_g;
    public float Q = 5f;
    public float d_goal = 10f;

    public APF(int pathAcc, int mapsize){
        att_g = att_size/((float)pathAcc)*(float)mapsize*5f;
        if(pathAcc != 1){
            att_g *= 4f;
        }
        rep_g = (float)mapsize/(float)pathAcc*2f;
    }
    
    private Vector3 calAtt(Vector3 cur, Vector3 end){
        float r = Vector3.Distance(cur, end);
        Vector3 dir = Vector3.Normalize(cur-end);
        Vector3 res;
        if(r <= d_goal)
            res = dir*att_g/r/r;
        else
            res = dir*att_g/d_goal/d_goal*r;
        Debug.Log("引力: "+res);
        return res;
    }

    private Vector3 calRep(Vector3 cur, List<Vector3> blocks, Vector3 tar){
        Vector3 res = Vector3.zero;
        foreach(Vector3 end in blocks){
            float r = Vector3.Distance(cur, end);
            float r_goal = Vector3.Distance(cur, tar);
            Vector3 dir = Vector3.Normalize(end-cur);
            res += dir * rep_g / r / r;
        }
        Debug.Log("斥力: "+res);
        return res;
    }

    public Vector3 calF(Vector3 cur, Vector3 dis, List<Vector3> blocks){
        Vector3 res = calAtt(cur, dis) + calRep(cur, blocks, dis);
        // Debug.Log("合力: "+res);
        // int rand = Random.Range(0, 100);
        // if(rand == 0)
        //     return Vector3.right + res.normalized;
        // if(rand == 1)
        //     return Vector3.left + res.normalized;
        // if(rand == 2)
        //     return Vector3.forward + res.normalized;
        return res;
    }

}
