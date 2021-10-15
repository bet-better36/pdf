using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;

    //[SerializeField] Text maxHPText;
    //[SerializeField] Text hpText;

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lv:" + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHP);
        //maxHPText.text = pokemon.MaxHP.ToString();
        //hpText.text = pokemon.HP.ToString();
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHP);

    }

}
