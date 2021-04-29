using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Chess.PieceFactory
{
    public abstract class Piece : Button
    {
        public bool isBlack;
        public List<string> legalMoves;
        public abstract Piece CreatePiece();
    }
}
