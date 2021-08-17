using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUICard : MonoBehaviour
{
    [SerializeField]
    private Image _bodySprite = null;

    [SerializeField]
    private TextMeshProUGUI _rankText = null;

    [SerializeField]
    private Image _suitImageBig = null;

    [SerializeField]
    private Image _suitImageSmall = null;

    public void SetCardData(CardData cardData)
    {
        _rankText.text = cardData.Rank.ToString();

        Sprite suitSprite = Resources.Load<Sprite>("Sprite_" + cardData.Suit);
        _suitImageSmall.sprite = suitSprite;
        _suitImageBig.sprite = suitSprite;

        switch (cardData.Rank)
        {
            case 1:
                _rankText.text = "A";
                _suitImageBig.sprite = Resources.Load<Sprite>("Sprite_" + cardData.GetCardColor() + "_Jolly");
                break;

            case 11:
                _rankText.text = "J";
                _suitImageBig.sprite = Resources.Load<Sprite>("Sprite_" + cardData.GetCardColor() + "_Jack");
                break;

            case 12:
                _rankText.text = "Q";
                _suitImageBig.sprite = Resources.Load<Sprite>("Sprite_" + cardData.GetCardColor() + "_Queen");
                break;

            case 13:
                _rankText.text = "K";
                _suitImageBig.sprite = Resources.Load<Sprite>("Sprite_" + cardData.GetCardColor() + "_King");
                break;
        }

        _rankText.color = GetColor(cardData.GetCardColor());
    }

    public void TurnCard(CardSide sideToShow)
    {
        _bodySprite.sprite = Resources.Load<Sprite>("Sprite_" + sideToShow);

        switch (sideToShow)
        {
            case CardSide.Back:
                _rankText.gameObject.SetActive(false);
                _suitImageBig.gameObject.SetActive(false);
                _suitImageSmall.gameObject.SetActive(false);
                break;

            case CardSide.Front:
                _rankText.gameObject.SetActive(true);
                _suitImageBig.gameObject.SetActive(true);
                _suitImageSmall.gameObject.SetActive(true);
                break;
        }
    }

    private Color GetColor(CardColor cardColor)
    {
        if (cardColor == CardColor.Red)
            return Color.red;

        if (cardColor == CardColor.Black)
            return Color.black;

        return Color.black;
    }
}
