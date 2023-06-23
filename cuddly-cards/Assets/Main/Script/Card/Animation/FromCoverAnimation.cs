
using System.Threading.Tasks;

public class FromCoverAnimation : CardAnimation
{
    public FromCoverAnimation(CardManager cardManager, CardMover cardMover) : base(cardManager, cardMover) { }

    public override async Task AnimateCards(CardNode mainNode, CardNode previousActiveNode, CardNode rootNode)
    {

    }
}
