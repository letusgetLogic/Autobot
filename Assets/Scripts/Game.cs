using System.Collections.Generic;

public class Game
{
    public GameMode Mode { get; set; }
    public int PlayerAmount { get; set; }
    public int Timer { get; set; }
    public int Lives { get; set; }
    public int WinCondition { get; set; }
    public GameState State { get; set; }
    public Template Template1 { get; set; }
    public Template Template2 { get; set; }
    public List<SoUnit> AvaiableUnits { get; set; }
    public List<SoItem> AvaiableItems { get; set; }
    public int CurrentPlayerIndex { get; set; }

    public Game(GameMode mode, int playerAmount, int timer, int lives, int winCondition, 
        GameState state, Template template1, Template template2)
    {
        Mode = mode;
        PlayerAmount = playerAmount;
        Timer = timer;
        Lives = lives;
        WinCondition = winCondition;
        State = state;
        Template1 = template1;
        Template2 = template2;
        AvaiableUnits = new();
        AvaiableItems = new();
        CurrentPlayerIndex = 0;
    }
}
