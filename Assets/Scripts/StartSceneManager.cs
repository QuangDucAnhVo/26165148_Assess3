using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartSceneManager : MonoBehaviour
{
   public Button level1Button;
   public TMP_Text level1ScoreText;
   public TMP_Text level1TimeText;

   void Start()
   {
      level1Button.onClick.AddListener(LoadLevel1);
      int highScore = PlayerPrefs.GetInt("Level1HighScore");
      float bestTime = PlayerPrefs.GetFloat("Level1BestTime");
      level1ScoreText.text = "High Score: " + highScore; 
      level1TimeText.text = "Best Time : " + HUDManager.FormatTime(bestTime);
   }
   void LoadLevel1()
   {
      SceneManager.LoadScene("RecreatedLevel");
   }
}
