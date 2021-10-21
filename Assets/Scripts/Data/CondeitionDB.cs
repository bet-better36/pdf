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
                    Debug.Log("poisoned!");
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
                //OnAfterTurn = Poison,
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    Debug.Log("poisoned!");
                    pokemon.UpdateHP(pokemon.MaxHP/16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はやけどのダメージをうけた");
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
