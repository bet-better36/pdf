using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon
{

    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public PokemonBase Base { get => _base; }
    public int Level { get => level; }

    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    public void Init()
    {
        HP = MaxHP;

        Moves = new List<Move>();

        foreach (LearnableMove learnableMove in Base.LearnableMoves)
        {
            if (Level >= learnableMove.Level)
            {
                Moves.Add(new Move(learnableMove.Base));
            }

            if (Moves.Count >= 4)
            {
                break;
            }
        }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; }
    }
    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; }
    }
    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5; }
    }
    public int SpDefense
    {
        get { return Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5; }
    }
    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
    }
    public int MaxHP
    {
        get { return Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10; }
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        float critical = 1f;
        if (Random.value * 100 <= 6.25f)
        {
            critical = 2f;
        }
        float type = TypeChart.GetEffectiveness(move.Base.Type, Base.Type1)
                   * TypeChart.GetEffectiveness(move.Base.Type, Base.Type2);

        DamageDetails damageDetails = new DamageDetails
        {
            Fainted = false,
            Critical = critical,
            TypeEffectiveness = type
        };

        //特殊技の場合の修正
        float attack = attacker.Attack;
        float defense = Defense;
        if (move.Base.IsSpecial)
        {
            attack = attacker.SpAttack;
            defense = SpDefense;
        }

        float modifires = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifires);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}
public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}