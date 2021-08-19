public enum GameState
{
    Running,
    Pause
}

public enum GameType
{
    Classic,
    Draw3
}

public enum CardSuit
{
    Empty,
    Diamonds,
    Clubs,
    Hearts,
    Spades
}

public enum CardSide
{
    Back,
    Front
}

public enum CardColor
{
    Empty,
    Black,
    Red
}

public enum CardArea
{
    Table,
    DrawPile,
    AcesPile,
}

public enum MoveUndoType
{
    Add,
    Remove
}