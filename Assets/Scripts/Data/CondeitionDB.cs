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
}
