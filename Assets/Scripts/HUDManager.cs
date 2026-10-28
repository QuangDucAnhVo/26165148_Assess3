using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public Button exitButton;
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text ghostTimerText;
    public GameObject[] lifeIcons;
    public GameObject blockImage;
    public TMP_Text messageText;
 
    void Start()
    {
        exitButton.onClick.AddListener(Exit);
    }
 
    void Exit()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void SetScore(int score)
    {
        string text = score.ToString();
        for (int i = text.Length; i < 6; i++)
        {
            text = "0" + text;
        }
        scoreText.text = text;
    }
    public void SetTimer(float seconds)
    {
        timerText.text = FormatTime(seconds);
    }
    public void SetLives(int lives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].SetActive(i < lives);
        }
    }
    public void ShowGhostTimer(float secondsLeft)
    {
        int seconds = (int)secondsLeft;
        if (seconds < secondsLeft)
        {
            seconds++;
        }
        ghostTimerText.text = seconds.ToString();
        ghostTimerText.gameObject.SetActive(true);
    }
    public void HideGhostTimer()
    {
        ghostTimerText.gameObject.SetActive(false);
    }
    public void ShowMessage(string message)
    {
        messageText.text = message;
        blockImage.SetActive(true);
    }
    public void HideMessage()
    {
        blockImage.SetActive(false);
    }
    public void DisableButtons()
    {
        exitButton.interactable = false;
    }
    public static string FormatTime(float seconds)
    {
        int minutes = (int)(seconds / 60f);
        int wholeSeconds = (int)(seconds % 60f);
        int hundredths = (int)(seconds * 100f) % 100;
        return TwoDigits(minutes) + ":" + TwoDigits(wholeSeconds) + ":" + TwoDigits(hundredths);
    }
    static string TwoDigits(int number)
    {
        if (number < 10)
        {
            return "0" + number;
        }
        return number.ToString();
    }
}
