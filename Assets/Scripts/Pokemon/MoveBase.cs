using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MoveBase : ScriptableObject
{
    [SerializeField] new string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accracy; //命中率
    [SerializeField] int pp;

    [SerializeField] MoveCategory category;
    [SerializeField] MoveTarget target;
    [SerializeField] MoveEffects effects;

    public string Name { get => name; }
    public string Description { get => description; }
    public PokemonType Type { get => type; }
    public int Power { get => power; }
    public int Accracy { get => accracy; }
    public int PP { get => pp; }
    public MoveCategory Category { get => category; set => category = value; }
    public MoveTarget Target { get => target; }
    public MoveEffects Effects { get => effects; }
}

public enum MoveCategory
{
    Physical,
    Special,
    Stat,
}

public enum MoveTarget
{
    Foe,
    Self,
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    public List<StatBoost> Boosts { get => boosts; }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}