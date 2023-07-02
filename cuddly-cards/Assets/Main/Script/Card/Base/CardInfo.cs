

public static class CardInfo
{
    public enum CardType { COVER, PLACE, THING, PERSON, DIALOGUE, KEY, LOCK, INVENTORY }

    public enum CardTraversal { BODY, CONTEXT }

    public enum CardTransition { CHILD, BACK, TOCOVER, FROMCOVER, CLOSE, OPEN, TOINVENTORY, FROMINVENTORY, ENTERINVENTORYPILE, EXITINVENTORYPILE, COLLECTCARD }

    public static readonly float CARDWIDTH = 1;

    public static readonly float CARDHEIGHT = 0.005f;

    public static readonly float CARDRATIO = 1.41935f;
}