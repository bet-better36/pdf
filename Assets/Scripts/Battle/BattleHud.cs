using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;

    [SerializeField] Text maxHPText;
    [SerializeField] Text currentHPText;

    Pokemon _pokemon;
    private int tHP;
    public Ease Ease_Type;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lv:" + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHP);
        maxHPText.text = pokemon.MaxHP.ToString();
        currentHPText.text = pokemon.HP.ToString();
    }


    public void HPTextAnimation()
    {
        //DOTween.To(() => tHP, x => tHP = x, 4000, 1f).SetEase(Ease_Type);
    }

    public IEnumerator UpdateHP()
    {
        if (_pokemon.HpChange)
        {
            yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHP);
            _pokemon.HpChange = false;
        }
        currentHPText.text = _pokemon.HP.ToString();
        //DOTween.To(() => tHP, x => tHP = x, _pokemon.HP, 100f).SetEase(Ease_Type);
        //currentHPText.text = tHP.ToString();
    }
}
