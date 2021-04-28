using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.PieceFactory
{
    interface IPieceFactory
    {
        Piece CreatePiece(char typeOfPiece, bool isBlack);
    }
}
