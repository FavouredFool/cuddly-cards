
using System.Threading.Tasks;

public class ChildAnimation : CardAnimation
{
    public ChildAnimation(CardManager cardManager, CardMover cardMover) : base(cardManager, cardMover) { }
   

    public override async Task AnimateCards(CardNode mainNode, CardNode previousActiveNode, CardNode rootNode)
    {

    }
}
