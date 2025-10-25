using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveGame(Game game)
    {
        GameData savedData = LoadGameData();
        if (savedData != null)
            Add(savedData, game);
        else
            savedData = new GameData(game);

        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + $"/game.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, savedData);
        stream.Close();
    }

    public static Game LoadGame(bool delete, GameMode gameMode)
    {
        GameData savedData = LoadGameData();

        if (savedData != null)
        {
            var savedGame = savedData.SavedGames.Find(game => game.Mode == gameMode);

            if (savedGame == null)
                return null;

            if (delete)
            {
                savedData.SavedGames.Remove(savedGame);
                return null;
            }

            return savedGame;
        }
        else
        {
            Debug.LogError("Save data is null");
            return null;
        }
    }

    private static GameData LoadGameData()
    {
        string path = Application.persistentDataPath + "/game.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData gameData = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return gameData;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    private static void Add(GameData gameData, Game game)
    {
        var savedGame = gameData.SavedGames.Find(v => v.Mode == game.Mode);
        if (savedGame != null)
            savedGame = game;
        else
            gameData.SavedGames.Add(game);
    }

}

