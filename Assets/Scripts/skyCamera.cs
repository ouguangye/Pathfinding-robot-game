using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skyCamera : MonoBehaviour
{
    
    public camera mainCamera;

    // mapsize 20 -> Field of View 63, mapsize 10 -> Field of View 35
    int viewPortAccordingToMapSize(int size){
        return (int)(7.0/85.0 * size* size + (63-400*7.0/85.0)) + 5;
    }

    void Start()
    {
        int mapsize = mainCamera.mapsize;
        GetComponent<Camera>().fieldOfView = viewPortAccordingToMapSize(mapsize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
