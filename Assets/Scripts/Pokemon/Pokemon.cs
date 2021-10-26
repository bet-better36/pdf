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
    public Dictionary<Stat, int> Stats { get; set; }
    public Dictionary<Stat, int> StatBoosts { get; set; }

    public Queue<string> StatusChanges { get; private set; }

    public Condition Status { get; private set; }
    public int StatusTime { get; set;  }

    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }

    public bool HpChange { get; set; }

    Dictionary<Stat, string> statDic = new Dictionary<Stat, string>()
    {
        {Stat.Attack, "こうげき"},
        {Stat.Defense, "ぼうぎょ"},
        {Stat.SpAttack, "とくこう"},
        {Stat.SpDefense, "とくぼう"},
        {Stat.Speed, "すばやさ"},
        {Stat.Accuracy, "めいちゅうりつ"},
        {Stat.Evasion, "かいひりつ"},
    };

    public System.Action OnStatusChanged;

    public void Init()
    {
        StatusChanges = new Queue<string>();
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

        CalculateStats();
        HP = MaxHP;

        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0 },
            {Stat.Defense, 0 },
            {Stat.SpAttack, 0 },
            {Stat.SpDefense, 0 },
            {Stat.Speed, 0 },
            {Stat.Accuracy, 0 },
            {Stat.Evasion, 0 },
        };
    }

    public void OnBattleOver()
    {
        ResetStatBoost();
        VolatileStatus = null;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);
        MaxHP = Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10 + Level;
    }

    int GetStat(Stat stat)
    {
        int statValue = Stats[stat];
        int boost = StatBoosts[stat];
        float[] boostValue = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f, };

        if (boost >= 0)
        {
            statValue = Mathf.FloorToInt(statValue * boostValue[boost]);
        }
        else
        {
            statValue = Mathf.FloorToInt(statValue / boostValue[-boost]);

        }
        return statValue;
    }

    public void ApplyBooosts(List<StatBoost> statBoosts)
    {
        foreach (StatBoost statBoost in statBoosts)
        {
            Stat stat = statBoost.stat;
            int boost = statBoost.boost;
            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
            if (boost > 0)
            {
                StatusChanges.Enqueue($"{Base.Name}の{statDic[stat]}があがった");
            }
            else
            {
                StatusChanges.Enqueue($"{Base.Name}の{statDic[stat]}がさがった");
                Debug.Log($"status {statBoost.stat} -{statBoost.boost}");
            }
        }
    }

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }
    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }
    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }
    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }
    public int MaxHP
    {
        get; private set; 
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
        if (move.Base.Category == MoveCategory.Special)
        {
            attack = attacker.SpAttack;
            defense = SpDefense;
        }

        float modifires = /*Random.Range(0.85f, 1f) */ type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifires);

        UpdateHP(damage);
        Debug.Log($"damage {damage}");
        return damageDetails;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        HpChange = true;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null)
        {
            return;
        }
        Status = CondeitionDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name}{Status.StartMessege}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null)
        {
            return;
        }
        VolatileStatus = CondeitionDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name}{VolatileStatus.StartMessege}");
        //OnStatusChanged?.Invoke(); // <=UIの変更
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
        //OnStatusChanged?.Invoke();
    }

    public bool OnBeforeMove()
    {
        bool canRunMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (Status.OnBeforeMove(this) == false)
            {
                canRunMove = false;
            }
        }
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (VolatileStatus.OnBeforeMove(this) == false)
            {
                canRunMove = false;
            }
        }
        return canRunMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }
}
public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}