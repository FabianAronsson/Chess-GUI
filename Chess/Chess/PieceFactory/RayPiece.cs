using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
