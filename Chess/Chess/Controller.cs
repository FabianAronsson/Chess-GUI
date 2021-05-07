using Chess.PieceFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chess
{
    public class Controller
    {
        private Model model;

        private Controller()
        {
            model = new Model();
        }

        public static Controller InitMainController()
        {
            return new Controller();
        }

        //todo document method
        public Piece[,] CreatePieces()
        {
            PieceFactory.PieceFactory factory = new PieceFactory.PieceFactory();
            Piece[,] pieces = new Piece[8, 8];
            char[] FENotation = ConvertStringToCharArray(model.FENotation);
            int xIndex = 0;
            int yIndex = 0;
            int index = 0;
            while (true)
            {
                if (char.IsDigit(FENotation[index]))
                {
                    if (int.Parse(FENotation[index].ToString()) == 8)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            pieces[yIndex, xIndex] = factory.CreatePiece('S', true);
                            xIndex++;
                        }
                        xIndex = 0;
                    }
                    else
                    {
                        int tempIndex = xIndex;
                        for (int i = 0; i < int.Parse(FENotation[index].ToString()); i++)
                        {
                            pieces[yIndex, tempIndex] = factory.CreatePiece('S', true);
                            tempIndex++;
                        }

                        xIndex += Convert.ToInt32(FENotation[index].ToString());
                    }
                }
                else if (FENotation[index].Equals('/'))
                {
                    yIndex++;
                    xIndex = 0;
                }
                else if (FENotation[index].Equals(' '))
                {
                    break;
                }
                else
                {
                    pieces[yIndex, xIndex] = factory.CreatePiece(FENotation[index], IsFENPieceBlack(FENotation, index));
                    if (xIndex == 7)
                    {
                        xIndex = 0;
                    }
                    else
                    {
                        xIndex++;
                    }

                }
                index++;
            }
            model.InternalBoard = pieces;
            return pieces;
        }



        public bool IsFENPieceBlack(char[] FENChar, int index)
        {
            if (char.IsLower(FENChar[index]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public char[] ConvertStringToCharArray(string str)
        {
            return str.ToCharArray();
        }

        public void SaveCoordinates(int y, int x)
        {

            if (!model.IsPieceSelected)
            {
                model.YSourceCoordinate = y;
                model.XSourceCoordinate = x;
                model.IsPieceSelected = true;
            }
            else
            {
                model.DestinationY = y;
                model.DestinationX = x;
                model.IsDestinationPieceSelected = true;
            }

        }

        public bool IsDestinationPieceSelected()
        {
            return model.IsDestinationPieceSelected;
        }

        public void SavePositionOfSquare(int y, int x)
        {
            if (model.IsPieceSelected)
            {
                model.DestinationY = y;
                model.DestinationX = x;
                model.IsDestinationPieceSelected = true;
            }
        }

        //todo document method
        public bool IsMoveLegal()
        {
            Piece piece = model.InternalBoard[model.YSourceCoordinate, model.XSourceCoordinate];
            string destinationCoords = ConvertIntToString(model.DestinationY, model.DestinationX);

            for (int i = 0; i < piece.legalMoves.Count; i++)
            { //Needs more validation

                if (piece.isBlack == model.IsItBlackToMove)
                {
                    if (piece.legalMoves[i] == destinationCoords)
                    {

                        //Special cases for pawns
                        if (piece is Pawn pawn)
                        {
                            pawn.canDoubleMove = false;
                            SetSpecialPawnProperties(pawn, pawn.legalMoves[i]);
                        }
                        //Special cases for kings
                        else if (piece is King king)
                        {
                            SetSpecialKingProperties(king, king.legalMoves[i]);
                            king.canCastleK = false;
                            king.canCastleQ = false;
                        }
                        else if (piece is Rook)
                        {
                            SetSpecialRookProperties();
                        }

                        if (piece.isBlack)
                        {
                            model.IsItBlackToMove = false;
                        }
                        else
                        {
                            model.IsItBlackToMove = true;
                        }

                        return true;
                    }

                }
            }
            return false;
        }

        private void SetSpecialRookProperties()
        {
            if (model.YSourceCoordinate == (0) && model.XSourceCoordinate == (7))
            {
                if (GetSpecificPiece(0, 4) is King kingPiece)
                {
                    kingPiece.canCastleK = false;
                    model.InternalBoard[0, 4] = kingPiece;
                }
            }
            else if (model.YSourceCoordinate == (7) && model.XSourceCoordinate == (7))
            {
                if (GetSpecificPiece(7, 4) is King kingPiece)
                {
                    kingPiece.canCastleK = false;
                    model.InternalBoard[7, 4] = kingPiece;
                }
            }

            if (model.YSourceCoordinate == (0) && model.XSourceCoordinate == (0))
            {
                if (GetSpecificPiece(0, 4) is King kingPiece)
                {
                    kingPiece.canCastleQ = false;
                    model.InternalBoard[0, 4] = kingPiece;
                }
            }
            else if (model.YSourceCoordinate == (7) && model.XSourceCoordinate == (0))
            {
                if (GetSpecificPiece(7, 4) is King kingPiece)
                {
                    kingPiece.canCastleQ = false;
                    model.InternalBoard[7, 4] = kingPiece;
                }
            }
        }

        private void SetSpecialKingProperties(King king, string currentLegalMove)
        {
            if (king.isBlack)
            {
                if (king.canCastleK)
                {
                    if (currentLegalMove.Equals(0 + " " + 6))
                    {
                        model.SpecialCaseCoordinates[0] = 0;
                        model.SpecialCaseCoordinates[1] = 7;

                        var tempPiece = model.InternalBoard[0, 5];
                        model.InternalBoard[0, 5] = model.InternalBoard[0, 7];
                        model.InternalBoard[0, 7] = tempPiece;

                        model.SpecialCaseCoordinates.Add(0);
                        model.SpecialCaseCoordinates.Add(5);

                    }

                }

                if (king.canCastleQ)
                {
                    if (currentLegalMove.Equals(0 + " " + 2))
                    {
                        model.SpecialCaseCoordinates[0] = 0;
                        model.SpecialCaseCoordinates[1] = 0;


                        var tempPiece = model.InternalBoard[0, 3];
                        model.InternalBoard[0, 3] = model.InternalBoard[0, 0];
                        model.InternalBoard[0, 0] = tempPiece;


                        model.SpecialCaseCoordinates.Add(0);
                        model.SpecialCaseCoordinates.Add(3);

                    }
                }
            }
            else
            {
                if (king.canCastleK)
                {
                    if (currentLegalMove.Equals(7 + " " + 6))
                    {
                        model.SpecialCaseCoordinates[0] = 7;
                        model.SpecialCaseCoordinates[1] = 7;

                        var tempPiece = model.InternalBoard[7, 5];
                        model.InternalBoard[7, 5] = model.InternalBoard[7, 7];
                        model.InternalBoard[7, 7] = tempPiece;

                        model.SpecialCaseCoordinates.Add(7);
                        model.SpecialCaseCoordinates.Add(5);

                    }

                }

                if (king.canCastleQ)
                {
                    if (currentLegalMove.Equals(7 + " " + 2))
                    {
                        model.SpecialCaseCoordinates[0] = 7;
                        model.SpecialCaseCoordinates[1] = 0;


                        var tempPiece = model.InternalBoard[7, 3];
                        model.InternalBoard[7, 3] = model.InternalBoard[7, 0];
                        model.InternalBoard[7, 0] = tempPiece;


                        model.SpecialCaseCoordinates.Add(7);
                        model.SpecialCaseCoordinates.Add(3);

                    }
                }
            }
        }

        private void SetSpecialPawnProperties(Pawn pawn, string currentLegalMove)
        {
            //if the delta of destination coordinate and source coordinate equals 2, then the move made was a double move. 
            //The coordinate behind the pawn is then saved as an eligible en passant square.
            if (model.DestinationY - model.YSourceCoordinate == 2)
            {
                model.EnPassantCoordinate[0] = model.YSourceCoordinate + 1;
                model.EnPassantCoordinate[1] = model.XSourceCoordinate;

            }
            else if (model.YSourceCoordinate - model.DestinationY == 2)
            {
                model.EnPassantCoordinate[0] = model.YSourceCoordinate - 1;
                model.EnPassantCoordinate[1] = model.XSourceCoordinate;
            }

            if (currentLegalMove.Equals(model.EnPassantCoordinate[0] + " " + model.EnPassantCoordinate[1]))
            {
                if (pawn.isBlack)
                {
                    model.SpecialCaseCoordinates[0] = model.EnPassantCoordinate[0] - 1;
                    model.SpecialCaseCoordinates[1] = model.EnPassantCoordinate[1];
                }
                else
                {
                    model.SpecialCaseCoordinates[0] = model.EnPassantCoordinate[0] + 1;
                    model.SpecialCaseCoordinates[1] = model.EnPassantCoordinate[1];
                }
            }

            //If any pawn move is on the 0th or 7th rank then that means that the pawn is on the final file, which also means it should promote.
            if (model.DestinationY == 0 || model.DestinationY == 7)
            {
                model.IsPromotion = true;
            }
        }

        public void ResetSpecialValues()
        {
            model.EnPassantCoordinate[0] = 9;
            model.EnPassantCoordinate[1] = 9;
        }

        private string ConvertIntToString(int y, int x)
        {
            return y + " " + x;
        }

        public void UpdateMovesOnBoard()
        {
            PieceFactory.PieceFactory factory = new PieceFactory.PieceFactory();
            Piece[,] pieces = model.InternalBoard;
            Piece sourcePiece = model.InternalBoard[model.YSourceCoordinate, model.XSourceCoordinate];

            pieces[model.YSourceCoordinate, model.XSourceCoordinate] = factory.CreatePiece('S', true);

            if (model.PromotionPiece != null)
            {
                pieces[model.DestinationY, model.DestinationX] = model.PromotionPiece;
                model.IsPromotion = false;
                model.PromotionPiece = null;

            }
            else
            {
                pieces[model.DestinationY, model.DestinationX] = sourcePiece;
            }
            model.InternalBoard = pieces;

        }

        public void ResetPieceValues()
        {
            model.IsPieceSelected = false;
            model.IsDestinationPieceSelected = false;
            model.SpecialCaseCoordinates = new List<int> { 9, 9 };
        }

        public Piece CreatePiece(char typeOfPiece, bool isBlack)
        {
            PieceFactory.PieceFactory factory = new PieceFactory.PieceFactory();
            return factory.CreatePiece(typeOfPiece, isBlack);
        }

        public Piece GetSourcePiece()
        {
            return model.InternalBoard[model.YSourceCoordinate, model.XSourceCoordinate];

        }

        public List<int> GetDestinationCoordinates()
        {
            return new List<int> { model.DestinationY, model.DestinationX };
        }

        public List<int> GetSourceCoordinates()
        {
            return new List<int> { model.YSourceCoordinate, model.XSourceCoordinate };
        }

        public void GenerateLegalMoves()
        {
            MoveGenerator generate = new MoveGenerator();
            generate.GeneratePseudoLegalMoves(model.InternalBoard, model.EnPassantCoordinate);
        }

        public List<int> GetSpecialCaseCoordinates()
        {
            return model.SpecialCaseCoordinates;
        }

        public Piece GetSpecificPiece(int y, int x)
        {
            return model.InternalBoard[y, x];
        }

        public bool GetIsPromotion()
        {
            return model.IsPromotion;
        }

        public Piece GetPromotionPiece()
        {
            return model.PromotionPiece;
        }

        public void SetPromotionPiece(Piece promotionPiece)
        {
            model.PromotionPiece = promotionPiece;
        }

        public bool GetTurnOrder()
        {
            return model.IsItBlackToMove;
        }
    }
}
