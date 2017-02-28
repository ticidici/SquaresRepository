using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

    public void LoadScene(string sceneName)
    {
        PoolManager.Clean();
        SceneManager.LoadScene(sceneName);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

}
