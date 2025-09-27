using UnityEngine;

public class Template : MonoBehaviour
{
    private int health;
    private int wins;
    private int turns;

    public Template(int _health, int _wins)
    {
        health = _health;
        wins = _wins;
        turns = 0; 
    }   

    private int currentWins;
    GameObject[] BattleBots;
}
