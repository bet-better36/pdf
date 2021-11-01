using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BattleState
{
    Start,
    ActionSelection,    //行動選択
    MoveSelection,      //技選択
    RunnigTurn,        //技の実行
    Busy,               //処理中
    PartyScreen,        //ポケモン選択
    BattleOver,         //バトル終了
}

public enum BattleAction
{
    Move,
    SwitchPokemon,
    Item,
    Run,
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;

    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    //[SerializeField] GameController gameController;
    public UnityAction OnBattleOver;

    BattleState state;
    BattleState? preState; //?はnullを含む
    int currentAction;  //0:fight, 1:Run
    int currentMove;    //0:左上, 1:右上, 2;左下, 3:右下　の技
    int currentMember;

    PokemonParty playerParty;
    Pokemon wildPokemon;
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        state = BattleState.Start;

        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);
        partyScreen.Init();


        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"やせいの {enemyUnit.Pokemon.Base.Name} があらわれた！");
        ActionSelection();
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.EnableActionSelector(true);
        StartCoroutine(dialogBox.TypeDialog("どうする？"));
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableMoveSelector(true);
    }

    void OpenPartyAction()
    {
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
        partyScreen.SetPatyData(playerParty.Pokemons);
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            Pokemon nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon == null)
            {
                Debug.Log("end");
                StartCoroutine(dialogBox.TypeDialog("サトシの　てもとには たたかえる\nポケモンが　いない・・・"));
                StartCoroutine(dialogBox.TypeDialog("サトシは\nめのまえが　まっくらに　なった・・・"));
                BattleOver();
            }
            else
            {
                OpenPartyAction();
            }
        }
        else
        {
            BattleOver();
        }
    }

    void BattleOver()
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver();
    }

    IEnumerator RunTurns(BattleAction battleAction)
    {
        state = BattleState.RunnigTurn;
        if (battleAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            bool playerGosefirst = true;
            BattleUnit firstUnit = playerUnit;
            BattleUnit secondUnit = enemyUnit;
            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            if (playerMovePriority < enemyMovePriority)
            {
                playerGosefirst = false;
            }
            else if(playerMovePriority == enemyMovePriority)
            {
                if (playerUnit.Pokemon.Speed < enemyUnit.Pokemon.Speed)
                {
                    playerGosefirst = false;
                }
            }

            if (playerGosefirst == false)
            {
                firstUnit = enemyUnit;
                secondUnit = playerUnit;
            }

            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver)
            {
                yield break;
            }

            if (secondUnit.Pokemon.HP > 0)
            {
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver)
                {
                    yield break;
                }
            }
        }
        else
        {
            if (battleAction == BattleAction.SwitchPokemon)
            {
                Pokemon selectedMember = playerParty.Pokemons[currentMember];
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedMember);
            }
            else if (battleAction == BattleAction.Item)
            {

            }
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver)
            {
                yield break;
            }
        }
        
        if (state != BattleState.BattleOver)
        {
            ActionSelection();
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        if (!canRunMove)
        {
            yield return targetUnit.Hud.UpdateHP();
            yield break;
        }
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}は{move.Base.Name}をつかった");

        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.PlayerAttackAnimation();
            yield return new WaitForSeconds(0.5f);
            targetUnit.PlayerHitAnimation();

            if (move.Base.Category == MoveCategory.Stat)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else
            {
                DamageDetails damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (SecondaryEffects secondary in move.Base.Secondaries)
                {
                    if (Random.Range(1, 101) <= secondary.Chance)
                    {
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                    }
                }
            }

            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name}はたおれた！");
                targetUnit.PlayerFaintAnimation();
                yield return new WaitForSeconds(0.5f);
                CheckForBattleOver(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}のこうげきは　はずれた");
        }
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver)
        {
            yield break;
        }

        yield return new WaitUntil(() => state == BattleState.RunnigTurn);
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.UpdateHP();

        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}はたおれた！");
            sourceUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(0.5f);
            CheckForBattleOver(sourceUnit);
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.ApplyBooosts(effects.Boosts);
            }
            else
            {
                target.ApplyBooosts(effects.Boosts);
            }
        }
        
        if (effects.Status != ConditionID.None)
        {
            target.SetStatus(effects.Status);
        }
        if (effects.VolatileStatus != ConditionID.None)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AnyHit)
        {
            return true;
        }
        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        float[] boostValue = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
        {
            moveAccuracy *= boostValue[accuracy];
        }
        else
        {
            moveAccuracy /= boostValue[-accuracy];
        }

        if (evasion > 0)
        {
            moveAccuracy /= boostValue[evasion];
        }
        else
        {
            moveAccuracy *= boostValue[-evasion];
        }

        return Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            string message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog($"きゅうしょに　あたった！");
        }
        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog($"こうかは　ばつぐんだ！");
        }
        if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogBox.TypeDialog($"こうかは　いまひとつだ...");
        }
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentAction--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentAction == 0)
            {
                MoveSelection();
            }
            if (currentAction == 2)
            { 
                preState = state;
                OpenPartyAction();
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMove++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMove--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMove -= 2;
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Move move = playerUnit.Pokemon.Moves[currentMove];
            if (move.PP == 0)
            {
                return;
            }

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            //StartCoroutine(PlayerMove());
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        if (Input.GetKeyDown(KeyCode.LeftCommand))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMember++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMember--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember -= 2;
        }
        

        currentMove = Mathf.Clamp(currentMove, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Pokemon selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0)
            { 
                partyScreen.SetMessage("その　ポケモンは　もう　うごけない");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessage("もうでているよ");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            if (preState == BattleState.ActionSelection)
            {
                preState = null;
                //行動選択でポケモンを入れ替える場合（行動選択の「ポケモン」から）
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                //自分が場に出していたポケモンが戦闘不能になってポケモンを入れ替える場合
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
            //dialogBox.EnableActionSelector(false);
        }
        if (Input.GetKeyDown(KeyCode.LeftCommand))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0) //「ポケモン」ボタンを押して交代した場合
        {
            yield return dialogBox.TypeDialog($"戻れ！ {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        yield return dialogBox.TypeDialog($"いけ{playerUnit.Pokemon.Base.Name}！");
        state = BattleState.RunnigTurn;
    }
    
}