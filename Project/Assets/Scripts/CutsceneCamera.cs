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
        PlayerPrefs.SetInt("Cutscene", 0);
        for (int i = 0; i < GameManager.instance.popUpList.Count - 2; i++)
        {
            GameManager.instance.popUpList[i].SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked; // Unlock the cursor, allowing free movement outside the game window.
        Cursor.visible = false;
    }
}