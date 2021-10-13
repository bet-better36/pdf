using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PokemonBase : ScriptableObject
{
    [SerializeField] new string name;
    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;  // 特攻
    [SerializeField] int spDefense; // 特防御
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;

    public int MaxHP { get => maxHP; }
    public int Attack { get => attack; }
    public int Defense { get => defense; }
    public int SpAttack { get => spAttack; }
    public int SpDefense { get => spDefense; }
    public int Speed { get => speed; }
    public List<LearnableMove> LearnableMoves { get => learnableMoves; }
    public string Name { get => name; }
    public string Description { get => description; }
    public Sprite FrontSprite { get => frontSprite; }
    public Sprite BackSprite { get => backSprite; }
    public PokemonType Type1 { get => type1; }
    public PokemonType Type2 { get => type2; }
}

[Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase _base;
    [SerializeField] int level;

    public MoveBase Base { get => _base; }
    public int Level { get => level; }
}
public enum PokemonType
{
    None,
    Normal,
    Fire,       // 火
    Water,      
    Electric,   // 電気
    Grass,      // 草
    Ice,        // 氷
    Fighting,   // 格闘
    Poison,     // 毒
    Ground,     // 地面
    Flying,     // 飛行
    Psychic,    // エスパー
    Bug,        // 虫
    Rock,       // 岩
    Ghost,      // ゴースト
    Dragon,     // ドラゴン


}

public class TypeChart
{
    static float[][] chart =
    {
          //攻撃\防御          NOR  FIR  WAT  ELE  GRS  ICE  FIG  POI
        /* NOR */new float[]{1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /* FIR */new float[]{1f,0.5f,0.5f,  1f,  2f,  2f,  1f,  1f},
        /* WAT */new float[]{1f,  2f,0.5f,  1f,0.5f,  1f,  1f,  1f},
        /* ELE */new float[]{1f,  1f,  2f,0.5f,0.5f,  1f,  1f,  1f},
        /* GRS */new float[]{1f,0.5f,  2f,  1f,0.5f,  1f,  1f,0.5f},
        /* ICE */new float[]{1f,0.5f,0.5f,  1f,  2f,0.5f,  1f,  1f},
        /* FIG */new float[]{2f,  1f,  1f,  1f,  1f,  2f,  1f,0.5f},
        /* POI */new float[]{1f,  1f,  1f,  1f,  2f,  1f,  1f,0.5f},
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defeseType)
    {
        if (attackType == PokemonType.None || defeseType == PokemonType.None)
        {
            return 1f;
        }
        int row = (int)attackType - 1;
        int col = (int)defeseType - 1;
        return chart[row][col];
    }
}