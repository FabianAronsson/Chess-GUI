using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Chess.PieceFactory
{
    public class Pawn : Piece
    {
        public bool canDoubleMove = true;
        public bool canPassant = false;
        public override Piece CreatePiece()
        {
            return new Pawn();
        }


    }

}

