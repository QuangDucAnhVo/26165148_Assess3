using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
   public Button level1Button;

   void Start()
   {
      level1Button.onClick.AddListener(LoadLevel1);
   }

   void LoadLevel1()
   {
      SceneManager.LoadScene("RecreatedLevel");
   }
}
