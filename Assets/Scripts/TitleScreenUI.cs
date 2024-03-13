using UnityEngine;
using UnityEngine.SceneManagement;
 

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] IInputManager _inputMgr;

    void OnEnable(){
        _inputMgr.OnFlapPressed += OnFlapPressed;
    }

    void OnDisable(){
        _inputMgr.OnFlapPressed -= OnFlapPressed;
    }

    void OnFlapPressed(){
        SceneManager.LoadScene("Game",LoadSceneMode.Single);
    }
}
