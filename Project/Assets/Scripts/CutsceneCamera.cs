using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    public GameObject mainCam; // Variable for the main camera

    public void SwitchCameras()
    {
        mainCam.SetActive(true);
        gameObject.SetActive(false);
        
        PlayerPrefs.SetInt("Cutscene", 0);
        
        ResetThePopups();

        LockTheCursor();
    }

    private static void LockTheCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private static void ResetThePopups()
    {
        for (int i = 0; i < GameManager.instance.popUpList.Count - 2; i++)
        {
            GameManager.instance.popUpList[i].SetActive(false);
        }
    }
}