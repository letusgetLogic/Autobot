using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    private static GameData _Instance = null;

    public static GameData Instance
    {
        get 
        {
            if (_Instance == null) // Lazy loading
            {
                _Instance = new GameData();
            }
            return _Instance;
        }
    }

    /// <summary>
    /// Blocks creating a new instance from the outside.
    /// </summary>
    /// <param name="game"></param>
    private GameData()
    { }

    private List<Game> savedGames;
    public List<Game> SavedGames => savedGames;

    public void AddGame(Game game)
    {
        if (savedGames == null)
            savedGames = new List<Game>();

        if (game == null || game.State == GameState.EndOfGame)
            return;

        // If it has a saved game with the same mode, override it with the current game.
        var savedGame = savedGames.Find(v => v.Mode == game.Mode);
        if (savedGame != null)
        {
            savedGame = game;
            return;
        }
       
        savedGames.Add(game);
    }
}
