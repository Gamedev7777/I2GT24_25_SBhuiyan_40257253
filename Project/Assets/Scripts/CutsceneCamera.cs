using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    public GameObject mainCam; // Variable for the main camera
    public AudioClip remyAudioClip, claireAudioClip;
    
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        PlayAudio();
    }
    
    public void SwitchCameras()
    {
        Debug.Log("Switch Cameras");
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

    private void PlayAudio()
    {
        if (PlayerPrefs.GetInt("Avatar", 0) == 0)
        {
            // Remy is chosen
            _audioSource.clip = remyAudioClip;
            _audioSource.Play();
        }
        else if (PlayerPrefs.GetInt("Avatar", 0) == 1)
        {
            // Claire is chosen
            _audioSource.clip = claireAudioClip;
            _audioSource.Play();
        }
    }
    
}