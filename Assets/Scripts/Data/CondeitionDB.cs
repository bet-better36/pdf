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
                //OnAfterTurn = Poison,
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    Debug.Log("poisoned!");
                    pokemon.UpdateHP(pokemon.MaxHP/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はどくのダメージをうけた");
                }
                
            }
        }
    };

    /*
    static void Poison(Pokemon pokemon)
    {

    }
    */
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
