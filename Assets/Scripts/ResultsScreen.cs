using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResultsScreen : MonoBehaviour {

    public Text _player1Text;
    public Text _player2Text;
    public Text _player3Text;
    public Text _player4Text;
    public Text _winnerText;

    private int[] _playerScores = new int[4];
    private int _numberOfPlayers;

    void Start () {
        System.Array.Copy(ScoreManager._scores, _playerScores, 4);
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
        //Draw Scores
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


        //Calculate winner or draw
        int highScore = _playerScores[0];
        int winnerId = 0;
        List<int> drawIds = new List<int>();
        bool isDraw = false;
        drawIds.Add(0);

        for (int i = 1; i < _numberOfPlayers; i++)
        {
            if (_playerScores[i] > highScore)
            {
                highScore = _playerScores[i];
                winnerId = i;
                isDraw = false;
                drawIds.Clear();
                drawIds.Add(i);
            }
            else if(_playerScores[i] == highScore)
            {
                isDraw = true;
                drawIds.Add(i);
            }
        }


        //Draw final results
        if (!isDraw)
        {
            _winnerText.text = "Winner: Player " + (winnerId + 1).ToString();
        }
        else
        {
            _winnerText.text = "Draw between players";
            foreach (int id in drawIds)
            {
                _winnerText.text += " " + (id + 1).ToString();
            }
        }
    }
}
