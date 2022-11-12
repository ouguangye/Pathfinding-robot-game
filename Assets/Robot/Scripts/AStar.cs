using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point{
	public float x, z;
	public float F, G, H;
	public Point parent;
	public Point(float x, float z){
		this.x = x;
		this.z = z;
		this.F = 0; 
		this.G = 0;
		this.H = 0;
	}

	public float CalG(Point previous = null){
		if(previous == null)
			return 0f;
		float tempG = Vector2.Distance(new Vector2(previous.x, previous.z)*0.5f, 
		new Vector2(this.x, this.z));
		float g = tempG + previous.G;
		this.G = g;
		return g;
	}

	public float CalH(Point end){
		H = Mathf.Sqrt(Mathf.Pow(end.x - this.x, 2) + Mathf.Pow(end.z - this.z, 2));
		return H;
	}

	public float CalF(Point end, Point previous = null){
		F = CalG(previous) + CalH(end);
		return F;
	}

	// public static bool operator ==(Point a, Point b){
	// 	if(a == null || b == null)
	// 		return false;
	// 	return a.x == b.x && a.z == b.z;
	// }

	// public static bool operator !=(Point a, Point b){
	// 	if(a == null || b == null)
	// 		return true;
	// 	return a.x != b.x || b.z != a.z;
	// }
}

public class AStar{
	RobotMap map;
	private List<Point> openlist = new List<Point>();
	private List<Point> closelist = new List<Point>();
	private int[,] dirs = {{1,0},{-1,0},{0,1},{0,-1}};
	public string logg;
	
	public AStar(RobotMap r_map){
		map = r_map;
	}

	private bool isInList_vec(List<Point> list, Point vec){
		foreach(Point v in list){
			if(v.x == vec.x && v.z == vec.z)
				return true;
		}
		return false;
	}

	private Point isInlist(List<Point> list, Point cur){
		foreach(Point p in list){
			if(p.x == cur.x && p.z == cur.z){
				return p;
			}
		}
		return null;
	}

	private bool isCanReach(int i, int j){
		return map.isCanReach(i, j) && !isInList_vec(closelist, new Point(map.mapToPoint(i,j).x, map.mapToPoint(i,j).z));
	}

	private List<Vector2> getSurround(Vector2 cur){
		List<Vector2> res = new List<Vector2>();
		for(int i = 0;i<4;++i){
			int x = (int)cur.x + dirs[i,0];
			int y = (int)cur.y + dirs[i,1];
			// Debug.Log("for point ("+ point.x +","+point.z+"), next is ("+ temp.x + "," + temp.z + ")");
			if(isCanReach(x, y)){
				res.Add(new Vector2(x,y));
			}
		}
		return res;
	}

    Point getLeastFPoint()
    {
		if (openlist.Count!=0)
        {
            Point resPoint = openlist[0];
            foreach(Point point in openlist)
                if (point.F < resPoint.F)
                    resPoint = point;
            return resPoint;
        }
        return null;
    }


	public Point findPath(Point start, Point end){
		Debug.Log("end: "+end.x+", "+end.z);
		start.CalF(end);
		openlist.Add(start); 
		int times = 0;
		while(openlist.Count>0 && times++ < 20000){
			Point cur_point = getLeastFPoint();
			Vector2 cur_vec = map.pointFillMap(cur_point.x, cur_point.z);
			Debug.Log("current map loc:" + cur_vec);
			openlist.Remove(cur_point);
			closelist.Add(cur_point);
			List<Vector2> surround = getSurround(cur_vec);
			foreach (var s_vec in surround){
				Point s = new Point(map.mapToPoint((int)s_vec.x, (int)s_vec.y).x, map.mapToPoint((int)s_vec.x, (int)s_vec.y).z);				
				if(isInlist(openlist, s) == null){
					s.parent = cur_point;
					s.CalF(end, cur_point);
					Debug.Log("for point ("+cur_point.x+", "+ cur_point.z+ "), next (" + s.x + ", " + s.z + ")'s F = " + s.F);
					openlist.Add(s);
				} else {
					Point old = isInlist(openlist, s);
					if(old.F>s.CalF(end, cur_point)){
						old.parent = cur_point;
						old.F = s.F;
					}
				}
			}
			Point res = isInlist(openlist, end);
			if (res != null){
				return res;
			}
		}
		Debug.Log("Empty path");
		return null;
	}

	public List<Point> getPath(Point start, Point end){
		Point res = findPath(start, end);
		List<Point> path = new List<Point>();
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
