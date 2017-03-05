using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class GameManager : MonoBehaviour {

    public static int _numberOfStartingPlayers = 0;
    public static int _currentNumberOfPlayers = 0;
    public static GameManager _instance;

    public int _winnerReward = 500;

    private bool _isGameFinished = false;
    private List<int> _currentPlayersIds = new List<int>();

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

    public void AddPlayerToGame(int id)//TODO Usar esta id para, más tarde darle puntos extra a quien quede último
    {
        _numberOfStartingPlayers += 1;
        _currentPlayersIds.Add(id);
        _currentNumberOfPlayers = _numberOfStartingPlayers;
        Debug.Log("Number of polygons: " + _numberOfStartingPlayers);
    }

    public void PolygonKilled(int id)
    {
        _currentNumberOfPlayers -= 1;

        _currentPlayersIds.Remove(id);

        Debug.Log("Number of polygons: " + _currentNumberOfPlayers + " out of " + _numberOfStartingPlayers);
        if (_currentNumberOfPlayers < 2 && !_isGameFinished)
        {
            _isGameFinished = true;
            RewardWinner();
            _currentPlayersIds.Clear();
            Invoke("ShowResultsScreen", 0.8f);
        }
    }

    private void ShowResultsScreen()
    {
        SceneManager.LoadScene("Results Screen");
        _isGameFinished = false;
        CancelInvoke();
    }

    private void RewardWinner()
    {
        ScoreManager.AddToScore(_winnerReward, _currentPlayersIds[0]);//Pressuposa que només queda un polygon
    }
  
}
