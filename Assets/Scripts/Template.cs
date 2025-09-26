using UnityEngine;

public class Template : MonoBehaviour
{
    private int health;
    private int wins;

    public Template(int _health, int _wins)
    {
        health = _health;
        wins = _wins;
    }   

    private int turns;
    private int currentWins;
    GameObject[] BattleBots;
}
