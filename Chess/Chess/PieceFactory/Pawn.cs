using System.Collections.Generic;

namespace Chess.PieceFactory
{
    public class Pawn : Piece
    {
        public bool canDoubleMove = true;
        public bool canPassant = false;
        public List<string> attckingSquares = new List<string>();


        public override Piece CreatePiece()
        {
            return new Pawn();
        }
    }

}

