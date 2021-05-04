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
        Piece[,] board;
        bool isBlackToMove;

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
                            board[i, j] = GenerateRookMoves(currentPiece, i, j);
                            break;
                        case "Bishop":
                            board[i, j] = GenerateBishopMoves(currentPiece, i, j);
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
            legalMoves.AddRange(GenerateRookMoves(currentPiece, y, x).legalMoves);
            legalMoves.AddRange(GenerateBishopMoves(currentPiece, y, x).legalMoves);

            currentPiece.legalMoves = legalMoves;
            return currentPiece;
        }

        //todo, document this monster of a method.
        private Piece GenerateRookMoves(Piece currentPiece, int y, int x)
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
                for (int j = 0; j < 7; j++)
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
        private Piece GenerateBishopMoves(Piece currentPiece, int y, int x)
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

                for (int j = 0; j < 8; j++)
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

        private Piece GeneratePawnMoves(Piece currentPiece, int y, int x, List<int> enPassantCoordinate)
        {
            Pawn pawn = currentPiece as Pawn;
            var move = new MoveModel(pawn, y, x, true);

            
            bool isDiagonalMove = false;

            //delegates for choosing what operation to use
            Func<int, int, bool> greaterThanDelegate = (a, b) => a > b;
            Func<int, int, bool> lessThanDelegate = (a, b) => a < b;
            var operationY = lessThanDelegate;
            var operationX = lessThanDelegate;

            var newOperationX = greaterThanDelegate;
            var newOppositeOperationX = lessThanDelegate;

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
                            if (!isDiagonalMove)
                            {
                                //If the next position is a piece, then no more moves can be generated for that direction.
                                if (currentPiece.isBlack != board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                                {
                                    break;
                                }
                                if (pawn.canDoubleMove && move.CanDoubleMove && j == 1)
                                {
                                    currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                                    move.CanDoubleMove = false;
                                    break;
                                }
                                else if (j == 1) 
                                {
                                    break;
                                }
                                currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                            }
                            

                            //If the next position for a diagonal move is a piece, then capture that piece.
                            if (isDiagonalMove)
                            {
                                if (currentPiece.isBlack != board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                                {
                                    currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                                }
                                //Should a coordinate be an en passant square then add that coordinate
                                else if (enPassantCoordinate[0] == (move.TempY + move.YOffset) && enPassantCoordinate[1] == (move.TempX + move.XOffset))
                                {
                                    currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                                }
                                move.MaxValueX = move.NewMaxValueX;
                                if (j == 1)
                                {
                                    operationX = newOperationX;
                                }
                                move.XOffset *= move.NX;
                            }

                            move.ActiveDirectionY += move.NY;

                            move.YOffset += move.NY;
                        }
                    }
                    else
                    {
                        move.TempY = y;
                        move.TempX = x;
                        break;
                    }
                }
                isDiagonalMove = true;
                if (currentPiece.isBlack)
                {
                    move.YOffset = 1;
                    move.XOffset = 1;
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
                    move.YOffset = -1;
                    move.XOffset = -1;
                    move.MaxValueX = -1;
                    move.NewMaxValueX = 8;
                    move.DirValueX = -1;
                    move.NY = 1;
                    move.NX = 1;
                    move.ActiveDirectionY = y;
                    move.ActiveDirectionX = x;
                    operationX = newOperationX;
                }
            }
            return pawn;
        }

        private void GenerateKingMoves(Piece currentPiece)
        {

        }
    }
}
