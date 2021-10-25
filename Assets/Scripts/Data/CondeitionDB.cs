using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CondeitionDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.Poison,
            new Condition()
            {
                Id = ConditionID.Poison,
                Name = "どく",
                StartMessege = "はどくになった",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHP/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はどくのダメージをうけた");
                }
            }
        },
        {
            ConditionID.Burn,
            new Condition()
            {
                Id = ConditionID.Burn,
                Name = "やけど",
                StartMessege = "はやけどになった",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHP/16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はやけどのダメージをうけた");
                }
            }
        },
        {
            ConditionID.Paralysis,
            new Condition()
            {
                Id = ConditionID.Paralysis,
                Name = "まひ",
                StartMessege = "はまひになった",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (UnityEngine.Random.Range(1,5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はしびれてうごけない");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.Freeze,
            new Condition()
            {
                Id = ConditionID.Freeze,
                Name = "こおり",
                StartMessege = "はこおった",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (UnityEngine.Random.Range(1,5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}のこおりはとけた");
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はこおってしまっていてうごけない");
                    return false;
                }
            }
        },
        {
            ConditionID.Sleep,
            new Condition()
            {
                Id = ConditionID.Sleep,
                Name = "ねむり",
                StartMessege = "はねむった",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.StatusTime = UnityEngine.Random.Range(1,4);
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はめをさました");
                    }
                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はねむっている");
                    return false;
                }
            }
        },
        {
            ConditionID.Confusion,
            new Condition()
            {
                Id = ConditionID.Confusion,
                Name = "こんらん",
                StartMessege = "はこんらんした",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.VolatileStatusTime = UnityEngine.Random.Range(1,5);
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はこんらんがとけた");
                        return true;
                    }
                    pokemon.VolatileStatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はこんらんしている");
                    Debug.Log(pokemon.HP);
                    if (UnityEngine.Random.Range(1,3) == 1)
                    {
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はわけもわからず\nじぶんをこうげきした");
                    pokemon.UpdateHP(pokemon.MaxHP/8);
                    Debug.Log(pokemon.HP);
                    return false;
                }
            }
        },
    };
}

public enum ConditionID
{
    None,
    Poison,
    Burn,
    Sleep,
    Paralysis,
    Freeze,
    Confusion,
}
