using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;

    public List<Pokemon> Pokemons { get => pokemons; }

    private void Start()
    {
        foreach (Pokemon pokemon in Pokemons)
        {
            pokemon.Init();
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        return Pokemons.Where(monster => monster.HP > 0).FirstOrDefault();
    }
}
