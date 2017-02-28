using UnityEngine;
using System.Collections;

public static class ScoreManager {

    public static int[] _scores = new int[] {0, 0, 0, 0}; 

    public static void ResetScores()
    {
        for (int i = 0; i < 4; i++)
        {
            _scores[i] = 0;
        }
    }

    public static void AddToScore(int points, int playerId)
    {
        _scores[playerId] += points;
    }
}
