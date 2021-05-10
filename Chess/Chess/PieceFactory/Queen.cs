namespace Chess.PieceFactory
{
    public class Queen : RayPiece
    {
        public override Piece CreatePiece()
        {
            return new Queen();
        }
    }
}
