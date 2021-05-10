
using System.Collections.Generic;
using System.Windows.Controls;

namespace Chess.PieceFactory
{
    public abstract class Piece : Button
    {
        public bool isBlack;
        public List<string> legalMoves = new List<string>();

        public string typeOfPiece;
        public abstract Piece CreatePiece();
    }
}
