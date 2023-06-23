
using DG.Tweening;
using System.Threading.Tasks;

public abstract class CardAnimation
{
    protected CardManager _cardManager;
    protected CardMover _cardMover;

    public CardAnimation(CardManager cardManager, CardMover cardMover)
    {
        _cardManager = cardManager;
        _cardMover = cardMover;
    }

    public abstract Task AnimateCards(CardNode activeNode, CardNode previousActiveNode, CardNode rootNode);
}
