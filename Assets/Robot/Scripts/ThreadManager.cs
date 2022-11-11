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

    private List<Vector3> blocks_positions;

    private List<Point> path;
    public ThreadManager(float current_x, float current_z, float distination_x, float distination_z, List<Vector3> blocks_positions){
        this.current_x = current_x;
        this.current_z = current_z;
        this.distination_x = distination_x;
        this.distination_z = distination_z;
        this.blocks_positions = blocks_positions;

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
        AStar astar = new AStar(blocks_positions);
        Debug.Log("start astar");
        path = astar.getPath(
            new Point(current_x,current_z),
            new Point(distination_x, distination_z)
        );
        isOver = true;
        thread.Abort();
    }
}