namespace Chess.PieceFactory
{
    interface IPieceFactory
    {
        Piece CreatePiece(char typeOfPiece, bool isBlack);
    }
}
