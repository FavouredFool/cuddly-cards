using UnityEngine;
using static CardInfo;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CardTypes", order = 1)]
public class CardScriptableType : ScriptableObject
{
    [SerializeField]
    CardType _cardType;

    [SerializeField]
    Sprite _cardIcon;

    [SerializeField]
    public Color _cardColor;

    public CardType GetCardType()
    {
        return _cardType;
    }

    public Sprite GetCardIcon()
    {
        return _cardIcon;
    }

    public Color GetCardColor()
    {
        return _cardColor;
    }
}