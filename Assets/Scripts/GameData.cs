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

        var savedGame = SavedGames.Find(v => v.Mode == game.Mode);

        if (savedGame == null)
        {
            if (game.State != GameState.EndOfGame)
                SavedGames.Add(game);
        }
        else
        {
            if (game.State == GameState.EndOfGame)
            {
                SavedGames.Remove(savedGame);
                return;
            }

            savedGame = game;
        }
    }
}
