using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    public GameObject mainCam;
    
    public void SwitchCameras()
    {
        Debug.Log("Switch Cameras");
        mainCam.SetActive(true);
        gameObject.SetActive(false);
        PlayerPrefs.SetInt("Cutscene",0);
    }
}
