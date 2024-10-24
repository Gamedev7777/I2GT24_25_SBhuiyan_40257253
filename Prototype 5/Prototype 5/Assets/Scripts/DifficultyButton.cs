using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    private Button _button;
    private GameManager _gameManager;

    public int difficulty;
    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _button.onClick.AddListener(SetDifficulty);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetDifficulty()
    {
        Debug.Log(_button.gameObject.name +" was clicked");
        _gameManager.StartGame(difficulty);
    }
}
