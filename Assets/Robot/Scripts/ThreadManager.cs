using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadManager {
    public bool isOver=false;

    private Thread thread;

    private float current_x;
    private float current_z;

    private float distination_x;
    private float distination_z;

    private RobotMap map;

    private List<Point> path;
    public ThreadManager(float current_x, float current_z, float distination_x, float distination_z, RobotMap map){
        this.current_x = current_x;
        this.current_z = current_z;
        this.distination_x = distination_x;
        this.distination_z = distination_z;
        this.map = map;
        Debug.Log("thread constru");
        thread = new Thread(Cal);
    }
    public void Start() {
        isOver = false;
        thread.Start();
    }

    public List<Point> getPath() {
        return path;
    }

    private void Cal() {
        AStar astar = new AStar(map);
        Debug.Log("start astar");
        path = astar.getPath(
            new Point(current_x,current_z),
            new Point(distination_x, distination_z)
        );
        isOver = true;
        thread.Abort();
    }
}