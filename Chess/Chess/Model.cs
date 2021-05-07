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

        public int YSourceCoordinate
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
        
        //standard setup for chess, can be substituted for any position
        private string FEN = "8/8/8/8/3K4/4rr2/8/8 w - - 0 1";

        public string FENotation
        {
            get { return FEN; }
            set { FEN = value; }
        }

        private bool isBlackToMove = false;

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

        private bool pieceSelected;

        public bool IsPieceSelected
        {
            get { return pieceSelected; }
            set { pieceSelected = value; }
        }

        private bool destinationPiece;

        public bool IsDestinationPieceSelected
        {
            get { return destinationPiece; }
            set { destinationPiece = value; }
        }

        private List<int> enPassantSquare = new List<int> { 9, 9}; //DEBUG COORDINATES

        public List<int> EnPassantCoordinate
        {
            get { return enPassantSquare; }
            set { enPassantSquare = value; }
        }

        private List<int> specialCaseCoordinates = new List<int> { 9, 9 };

        public List<int> SpecialCaseCoordinates
        {
            get { return specialCaseCoordinates; }
            set { specialCaseCoordinates = value; }
        }

        private List<int> promotionCoordinates;

        public List<int> PromotionCoordinates
        {
            get { return promotionCoordinates; }
            set { promotionCoordinates = value; }
        }

        private bool isPromotion;

        public bool IsPromotion
        {
            get { return isPromotion; }
            set { isPromotion = value; }
        }

        private Piece promotionPiece;

        public Piece PromotionPiece
        {
            get { return promotionPiece; }
            set { promotionPiece = value; }
        }


    }
}
