public static class NameList
{
    private static readonly string[] Names = new string[]
   {
        "The B-Team",
        "The Rockets",
        "Megadron",
        "The Tanks",
        "The Terminators",
   };

    public static string GetRandomName()
    {
        int index = UnityEngine.Random.Range(0, Names.Length);
        return Names[index];
    }

    public static string GetRandomExclusive(string[] existingNames)
    {
        string name;
        do
        {
            name = GetRandomName();
        } while (System.Array.Exists(existingNames, n => n == name));
        return name;
    }
}