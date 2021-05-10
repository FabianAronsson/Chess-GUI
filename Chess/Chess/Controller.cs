using Chess.PieceFactory;
using System;
using System.Collections.Generic;
using System.Media;

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

        /// <summary>
        /// Translates a FEN string and creates a board with the corresponding pieces.
        /// </summary>
        /// <returns></returns>
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
                //If a char is a number, then it means it should insert x amount of EmptySquares.
                //The amount of times depends on the number, if it is 8, then a full row of EmptySquares is created.
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
                //If a char equals /, then it means it should go to the next row.
                else if (FENotation[index].Equals('/'))
                {
                    yIndex++;
                    xIndex = 0;
                }
                //A space is the end of a FEN string. The setup is therefore finished.
                else if (FENotation[index].Equals(' '))
                {
                    break;
                }
                else
                {
                    //If any char equals a letter, then a piece is supposed to be created.
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

        /// <summary>
        /// Validates the move the current player is tring to perform. If it is illegal, then the method returns false.
        /// </summary>
        /// <returns>A boolean.</returns>
        public bool IsMoveLegal()
        {
            Piece piece = model.InternalBoard[model.YSourceCoordinate, model.XSourceCoordinate];
            string destinationCoords = ConvertIntToString(model.DestinationY, model.DestinationX);

            for (int i = 0; i < piece.legalMoves.Count; i++)
            {
                //If the piece color is the same as the current turn
                if (piece.isBlack == model.IsItBlackToMove)
                {
                    //Then check if the move exists on the piece which is trying to move.
                    if (piece.legalMoves[i] == destinationCoords)
                    {
                        //Checks if a move is illegal when the king is under attack.
                        if (IsMoveIllegal(piece) && !(piece is King))
                        {
                            return false;
                        }
                        else
                        {
                            //Due to a reference bug, the actual board is somehow being edited. This means that
                            //New moves has to be generated again because the previously legal moves are now heavily inaccurate.
                            GenerateLegalMoves();
                        }

                        //If the move passed the previous checks; special values are set on pieces that have special moves.

                        //Special cases for pawns, en passants, double moves, promotions
                        if (piece is Pawn pawn)
                        {
                            pawn.canDoubleMove = false;
                            SetSpecialPawnProperties(pawn, pawn.legalMoves[i]);
                        }
                        //Special cases for kings, castling
                        else if (piece is King king)
                        {
                            SetSpecialKingProperties(king, king.legalMoves[i]);
                            king.canCastleK = false;
                            king.canCastleQ = false;
                        }
                        //Special cases for rooks, castling
                        else if (piece is Rook)
                        {
                            SetSpecialRookProperties();
                        }

                        //Change turn order
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

        public bool DoesNextPlayerHaveLegalMoves()
        {
            bool isBlack;
            //The current turncolor has to be the opposite for the next player. It is later used in validation.
            if (!model.IsItBlackToMove)
            {
                isBlack = false;
            }
            else
            {
                isBlack = true;
            }

            King tempKing = null;
            if (IsKingUnderAttack(isBlack))
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        //If the current piece equals that of the turnorder and it is not an EmptySquare
                        if (model.InternalBoard[i, j].isBlack == isBlack && !(model.InternalBoard[i, j] is EmptySquare))
                        {
                            //Gets the current king
                            if (model.InternalBoard[i, j] is King king)
                            {
                                tempKing = king;
                            }
                            
                            if (IsMoveIllegal(model.InternalBoard[i, j]))
                            {
                                return false;
                            }

                            //Due to previous bugs in the "IsMoveIllegal()" method.
                            GenerateLegalMoves();
                        }
                    }
                }
            }

            //If the king has no legal moves, then it must mean that it is checkmate.
            if (tempKing != null && tempKing.legalMoves.Count == 0)
            {
                return false;
            }

            return true;
        }

       
        /// <summary>
        /// Checks if a piece is attacking the king. Other methods that use this method, checks if the king is under attack
        /// after a move has been played out.
        /// </summary>
        /// <param name="currentPiece">The piece to check.</param>
        /// <returns>A boolean.</returns>
        private bool IsMoveIllegal(Piece currentPiece)
        {
            Piece[,] tempBoard = (Piece[,])model.InternalBoard.Clone();
            MoveGenerator generate = new MoveGenerator();
            PieceFactory.PieceFactory factory = new PieceFactory.PieceFactory();

            tempBoard[model.DestinationY, model.DestinationX] = currentPiece;
            tempBoard[model.YSourceCoordinate, model.XSourceCoordinate] = factory.CreatePiece('S', true);

            bool isBlack = currentPiece.isBlack;

            tempBoard = (Piece[,])generate.GeneratePseudoLegalMoves(tempBoard, model.EnPassantCoordinate).Clone();
            string kingCoordinates = GetKingCoordinates(isBlack);

            //Check if any "legal moves" of the opposite color equals the king coordinate.
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!(tempBoard[i, j] is EmptySquare) && !(tempBoard[i, j] is King))
                    {
                        if (tempBoard[i, j].legalMoves.Contains(kingCoordinates))
                        {
                            return true;
                        }
                    }

                }
            }
            tempBoard[model.DestinationY, model.DestinationX] = factory.CreatePiece('S', true);
            tempBoard[model.YSourceCoordinate, model.XSourceCoordinate] = currentPiece;

            return false;
        }

        public void PlaySound(bool isBlack, bool isCheck)
        {
            if (isCheck)
            {
                //If the king is under attack then play the check sound.
                if (IsKingUnderAttack(!isBlack))
                {
                    SoundPlayer player = new SoundPlayer("../../Sound/Check.wav");
                    player.Play();
                }
            }
            else
            {
                //Otherwise play normal moves, that is capture or move sounds.
                if (DetermineTypeOfMove(isBlack))
                {
                    SoundPlayer player = new SoundPlayer("../../Sound/Capture.wav");
                    player.Play();
                }
                else
                {
                    SoundPlayer player = new SoundPlayer("../../Sound/Move.wav");
                    player.Play();
                }
            }



        }

        /// <summary>
        /// Checks if any piece is attacking the king.
        /// </summary>
        /// <param name="isBlack"></param>
        /// <returns></returns>
        private bool IsKingUnderAttack(bool isBlack)
        {
            string kingCoordinates = GetKingCoordinates(isBlack);
            //check if any "legal moves" of the opposite color equals the king coordinate.
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!(model.InternalBoard[i, j] is EmptySquare) && !(model.InternalBoard[i, j] is King))
                    {
                        //If the current piece is equal to the kings coordinates, then the king is under attack,
                        if (model.InternalBoard[i, j].legalMoves.Contains(kingCoordinates))
                        {
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        private string GetKingCoordinates(bool isBlack)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (model.InternalBoard[i, j] is King && model.InternalBoard[i, j].isBlack == isBlack)
                    {
                        return i + " " + j;
                    }
                }
            }
            return "";
        }

        private bool DetermineTypeOfMove(bool isBlack)
        {
            //If the destination piece is not equal to the current piece's color and if it is a piece, then return true.
            //This also means that the move is capture move.
            if (isBlack != model.InternalBoard[model.DestinationY, model.DestinationX].isBlack && !(model.InternalBoard[model.DestinationY, model.DestinationX] is EmptySquare))
            {
                return true;
            }
            //Otherwise it is a normal move.
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Sets specific values that disables castling if the rook has moved.
        /// </summary>
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

        /// <summary>
        /// Sets special coordinates if the player has decided to castle.
        /// These values are later used when determining where the king and rook is supposed to move during castling.
        /// The values are specific for each king.
        /// </summary>
        /// <param name="king">The king to check.</param>
        /// <param name="currentLegalMove">The castling move the user performed.</param>
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
            //If the delta of destination coordinate and source coordinate equals 2, then the move made was a double move. 
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
            else if (currentLegalMove.Equals(model.EnPassantCoordinate[0] + " " + model.EnPassantCoordinate[1]))
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
            else
            {
                //Basically a reset of en passant coordinates, so that previous positions are not saved.
                model.EnPassantCoordinate[0] = 9;
                model.EnPassantCoordinate[1] = 9;
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

        public Piece GetDestinationPiece()
        {
            return model.InternalBoard[model.DestinationY, model.DestinationX];
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
            model.InternalBoard = (Piece[,])generate.GeneratePseudoLegalMoves(model.InternalBoard, model.EnPassantCoordinate).Clone();
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
