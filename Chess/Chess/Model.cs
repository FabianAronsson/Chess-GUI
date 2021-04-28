using Chess.PieceFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Chess
{
    public class Model
    {
        private int srcX;

        public int XSourceCoordinate
        {
            get { return srcX; }
            set { srcX = value; }
        }

        private int srcY;

        public int YSouceCoordinate
        {
            get { return srcY; }
            set { srcY = value; }
        }

        private int destX;

        public int DestinationX
        {
            get { return destX; }
            set { destX = value; }
        }
        private int destY;

        public int DestinationY
        {
            get { return destY; }
            set { destY = value; }
        }

        private string FEN = "5rk1/6p1/6Rp/2p1p3/1pq5/5P1P/P6P/R2Q2NK b - - 0 32";

        public string FENotation
        {
            get { return FEN; }
            set { FEN = value; }
        }

        private bool isBlackToMove;

        public bool IsItBlackToMove
        {
            get { return isBlackToMove; }
            set { isBlackToMove = value; }
        }

        private Piece[,] board;

        public Piece[,] InternalBoard
        {
            get { return board; }
            set { board = value; }
        }

        private Grid extBoard;

        public Grid ExternalBoard
        {
            get { return extBoard; }
            set { extBoard = value; }
        }

    }
}
