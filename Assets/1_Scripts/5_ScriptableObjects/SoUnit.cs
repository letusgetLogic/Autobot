using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObject/Unit")]
public class SoUnit : ScriptableObject
{
    public UnitType UnitType;

    public Sprite Sprite;

    public int ID;
    public string Name;
    public string ModelID;

    public int Health;
    public int Attack;
    public SoIntVariable Energy;

    public bool HasUniqueCost;
    public SoIntVariable UniqueCostNuts;
    public SoIntVariable UniqueCostTools;

    public SoIntVariable LevelLimit;
    public Level[] Levels ;

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}

/*
Balancing 20.06.26 

- Scaling recycle value
- Scaling the ability of robots:

Tier 1
- Steel Boy - Amount of mates increased (Lv.2: 1 → 2 | Lv.3: 1 → 3)
- Copper Man - Amount of Copper Prototype increased (Lv.2: 1 → 2 | Lv.3: 1 → 3)
- Gold Eye - Buff ATK increased (Lv.1: +1 → +2 | Lv.2: +2 → +4 | Lv.3: +3 → +8)
- Blue Squid - Amount of mates increased (Lv.2: 1 → 2 | Lv.3: 1 → 3)
- Drone - Damage increased (Lv.2: 1 → 2 | Lv.3: 1 → 3)

Tier 2
- Black Crow - Stolen Energy increased (Lv.1: 1 → 2 | Lv.2: 2 → 4 | Lv.3: 3 → 8)
- Red Snowman - Buff ATK & Buff HP increased (Lv.1: +1/1 → +2/2 | Lv.2: +2/2 → +4/4 | Lv.3: +3/3 → +8/8)
- Bubble Clean - Buff Bubble (Lv.3: 6/6 → 8/8 Bubble)

Tier 3
- Boxer - Buff ATK & Buff HP increased (Lv.2: +8/4 → +8/6 | Lv.3: +12/6 → +14/12)
- Mammoth - Buff ATK & Buff HP increased (Lv.3: +6/6 → 8/8)
*/