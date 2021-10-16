using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    [SerializeField] float enterTime;
    [SerializeField] float animationAttackTime;
    [SerializeField] float animationBackTime;


    public Pokemon Pokemon { get; set; }
    public bool IsPlayerUnit { get => isPlayerUnit; }
    public BattleHud Hud { get => hud; }

    Vector3 originalPos;
    Color originalColor;
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Pokemon pokemon)
    {
        Pokemon = pokemon;

        if (isPlayerUnit)
        {
            image.sprite = Pokemon.Base.BackSprite;
        }
        else
        {
            image.sprite = Pokemon.Base.FrontSprite;
        }
        hud.SetData(pokemon);
        image.color = originalColor;
        PlayerEnterAnimetion();
    }

    public void PlayerEnterAnimetion()
    {
        if (IsPlayerUnit)
        {
            transform.localPosition = new Vector3(-1000, originalPos.y);
        }
        else
        {
            transform.localPosition = new Vector3(1000, originalPos.y);
        }
        transform.DOLocalMoveX(originalPos.x, enterTime);
    }
    public void PlayerAttackAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        if (IsPlayerUnit)
        {
            sequence.Append(transform.DOLocalMoveX(originalPos.x + 30f, animationAttackTime));
        }
        else
        {
            sequence.Append(transform.DOLocalMoveX(originalPos.x - 30f, animationAttackTime));
        }

        sequence.Append(transform.DOLocalMoveX(originalPos.x, animationBackTime));
    }

    public void PlayerHitAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayerFaintAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0, 0.5f));
    }
}
