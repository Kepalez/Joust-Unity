using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TMP_Text _score;
    [SerializeField] InputManagerBase _inputManager;

    IInputManager InputManager => _inputManager.InputManager;

    void Start(){
        _score.text = "Score\n"+ScoreManager.Score.ToString();
        InputManager.OnFlapPressed += OnFlapPressed;
    }

    private void OnFlapPressed()
    {
        InputManager.OnFlapPressed += OnFlapPressed;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
