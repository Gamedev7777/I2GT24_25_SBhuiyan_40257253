using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    public GameObject mainCam; // Reference to the main camera in the scene
    public AudioClip remyAudioClip, claireAudioClip; // Audio clips for Remy and Claire

    private AudioSource _audioSource; // AudioSource component for playing audio clips

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>(); // Assigns the AudioSource component
        PlayAudio(); // Starts playing the appropriate audio based on player preferences
    }

    public void SwitchCameras()
    {
        ChangeMusicVolume(0.3f); // Lowers the music volume for the transition

        mainCam.SetActive(true); // Activates the main camera

        gameObject.SetActive(false); // Deactivates the current cutscene camera

        PlayerPrefs.SetInt("Cutscene", 0); // Marks the cutscene as complete

        ResetThePopups(); // Resets any active pop-ups

        LockTheCursor(); // Locks the cursor for gameplay
    }

    private static void LockTheCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor
        Cursor.visible = false; // Hides the cursor from view
    }

    private static void ResetThePopups()
    {
        // Deactivates all popups in the GameManager's popUpList except the last two
        for (int i = 0; i < GameManager.instance.popUpList.Count - 2; i++)
        {
            GameManager.instance.popUpList[i].SetActive(false);
        }
    }

    private void PlayAudio()
    {
        ChangeMusicVolume(0.1f); // Reduces the music volume for the cutscene
        if (PlayerPrefs.GetInt("Avatar", 0) == 0)
        {
            // If Remy is chosen, play Remy's audio clip
            _audioSource.clip = remyAudioClip;
            _audioSource.Play();
        }
        else if (PlayerPrefs.GetInt("Avatar", 0) == 1)
        {
            // If Claire is chosen, play Claire's audio clip
            _audioSource.clip = claireAudioClip;
            _audioSource.Play();
        }
    }

    private void ChangeMusicVolume(float volume)
    {
        // Adjusts the music volume in the GameManager's musicAudioSource
        GameManager.instance.musicAudioSource.volume = volume;
    }
}
