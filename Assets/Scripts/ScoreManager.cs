using UnityEngine;
using System.Collections;

public static class ScoreManager {

    public static int _player1Score = 0;
    public static int _player2Score = 0;
    public static int _player3Score = 0;
    public static int _player4Score = 0;

    public static void ResetScores()
    {
        _player1Score = 0;
        _player2Score = 0;
        _player3Score = 0;
        _player4Score = 0;
    }

    public static void AddToScore(int points, int playerId)
    {
        switch (playerId)
        {
            case 0:
                _player1Score += points;
                break;
            case 1:
                _player2Score += points;
                break;
            case 2:
                _player3Score += points;
                break;
            case 3:
                _player4Score += points;
                break;
        }
    }
}
