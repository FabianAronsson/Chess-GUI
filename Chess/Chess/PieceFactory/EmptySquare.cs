namespace Chess.PieceFactory
{
    public class EmptySquare : Piece
    {
        public bool isSpecialPiece;
        //Not an ideal solution, since EmptySquare inherits unused fields. However, due to limitations in WPF-technology, this was neccessary.
        public override Piece CreatePiece()
        {
            return new EmptySquare();
        }
    }
}
