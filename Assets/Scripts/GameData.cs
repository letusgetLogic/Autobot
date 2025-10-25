using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<Game> SavedGames { get; set; }

    // Settings

    public GameData(Game game)
    {
        if (SavedGames == null)
            SavedGames = new List<Game>();

        if (game.State != GameState.EndOfGame)
            SavedGames.Add(game);
    }
}
