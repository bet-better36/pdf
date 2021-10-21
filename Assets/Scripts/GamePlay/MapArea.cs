using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;

    public Pokemon GetRandomWildPokemon()
    {
        int r = Random.Range(0, pokemons.Count);
        Pokemon pokemon = pokemons[r];
        pokemon.Init();
        return pokemons[r];
    }

    public void faf()
    {

    }

}
