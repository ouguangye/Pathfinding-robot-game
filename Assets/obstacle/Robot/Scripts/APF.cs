using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APF{
    public float att_g = 20f;
    public float rep_g = 10f;
    public float d_goal = 20f;
    
    private Vector3 calAtt(Vector3 cur, Vector3 end){
        float r = Vector3.Distance(cur, end);
        Vector3 dir = Vector3.Normalize(cur-end);
        if(r <= d_goal)
            return 0.5f * att_g * dir * r * r;
        else
            return dir * (d_goal * att_g * r - 0.5f * att_g * d_goal * d_goal);
    }

    private Vector3 calRep(Vector3 cur, List<Vector3> blocks, Vector3 tar){
        Vector3 res = Vector3.zero;
        foreach(Vector3 end in blocks){
            float r = Vector3.Distance(cur, end);
            float r_goal = Vector3.Distance(cur, tar);
            Vector3 dir = Vector3.Normalize(end-cur);
            res += dir * 0.5f * rep_g * (1f/r - 1f/40f) * (1f/r - 1f/40f)*r_goal*r_goal/2f;
        }
        return res;
    }

    public Vector3 calF(Vector3 cur, Vector3 dis, List<Vector3> blocks){
        return calAtt(cur, dis) + calRep(cur, blocks, dis);
    }

}
