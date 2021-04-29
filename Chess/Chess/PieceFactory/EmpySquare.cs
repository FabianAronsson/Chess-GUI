using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.PieceFactory
{
    public class EmpySquare : Piece
    {
        public bool isSpecialPiece;
        //Not an ideal solution, since EmptySquare inherits unused fields. However, due to limitations in WPF-technology, this was neccessary.
        public override Piece CreatePiece()
        {
            return new EmpySquare();
        }
    }
}
