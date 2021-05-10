
using System.Collections.Generic;

namespace Chess.PieceFactory
{
    public class King : Piece
    {
        public bool isInCheck = false;
        public bool canCastleK = true;
        public bool canCastleQ = true;

        public List<string> attckingSquares = new List<string>();

        public override Piece CreatePiece()
        {
            return new King();
        }
    }
}
