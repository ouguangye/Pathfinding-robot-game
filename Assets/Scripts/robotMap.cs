using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMap {
    // 二维地图存的是每个方块左上角的点
    // 二维地图和世界地图坐标转换关系为： 
    // x = (i-mapsize/2)*cubeLength 
    // z = (j-mapsize/2+1)*cubeLength
    public int[,] map;

    private int mapsize;

    private int cubeLength;

    public RobotMap(int mapsize, int cubeLength) {
        this.mapsize = mapsize;
        this.cubeLength = cubeLength;
    }

    // 传入二维数据的i,j值，放回物体的中心坐标
    public Vector3 mapToPoint(int i, int j) {
        return new Vector3((i-mapsize/2)*cubeLength+cubeLength/2,2.5f,(j-mapsize/2+1)*cubeLength-cubeLength/2);
    }

    // 接收物体的x, z值
    public void pointFillMap(float x, float z) {
        // 将中心坐标转换为左上角的坐标
        int point_x = (int)(x - cubeLength/2);
        int point_z = (int)(z + cubeLength/2);

        map[point_x/(int)cubeLength+mapsize/2, point_z/(int)cubeLength+mapsize/2-1] = 1;
    }

    public Vector3 findLongestPathFromHorizontal() {
        int maxLen = 0;
        int max_i = 0 , max_j = 0;

        // 寻找一行中 最长可运动的 区域
        for(int i = 0;i<mapsize;i++) {
            int len = 0;
            bool flag = false;
            for(int j = 0 ; j<mapsize;j++) {
                if(map[i,j] == 1){
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
            map[max_i, max_j-i+1] = 1;
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
                if(map[j,i] == 1){
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
            map[max_j-i+1, max_i] = 1;
        }
        return new Vector3(max_i, max_j, maxLen);
    }
}