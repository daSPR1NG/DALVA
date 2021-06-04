﻿using UnityEngine;

public class GameParameters : MonoBehaviour
{
    public bool classIsMage = false;

    public int maxLevelDone;

    #region Singleton
    public static GameParameters Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
    }
    #endregion

    public void SetClassChosenToMage()
    {
        classIsMage = true;
    }

    public void SetClassChosenToWarrior()
    {
        classIsMage = false;
    }
}
