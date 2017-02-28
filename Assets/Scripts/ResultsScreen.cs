using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResultsScreen : MonoBehaviour {

    public Text _player1Text;
    public Text _player2Text;
    public Text _player3Text;
    public Text _player4Text;
    public Text _winnerText;

    private int[] _playerScores = new int[4];
    private int _numberOfPlayers;

    void Start () {
        _playerScores[0] = ScoreManager._player1Score;
        _playerScores[1] = ScoreManager._player2Score;
        _playerScores[2] = ScoreManager._player3Score;
        _playerScores[3] = ScoreManager._player4Score;
        _numberOfPlayers = GameManager._numberOfStartingPlayers;
        Debug.Log("Results Screen: Number of players: " + _numberOfPlayers);

        //Reset static parameters
        GameManager._numberOfStartingPlayers = 0;
        GameManager._currentNumberOfPlayers = 0;
        ScoreManager.ResetScores();
        Polygon.ID_COUNT = 0;

        DrawScoresAndWinner();
    }


    void DrawScoresAndWinner()
    {
        _player1Text.text = "Player 1: " + _playerScores[0].ToString();
        _player2Text.text = "Player 2: " + _playerScores[1].ToString();
        if (_numberOfPlayers > 2)
        {
            _player3Text.text = "Player 3: " + _playerScores[2].ToString();
        }
        else
        {
            _player3Text.enabled = false;
        }
        if (_numberOfPlayers > 3)
        {
            _player4Text.text = "Player 4: " + _playerScores[3].ToString();
        }
        else
        {
            _player4Text.enabled = false;
        }

        int highScore = _playerScores[0];
        for (int i = 1; i < _numberOfPlayers; i++)
        {
            if (_playerScores[i] > highScore)
            {
                highScore = _playerScores[i];
            }
        }

        _winnerText.text = highScore.ToString();//TODO Descubrir quien es el ganador
    }
}
