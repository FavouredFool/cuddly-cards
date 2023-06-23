
using System.Threading.Tasks;

public class BackAnimation : CardAnimation
{
    public BackAnimation(CardManager cardManager, CardMover cardMover) : base(cardManager, cardMover) { }


    public override async Task AnimateCards(CardNode mainNode, CardNode previousActiveNode, CardNode rootNode)
    {

    }
}
