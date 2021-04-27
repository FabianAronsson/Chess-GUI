using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.PieceFactory
{
    public class King : Piece
    {
        public bool isInCheck = false;
        public bool canCastleK = true;
        public bool canCastleQ = true;
        public override Piece CreatePiece()
        {
            return new King();
        }
    }
}
