using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;
    
    [SerializeField] Color poisonColor;
    [SerializeField] Color burnColor;
    [SerializeField] Color paralysisColor;
    [SerializeField] Color sleepColor;
    [SerializeField] Color freezeColor;

    [SerializeField] Text maxHPText;
    [SerializeField] Text currentHPText;

    Pokemon _pokemon;

    Dictionary<ConditionID, Color> statusColors;

    private int tHP;
    public Ease Ease_Type;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lv:" + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHP);

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.Poison, poisonColor},
            {ConditionID.Burn, burnColor},
            {ConditionID.Paralysis, paralysisColor},
            {ConditionID.Sleep, sleepColor},
            {ConditionID.Freeze, freezeColor},
        };
        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;

        maxHPText.text = pokemon.MaxHP.ToString();
        currentHPText.text = pokemon.HP.ToString();
    }

    void SetStatusText()
    {
        if (_pokemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _pokemon.Status.Name;
            statusText.color = statusColors[_pokemon.Status.Id];
        }
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
            DOTween.To(() => tHP, x => tHP = x, _pokemon.HP, 100f).SetEase(Ease_Type);
            currentHPText.text = _pokemon.HP.ToString();

            //currentHPText.text = tHP.ToString();
            //_pokemon.HpChange = false;
        }
    }
}
