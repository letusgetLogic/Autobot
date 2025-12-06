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

    public List<Game> SavedGames => savedGames;
    private List<Game> savedGames;

    /// <summary>
    /// Add or override the current game to the saved game with the same mode.
    /// </summary>
    /// <param name="_game"></param>
    public void AddGame(Game _game)
    {
        if (savedGames == null)
            savedGames = new List<Game>();

        if (_game == null || _game.State == GameState.EndOfGame)
            return;

        // If it has a saved game with the same mode, override it with the current game.
        var savedGame = savedGames.Find(v => v.Mode == _game.Mode);
        if (savedGame != null)
        {
            savedGame = _game;
            return;
        }
       
        savedGames.Add(_game);
    }
}
