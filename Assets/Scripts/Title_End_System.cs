using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_End_System : MonoBehaviour
{
    public void StartButton()
    {
        //게임 씬 이동.
        SceneManager.LoadScene(1); 
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
