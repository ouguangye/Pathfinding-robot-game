using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMap {
    // 二维地图存的是每个方块左上角的点
    // 二维地图和世界地图坐标转换关系为： 
    // z = (mapsize/2-i)*cubeLength 
    // x = (j-mapsize/2)*cubeLength
    public int[,] map;

    public int mapsize;

    private int cubeLength;

    public RobotMap(int mapsize, int cubeLength) {
        this.mapsize = mapsize;
        this.cubeLength = cubeLength;
    }

    // 测试该点是否合法
    public bool isCanReach(int i, int j){
        if(i<0||j<0||i>=mapsize||j>=mapsize)
            return false;
        return map[i,j] == 0 || map[i,j] == 2 || map[i,j] == 3;
    }

    // 传入二维数据的i,j值，返回物体的中心坐标
    public Vector3 mapToPoint(int i, int j) {
        float x = ((float)j-(float)mapsize/2f+0.5f)*cubeLength;
        float z = ((float)mapsize/2f-(float)i-0.5f)*cubeLength;
        return new Vector3(x, 2.5f, z);
    }

    public Vector3 mapToPoint(int i, int j, float y) {
        float x = ((float)j-(float)mapsize/2f+0.5f)*cubeLength;
        float z = ((float)mapsize/2f-(float)i-0.5f)*cubeLength;
        return new Vector3(x, y, z);
    }

    // 接收物体中心的 x, z坐标， 返回地图上的 i,j位置
    public Vector2 pointFillMap(float x, float z) {
        // 将中心坐标转换为左上角的坐标
        float point_x = x - cubeLength/2;
        float point_z = z + cubeLength/2; 
        float i = mapsize/2-point_z/(int)cubeLength;
        float j = mapsize/2+point_x/(int)cubeLength;

        // map[point_x/(int)cubeLength+mapsize/2, point_z/(int)cubeLength+mapsize/2-1] = 1;
        return new Vector2(i,j);
    }

    public Vector3 findLongestPathFromHorizontal() {
        int maxLen = 0;
        int max_i = 0 , max_j = 0;

        // 寻找一行中 最长可运动的 区域
        for(int i = 0;i<mapsize;i++) {
            int len = 0;
            bool flag = false;
            for(int j = 0 ; j<mapsize;j++) {
                if(map[i,j] == 1 || map[i,j] == 2){
                    if(!flag) continue;
                    if(len > maxLen) {
                        maxLen = len;
                        max_i = i;
                        max_j = j-1;
                    }
                    len = 0;
                    flag = false;
                    continue;
                }
                flag = true;
                len++;
            }
            if(len > maxLen) {
                maxLen = len;
                max_i = i;
                max_j = mapsize-1;
            }
        }

        Debug.Log(max_i + " " + max_j + " " + maxLen);
        for (int i = 1; i<=maxLen;i++){
            map[max_i, max_j-i+1] = 2;
        }
        return new Vector3(max_i, max_j, maxLen);
    }

    public Vector3 findLongestPathFromVertical() {
        int maxLen = 0;
        int max_i = 0 , max_j = 0;

        // 寻找一行中 最长可运动的 区域
        for(int i = 0;i<mapsize;i++) {
            int len = 0;
            bool flag = false;
            for(int j = 0 ; j<mapsize;j++) {
                if(map[j,i] == 1 || map[j,i] == 2){
                    if(!flag) continue;
                    if(len > maxLen) {
                        maxLen = len;
                        max_i = i;
                        max_j = j-1;
                    }
                    len = 0;
                    flag = false;
                    continue;
                }
                flag = true;
                len++;
            }
            if(len > maxLen) {
                maxLen = len;
                max_i = i;
                max_j = mapsize-1;
            }
        }

        Debug.Log(max_i + " " + max_j + " " + maxLen);
        for (int i = 1; i<=maxLen;i++){
            map[max_j-i+1, max_i] = 2;
        }
        return new Vector3(max_i, max_j, maxLen);
    }
}