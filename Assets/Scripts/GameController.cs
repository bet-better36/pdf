using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    FreeRoam,
    Battle,
}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    GameState state = GameState.FreeRoam;

    private void Start()
    {
        playerController.OnEncouted += StartBatlle;
        battleSystem.BattleOver += EndBattle;
    }

    public void StartBatlle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        Pokemon wildPokemon = FindObjectOfType<MapArea>().GetRandomWildPokemon();
        battleSystem.StartBattle(playerParty, wildPokemon);
    }

    public void EndBattle()
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true );
    }

    void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }
}
