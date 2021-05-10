
using System.Collections.Generic;

namespace Chess.PieceFactory
{
    public class RayPiece : Piece
    {
        public List<string> attckingSquares = new List<string>();

        public override Piece CreatePiece()
        {
            return new RayPiece();
        }
    }
}
