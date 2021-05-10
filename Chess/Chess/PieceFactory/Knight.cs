namespace Chess.PieceFactory
{
    public class Knight : Piece
    {

        public override Piece CreatePiece()
        {
            return new Knight();
        }
    }
}
