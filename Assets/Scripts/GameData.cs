using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<Game> SavedGames { get; set; }
    
    // Settings

    public GameData()
    {
        SavedGames = new List<Game>();
    }
}
