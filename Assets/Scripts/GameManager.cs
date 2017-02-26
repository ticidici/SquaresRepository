using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {

    public static int _numberOfStartingPlayers = 0;
    public static int _currentNumberOfPlayers = 0;

    public static GameManager _instance;
    private bool _isGameFinished = false;

    private void Awake()//singleton time
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void AddPlayerToGame()
    {
        _numberOfStartingPlayers += 1;
        _currentNumberOfPlayers = _numberOfStartingPlayers;
        Debug.Log("Number of polygons: " + _numberOfStartingPlayers);
    }

    public void PolygonKilled()
    {
        _currentNumberOfPlayers -= 1;
        Debug.Log("Number of polygons: " + _currentNumberOfPlayers + " out of " + _numberOfStartingPlayers);
        if (_currentNumberOfPlayers < 2 && !_isGameFinished)
        {
            _isGameFinished = true;
            Invoke("ShowResultsScreen", 0.8f);
        }
    }

    private void ShowResultsScreen()
    {
        SceneManager.LoadScene("Results Screen");
        _isGameFinished = false;
        CancelInvoke();
    }
  
}
