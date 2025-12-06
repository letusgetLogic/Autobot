using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    /// <summary>
    /// Saves the game.
    /// </summary>
    /// <param name="_game"></param>
    public static void SaveGame(Game _game)
    {
        GameData savedData = LoadGameData() == null ? LoadGameData() : GameData.Instance;
        savedData.AddGame(_game);

        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + $"/game.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, savedData);
        stream.Close();
    }

    /// <summary>
    /// Load the game data.
    /// </summary>
    /// <returns></returns>
    private static GameData LoadGameData()
    {
        string path = Application.persistentDataPath + "/game.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            stream.Position = 0;
            GameData gameData = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return gameData;
        }
        else
        {
            Debug.Log("Save file not found in " + path);
            return null;
        }
    }

    /// <summary>
    /// Load the saved game with given settings.
    /// </summary>
    /// <param name="_isNotSaving"></param>
    /// <param name="_gameMode"></param>
    /// <returns></returns>
    public static Game LoadGame(bool _isNotSaving, GameMode _gameMode)
    {
        GameData savedData = LoadGameData();

        if (savedData != null)
        {
            if (savedData.SavedGames != null)
            {
                var savedGame = savedData.SavedGames.Find(game => game.Mode == _gameMode);

                if (savedGame == null)
                    return null;

                if (_isNotSaving)
                {
                    savedData.SavedGames.Remove(savedGame);
                    return null;
                }

                return savedGame;
            }
            else
            {
                Debug.Log("Saved games is null");
                return null;
            }
        }
        else
        {
            Debug.Log("Saved data is null");
            return null;
        }
    }

}

