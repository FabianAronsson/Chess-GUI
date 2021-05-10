namespace Chess.PieceFactory
{
    public class Bishop : RayPiece
    {
        public override Piece CreatePiece()
        {
            return new Bishop();
        }
    }
}
