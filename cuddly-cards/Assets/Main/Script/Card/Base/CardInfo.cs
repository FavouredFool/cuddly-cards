

public static class CardInfo
{
    public enum CardType { COVER, PLACE, THING, PERSON, DIALOGUE, KEY, LOCK, INVENTORY, DWRAPPER, TALK }

    public enum CardTraversal { BODY, CONTEXT }

    public enum Click { LEFT, RIGHT }

    public static readonly float CARDWIDTH = 1;

    public static readonly float CARDHEIGHT = 0.005f;

    public static readonly float CARDRATIO = 1.41935f;

    public static readonly float HOVERSPEED = 2.5f;
}