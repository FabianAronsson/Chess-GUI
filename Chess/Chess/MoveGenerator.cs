using Chess.PieceFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class MoveGenerator
    {
        private Piece[,] board;
        private bool isBlackToMove;

        public Piece[,] GeneratePseudoLegalMoves(Piece[,] board, bool isBlackToMove, List<int> enPassantSquare) //board is binded to Internalboard.
        {
            this.board = board;
            this.isBlackToMove = isBlackToMove;
            ResetPreviousLegalMoves(board);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var currentPiece = board[i, j];
                    switch (board[i, j].typeOfPiece)
                    {
                        case "Rook":
                            board[i, j] = GenerateLateralMoves(currentPiece, i, j, 7);
                            break;
                        case "Bishop":
                            board[i, j] = GenerateDiagonalMoves(currentPiece, i, j, 7);
                            break;
                        case "Queen":
                            board[i, j] = GenerateQueenMoves(currentPiece, i, j);
                            break;
                        case "Knight":
                            board[i, j] = GenerateKnightMoves(currentPiece, i, j);
                            break;
                        case "Pawn":
                            board[i, j] = GeneratePawnMoves(currentPiece, i, j, enPassantSquare);
                            break;
                        default:
                            break;
                    }
                }
            }


            this.board = board;


            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var currentPiece = board[i, j];
                    if(board[i, j].typeOfPiece.Equals("King")){
                        board[i, j] = GenerateKingMoves(currentPiece, i, j);
                    }     
                }
            }

            return board; //not actually neccessary
        }

        private void ResetPreviousLegalMoves(Piece[,] board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j].legalMoves.Clear();
                }

            }
        }

        private Piece GenerateQueenMoves(Piece currentPiece, int y, int x)
        {
            List<string> legalMoves = new List<string>();

            //since a queen is just a bishop and a rook combinded, their respective methods can be used to generate pseudo-legal moves
            legalMoves.AddRange(GenerateLateralMoves(currentPiece, y, x, 7).legalMoves);
            legalMoves.AddRange(GenerateDiagonalMoves(currentPiece, y, x, 7).legalMoves);

            currentPiece.legalMoves = legalMoves;
            return currentPiece;
        }

        //todo, document this monster of a method.
        private Piece GenerateLateralMoves(Piece currentPiece, int y, int x, int maxLoopValue)
        {
            Rook rook = currentPiece as Rook;
            var move = new MoveModel(rook, y, x);

            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 1: //East
                        move.YOffset = 0;
                        move.XOffset = 1;
                        move.ActiveDirection = x;
                        move.DirValue = 1;
                        move.MaxValue = 8;
                        move.N = 1;
                        move.IsPositiveOperation = true;
                        break;
                    case 2: //South
                        move.YOffset = 1;
                        move.XOffset = 0;
                        move.ActiveDirection = y;
                        move.IsPositiveOperation = true;
                        break;
                    case 3: //West
                        move.YOffset = 0;
                        move.XOffset = -1;
                        move.ActiveDirection = x;
                        move.DirValue = -1;
                        move.MaxValue = -1;
                        move.N = -1;
                        move.IsPositiveOperation = false;
                        break;
                    default:
                        break;
                }
                for (int j = 0; j < maxLoopValue; j++)
                {
                    if (!move.IsPositiveOperation)
                    {
                        if (move.ActiveDirection + move.DirValue > move.MaxValue)
                        {
                            //Checks whether or not the next square is a piece of the same color and if its a piece on the square
                            if (currentPiece.isBlack == board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                            {
                                break;
                            }
                            //If not, then add that move to legal moves
                            else
                            {
                                currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                                move.ActiveDirection += move.N;

                                //If the next position is a piece, then no more moves can be generated for that direction.
                                if (currentPiece.isBlack != board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                                {
                                    break;
                                }

                                if (move.YOffset == 0)
                                {
                                    move.XOffset += move.N;
                                }
                                else if (move.XOffset == 0)
                                {
                                    move.YOffset += move.N;
                                }
                            }
                        }
                        else
                        {
                            move.TempY = y;
                            move.TempX = x;
                            move.IsPositiveOperation = true;
                            break;
                        }
                    }
                    else
                    {
                        if (move.ActiveDirection + move.DirValue < move.MaxValue && (move.XOffset >= 0 && move.YOffset >= 0))
                        {
                            if (currentPiece.isBlack == board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                            {
                                break;
                            }
                            else
                            {
                                currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                                move.ActiveDirection += move.N;

                                if (currentPiece.isBlack != board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                                {
                                    break;
                                }

                                if (move.YOffset == 0)
                                {
                                    move.XOffset += move.N;
                                }
                                else if (move.XOffset == 0)
                                {
                                    move.YOffset += move.N;
                                }
                            }
                        }
                        else
                        {
                            move.TempY = y;
                            move.TempX = x;
                            move.IsPositiveOperation = false;
                            break;
                        }
                    }
                }
            }
            return currentPiece;
        }

        //todo, document this monster of a method.
        private Piece GenerateDiagonalMoves(Piece currentPiece, int y, int x, int maxLoopValue)
        {
            Bishop bishop = currentPiece as Bishop;
            var move = new MoveModel(bishop, y, x);

            //delegates for choosing what operation to use
            Func<int, int, bool> greaterThanDelegate = (a, b) => a > b;
            Func<int, int, bool> lessThanDelegate = (a, b) => a < b;
            var operationY = greaterThanDelegate;
            var operationX = greaterThanDelegate;

            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    //document what the numbers mean
                    case 1: //Northeast
                        move.YOffset = -1;
                        move.XOffset = 1;
                        move.ActiveDirectionY = y;
                        move.ActiveDirectionX = x;
                        move.DirValueY = -1;
                        move.DirValueX = 1;
                        move.MaxValueY = -1;
                        move.MaxValueX = 8;
                        move.NY = -1;
                        move.NX = 1;
                        operationY = greaterThanDelegate;
                        operationX = lessThanDelegate;
                        break;
                    case 2: //Southeast
                        move.YOffset = 1;
                        move.XOffset = 1;
                        move.ActiveDirectionY = y;
                        move.ActiveDirectionX = x;
                        move.DirValueY = 1;
                        move.DirValueX = 1;
                        move.MaxValueY = 8;
                        move.MaxValueX = 8;
                        move.NY = 1;
                        move.NX = 1;
                        operationY = lessThanDelegate;
                        operationX = lessThanDelegate;
                        break;
                    case 3: //Southwest
                        move.YOffset = 1;
                        move.XOffset = -1;
                        move.ActiveDirectionY = y;
                        move.ActiveDirectionX = x;
                        move.DirValueY = 1;
                        move.DirValueX = -1;
                        move.MaxValueY = 8;
                        move.MaxValueX = -1;
                        move.NY = 1;
                        move.NX = -1;
                        operationY = lessThanDelegate;
                        operationX = greaterThanDelegate;
                        break;
                    default:
                        break;
                }

                for (int j = 0; j < maxLoopValue; j++)
                {
                    if (operationY(move.ActiveDirectionY + move.DirValueY, move.MaxValueY) && operationX(move.ActiveDirectionX + move.DirValueX, move.MaxValueX))
                    {
                        if (currentPiece.isBlack == board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                        {
                            break;
                        }
                        else
                        {
                            currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                            move.ActiveDirectionY += move.NY;
                            move.ActiveDirectionX += move.NX;

                            if (currentPiece.isBlack != board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                            {
                                break;
                            }
                            move.XOffset += move.NX;
                            move.YOffset += move.NY;
                        }
                    }
                }
            }
            return currentPiece;
        }

        //todo, document this monster of a method.
        private Piece GenerateKnightMoves(Piece currentPiece, int y, int x)
        {
            Knight knight = currentPiece as Knight;
            var move = new MoveModel(knight, y, x);
            //delegates for choosing what operation to use
            Func<int, int, bool> greaterThanDelegate = (a, b) => a > b;
            Func<int, int, bool> lessThanDelegate = (a, b) => a < b;
            var operationY = greaterThanDelegate;
            var operationX = greaterThanDelegate;

            var newOperationY = greaterThanDelegate;
            var newOperationX = lessThanDelegate;

            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    //document what the numbers mean
                    case 1: //East
                        move.YOffset = -1;
                        move.XOffset = 2;
                        move.MaxValueY = -1;
                        move.MaxValueX = 8;
                        move.NewMaxValueY = 8;
                        move.NY = -1;
                        move.NX = 1;
                        operationY = greaterThanDelegate;
                        operationX = lessThanDelegate;
                        newOperationY = lessThanDelegate;
                        break;
                    case 2: //South
                        move.YOffset = 2;
                        move.XOffset = -1;
                        move.MaxValueY = 8;
                        move.MaxValueX = -1;
                        move.NewMaxValueX = 8;
                        move.NY = 1;
                        move.NX = -1;
                        operationY = lessThanDelegate;
                        operationX = greaterThanDelegate;
                        newOperationX = lessThanDelegate;
                        break;
                    case 3: //West
                        move.YOffset = -1;
                        move.XOffset = -2;
                        move.MaxValueY = -1;
                        move.MaxValueX = -1;
                        move.NewMaxValueY = 8;
                        move.NY = -1;
                        move.NX = 1;
                        operationY = greaterThanDelegate;
                        operationX = greaterThanDelegate;
                        newOperationY = lessThanDelegate;
                        break;
                    default:
                        break;
                }

                //document what this thing does
                for (int j = 0; j < 2; j++)
                {
                    if ((operationY(move.TempY + move.YOffset, move.MaxValueY) && operationX(move.TempX + move.XOffset, move.MaxValueX)) && (move.TempY + move.YOffset > -1 && move.TempX + move.XOffset > -1))
                    {
                        if (!(currentPiece.isBlack == board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare)))
                        {
                            currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                        }
                    }
                    move.YOffset *= move.NY;
                    move.XOffset *= move.NX;
                    move.MaxValueY = move.NewMaxValueY;
                    move.MaxValueX = move.NewMaxValueX;
                    operationY = newOperationY;
                    operationX = newOperationX;

                }
            }
            return currentPiece;
        }

        //todo, document this monster of a method.
        private Piece GeneratePawnMoves(Piece currentPiece, int y, int x, List<int> enPassantCoordinate)
        {
            Pawn pawn = currentPiece as Pawn;
            //Assuming it is a black pawn those values are set, otherwise it is set to white on line 389.
            var move = new MoveModel(pawn, y, x, true);

            bool isDiagonalMove = false;

            //delegates for choosing what operation to use
            Func<int, int, bool> greaterThanDelegate = (a, b) => a > b;
            Func<int, int, bool> lessThanDelegate = (a, b) => a < b;
            var operationY = lessThanDelegate;
            var operationX = lessThanDelegate;

            var newOperationX = greaterThanDelegate;
            var newOppositeOperationX = lessThanDelegate;

            //set position specific values for white
            if (!currentPiece.isBlack)
            {
                move = new MoveModel(pawn, y, x, false);
                operationY = greaterThanDelegate;
            }

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (operationY(move.ActiveDirectionY + move.DirValueY, move.MaxValueY) && operationX(move.ActiveDirectionX + move.DirValueX, move.MaxValueX))
                    {
                        //Checks whether or not the next square is a piece of the same color and if its a piece on the square
                        if (currentPiece.isBlack == board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                        {
                            break;
                        }
                        //If not, then add that move to legal moves
                        else
                        {
                            //Check moves that goes either up or down depending on color.
                            if (!isDiagonalMove)
                            {
                                //If the next position is a piece, then no more moves can be generated for that direction.
                                if (currentPiece.isBlack != board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                                {
                                    break;
                                }
                                //Check if pawn can do double move
                                if (y == 1 || y == 6)
                                {
                                    if (pawn.canDoubleMove && move.CanDoubleMove && j == 1)
                                    {
                                        currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                                        move.CanDoubleMove = false;
                                        break;
                                    }

                                }
                                
                                //If the pawn cannot do a double move then that means the pawn cannot move any further. If j == 1 then that
                                //means the pawn is on its last possible move, if no double move is possible, then no more legal moves forward are possible.
                                else if (j == 1)
                                {
                                    break;
                                }
                                currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                                move.YOffset += move.NY;
                                move.ActiveDirectionY += move.NY;
                            }
                            else
                            {
                                //If the next position for a diagonal move is a piece of the opposite color, add it to the list of legalmoves.
                                if (currentPiece.isBlack != board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                                {
                                    currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                                }
                                //Should a coordinate be an en passant square then add that coordinate
                                else if (enPassantCoordinate[0] == (move.TempY + move.YOffset) && enPassantCoordinate[1] == (move.TempX + move.XOffset))
                                {
                                    currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                                }

                                //set new values specific to diagonal moves
                                move.MaxValueX = move.NewMaxValueX;
                                operationX = newOppositeOperationX;
                                move.XOffset *= move.NX;
                                move.DirValueX *= move.NX;
                            }

                        }
                    }
                    else
                    {
                        //Normally the loop "breaks" when the index is out of bounds, however for diagonal
                        //moves it is neccessary to check for the additional attacking square pawns can move to
                        if (isDiagonalMove)
                        {
                            move.MaxValueX = move.NewMaxValueX;
                            operationX = newOppositeOperationX;
                            move.XOffset *= move.NX;
                            move.DirValueX *= move.NX;
                            continue;
                        }

                        move.TempY = y;
                        move.TempX = x;
                        break;
                    }
                }
                isDiagonalMove = true;
                if (currentPiece.isBlack)
                {
                    //set new values for black pawns
                    move.YOffset = 1;
                    move.XOffset = -1;
                    move.MaxValueX = -1;
                    move.NewMaxValueX = 8;
                    move.DirValueX = -1;
                    move.NY = 1;
                    move.NX = -1;
                    move.ActiveDirectionY = y;
                    move.ActiveDirectionX = x;
                    operationX = newOperationX;
                }
                else
                {
                    //set new values for white pawns
                    move.YOffset = -1;
                    move.XOffset = -1;
                    move.MaxValueX = -1;
                    move.NewMaxValueX = 8;
                    move.DirValueX = -1;
                    move.NY = 1;
                    move.NX = -1;
                    move.ActiveDirectionY = y;
                    move.ActiveDirectionX = x;
                    operationX = newOperationX;
                }
            }
            return currentPiece; // check so it is working, was "pawn" before.
        }

        private Piece GenerateKingMoves(Piece currentPiece, int y, int x)
        {
            List<string> kingMoves = GenerateLateralKingMoves(currentPiece, y, x);
            kingMoves.AddRange(GenerateDiagonalKingMoves(currentPiece, y, x));
            kingMoves.AddRange(GetKingCastleCoordinates(currentPiece, y, x));
            kingMoves = kingMoves.Distinct().ToList(); //due to a "bug" with "AddRange", duplicate values were assigned, creating a need for distinct values


            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!board[i, j].typeOfPiece.Equals("King"))
                    {
                        if (!(board[i, j] is EmptySquare))
                        {
                            var selectedPiece = board[i, j];
                            if (currentPiece.isBlack != board[i, j].isBlack)
                            {
                                kingMoves = RemoveIllegalSquaresForKing(kingMoves, selectedPiece);
                                
                            }
                        }
                    }
                }
            }



            currentPiece.legalMoves = kingMoves;



            return currentPiece;
        }

        private List<string> GetKingCastleCoordinates(Piece currentPiece, int y, int x)
        {
            King king = currentPiece as King;
            List<string> castleCoordinates = new List<string>(2);
            if (king.isBlack)
            {
                if (y == 0 && x == 4)
                {
                    if (board[0, x + 1] is EmptySquare && board[0, x + 2] is EmptySquare)
                    {
                        if (king.canCastleK)
                        {
                            castleCoordinates.Add("0 6");
                        }
                    }

                    if (board[0, x - 1] is EmptySquare && board[0, x - 2] is EmptySquare)
                    {
                        if (king.canCastleQ)
                        {
                            castleCoordinates.Add("0 2");
                        }
                    }
                    
                }
            }
            else
            {
                if (y == 7 && x == 4)
                {
                    if (board[7, x + 1] is EmptySquare && board[7, x + 2] is EmptySquare)
                    {
                        if (king.canCastleK)
                        {
                            castleCoordinates.Add("7 6");
                        }
                    }

                    if (board[7, x - 1] is EmptySquare && board[7, x + 2] is EmptySquare)
                    {
                        if (king.canCastleQ)
                        {
                            castleCoordinates.Add("7 2");
                        }
                    }
                }
            }

            return castleCoordinates;
        }

        private List<string> RemoveIllegalSquaresForKing(List<string> kingMoves, Piece selectedPiece)
        {
            List<string> coordinatesToBeDeleted = new List<string>(kingMoves.Count);
            for (int i = 0; i < kingMoves.Count; i++)
            {
                string kingmove = kingMoves[i];
                for (int j = 0; j < selectedPiece.legalMoves.Count; j++)
                {
                    if (kingmove.Equals(selectedPiece.legalMoves[j]))
                    {
                        coordinatesToBeDeleted.Add(kingmove);
                        break;
                    }
                }
            }

            for (int i = 0; i < coordinatesToBeDeleted.Count; i++)
            {
                kingMoves.Remove(coordinatesToBeDeleted[i]);
            }
            
            return kingMoves;
        }

        private List<string> GenerateLateralKingMoves(Piece currentPiece, int y, int x)
        {
            List<string> legalMoves = new List<string>();
            legalMoves.AddRange(GenerateLateralMoves(currentPiece, y, x, 1).legalMoves);

            return legalMoves;
        }

        private List<string> GenerateDiagonalKingMoves(Piece currentPiece, int y, int x)
        {
            List<string> legalMoves = new List<string>();
            legalMoves.AddRange(GenerateDiagonalMoves(currentPiece, y, x, 1).legalMoves);

            return legalMoves;
        }
    }
}