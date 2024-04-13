using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Score
{

    private static int score;
    private static Text scoreTxt;
    private static Text ttlScoreTxt;
    private static int applePoints = 100;
    private static int corePoints = 1;
    private static int blockPoints = 1;
    private static int multiplier = 160;

    public static void Setup()
    {
        score = 0;
        scoreTxt = GameAssets.Instance.score;
        ttlScoreTxt = GameAssets.Instance.totalscore;
        ttlScoreTxt.text = PlayerPrefs.GetInt("TotalScore", 0).ToString();
    }
    
    public static void ResetScore()
    {
        score = 0;
        scoreTxt.text = score.ToString();
    }

    public static void AppleScore()
    {
        score += applePoints;
        scoreTxt.text = score.ToString();
    }

    public static void CoreScore(int husks)
    {
        score += (corePoints + husks);
        scoreTxt.text = score.ToString();
    }

    public static void BlockScore()
    {
        score += blockPoints;
        scoreTxt.text = score.ToString();
    }

    public static int GetScore()
    {
        return score;
    }

    public static bool TrySetHighScore()
    {
        if (score > GetHighScore())
        {
            PlayerPrefs.SetInt("HScore", score);
            PlayerPrefs.Save();
            return true;
        }
        else
        {
            return false;
        }
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt("HScore", 0);
    }

    public static void UpdateTotal()
    {
        PlayerPrefs.SetInt("TotalScore", PlayerPrefs.GetInt("TotalScore", 0) + score);
        PlayerPrefs.Save();
        ttlScoreTxt.text = PlayerPrefs.GetInt("TotalScore", 0).ToString();
    }

    public static void CheckForChat()
    {
        int chatIndex = PlayerPrefs.GetInt("ChatIndex", 1);
        if (PlayerPrefs.GetInt("TotalScore", 0) >= chatIndex * multiplier)
        {
            ScenarioKeeper.GetKeeper().PlayChat();
        }
    }
}
