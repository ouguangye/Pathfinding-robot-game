using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point{
	public float x, z;
	public float F, G, H;
	public Point parent;
	public Point(float x, float z){
		this.x = (float)(Mathf.Floor(x) + (x - Mathf.Floor(x) > 0 ? 0.5 : 0) + (x - Mathf.Floor(x) > 0.5 ? 0.5 : 0));
		this.z = (float)(Mathf.Floor(z) + (z - Mathf.Floor(z) > 0 ? 0.5 : 0) + (z - Mathf.Floor(z) > 0.5 ? 0.5 : 0));
		this.F = 0; 
		this.G = 0;
		this.H = 0;
	}

	public float CalG(Point previous){
		float tempG = Vector2.Distance(new Vector2(previous.x, previous.z), 
		new Vector2(this.x, this.z));
		float g = tempG + previous.G;
		return g;
	}

	public float CalH(Point end){
		return Mathf.Sqrt(Mathf.Pow(end.x - this.x, 2) + Mathf.Pow(end.z - this.z, 2));
	}
}

public class AStar{
	private List<Vector3> blocks;
	private List<Point> openlist = new List<Point>();
	private List<Point> closelist = new List<Point>();
	private int[,] maze = new int[400, 400];
	private float[,] dirs = {{0.5f,0},{-0.5f,0},{0,0.5f},{0,-0.5f}};
	public string logg;
	public AStar(List<Vector3> blocks){
		// blocks = GameObject.FindGameObjectsWithTag("Obstacle");
        this.blocks = blocks;
		for(int z = 0; z < 400; ++z){
			for(int x = 0; x < 400; ++x){
				maze[z, x] = 0;
			}
		}
		foreach(Vector3 block in blocks){
			int offset = 7;
			for(float x = -offset; x <= offset; x += 0.5f){
				for(float z = -offset; z <= offset; z += 0.5f){
					Point temp = new Point(block.x + x, block.z + z);
					if(temp.z < -100f)
						temp.z = -100f;
					if(temp.x < -100f)
						temp.x = -100f;
					if(temp.x > 99.5f)
						temp.x = 99.5f;
					if(temp.z > 99.5f)
						temp.z = 99.5f;
					maze[(int)(temp.z*2)+200, (int)(temp.x*2)+200] = 1;
				}
			}
		} 
	}
	public Point getLeastFPoint(){
		if(openlist.Count != 0){
			Point res = openlist[0];
			foreach(Point point in openlist){
				if(point.F < res.F){
					res = point;
				}
			}
			return res;
		}
		return null;
	}
	Point isInList(List<Point> list, Point point){
		foreach(Point p in list){
			if(p.x == point.x && p.z == point.z){
				return p;
			}
		}
		return null;
	}
	private bool isCanReach(Point cur, Point tar){
		if(tar.x<-100f||tar.x>99.5f||tar.z<-100f||tar.z>99.5f||isInList(closelist, tar)!=null)
			return false;
		else if(maze[(int)(tar.z*2)+200, (int)(tar.x*2)+200] == 1)
			return false;
		return true;
	}
	private List<Point> getSurround(Point point){
		List<Point> res = new List<Point>();
		for(int i = 0;i<4;++i){
			float x = point.x + dirs[i, 1];
			float z = point.z + dirs[i, 0];
			Point temp = new Point(x, z);
			// Debug.Log("for point ("+ point.x +","+point.z+"), next is ("+ temp.x + "," + temp.z + ")");
			if(isCanReach(point, temp)){
				res.Add(temp);
			}
		}
		return res;
	}
	public Point findPath(Point start, Point end){
		openlist.Add(start); 
		Debug.Log(end.x + "," + end.z);
		int times = 0;
		while(openlist.Count>0 && times++ < 100000){
			Point curPoint = getLeastFPoint();
			openlist.Remove(curPoint);
			closelist.Add(curPoint);
			List<Point> surround = getSurround(curPoint);
			foreach (var s in surround){
				if(isInList(openlist, s) == null){
					s.parent = curPoint;
					s.G = s.CalG(curPoint);
					s.H = s.CalH(end);
					s.F = s.G + s.H;
					// Debug.Log("for point ("+curPoint.x+", "+ curPoint.z+ "), next (" + s.x + ", " + s.z + ")'s F = " + s.F);
					openlist.Add(s);
				} else {
					Point old = isInList(openlist, s);
					if(old.G > s.CalG(curPoint)){
						old.G = s.CalG(curPoint);
						old.parent = curPoint;
						old.F = old.G + old.H;
					}
				}
			}
			Point resPoint = isInList(openlist, end);
			if (resPoint != null){
				Debug.Log(resPoint.x+", "+resPoint.z);
				return resPoint;
			}
		}
		return null;
	}
	public List<Point> getPath(Point start, Point end){
		Point res = findPath(start, end);
		List<Point> path = new List<Point>();
		int loc = 0;
		while(res!=null){
			path.Add(res);
			res = res.parent;
		}
		path.Reverse();
		openlist.Clear();
		closelist.Clear();
		return path;
	}
}
