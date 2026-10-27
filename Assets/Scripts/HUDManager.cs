using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    public Button exitButton;
    
    void Start()
    {
        exitButton.onClick.AddListener(ExitToStartScene);
    }

    void ExitToStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }
}
