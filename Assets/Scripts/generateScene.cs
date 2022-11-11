using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class generateScene : MonoBehaviour
{
    public Dropdown dropdown;
    
    public InputField textField;
    
    public GameObject ui_camera;

    public GameObject scene;

    public int mode = 0;
    public int mapSize = 10;

    public void myClick() {
        globalVariable.mode = (int)dropdown.value;
        globalVariable.mapsize = int.Parse(textField.text);

        gameObject.SetActive(false);
        scene.SetActive(true);
        ui_camera.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
