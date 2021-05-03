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

        public Piece[,] GeneratePseudoLegalMoves(Piece[,] board, bool isBlackToMove)
        {
            this.board = board;
            this.isBlackToMove = isBlackToMove;

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
                            break;
                        case "Queen":
                            break;
                        default:
                            break;
                    }
                }
            }

            return board;
        }

        private void GenerateQueenMoves(Piece currentPiece)
        {

        }

        //todo, document this monster of a method.
        private Piece GenerateRookMoves(Piece currentPiece, int y, int x)
        {
            //values are set up to start checking the northern direction first, it then follows the switch statement in order for each direction
            int tempY = y;
            int tempX = x;
            int yOffset = -1;
            int xOffset = 0;
            int activeDirection = tempY;
            bool isPositiveOperation = false;
            int maxValue = 0;
            int dirValue = -1;
            int n = -1;


            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 1: //East
                        yOffset = 0;
                        xOffset = 1;
                        activeDirection = x;
                        dirValue = 1;
                        maxValue = 8;
                        n = 1;
                        isPositiveOperation = true;
                        break;
                    case 2: //South
                        yOffset = 1;
                        xOffset = 0;
                        activeDirection = y;
                        isPositiveOperation = true;
                        break;
                    case 3: //West
                        yOffset = 0;
                        xOffset = -1;
                        activeDirection = x;
                        dirValue = -1;
                        maxValue = 0;
                        n = -1;
                        isPositiveOperation = false;
                        break;
                    default:
                        break;
                }
                for (int j = 0; j < 7; j++)
                {
                    if (!isPositiveOperation)
                    {
                        if (activeDirection + dirValue >= maxValue)
                        {
                            //Checks whether or not the next square is a piece of the same color and if its a piece on the square
                            if (currentPiece.isBlack == board[tempY + yOffset, tempX + xOffset].isBlack && !(board[tempY + yOffset, tempX + xOffset] is EmptySquare))
                            {
                                break;
                            }
                            //If not, then add that move to legal moves
                            else
                            {
                                currentPiece.legalMoves.Add((tempY + yOffset) + " " + (tempX + xOffset));
                                activeDirection += n;

                                //If the next piece is of the opposite color, then stop searching for legal moves
                                if (currentPiece.isBlack != board[tempY + yOffset, tempX + xOffset].isBlack && !(board[tempY + yOffset, tempX + xOffset] is EmptySquare))
                                {
                                    break;
                                }

                                if (yOffset == 0)
                                {
                                    xOffset += n;
                                }
                                else if (xOffset == 0)
                                {
                                    yOffset += n;
                                }
                            }
                        }
                        else
                        {
                            tempY = y;
                            tempX = x;
                            isPositiveOperation = true;
                            break;
                        }
                    }
                    else
                    {
                        if (activeDirection + dirValue < maxValue && (xOffset >= 0 && yOffset >= 0))
                        {
                            if (currentPiece.isBlack == board[tempY + yOffset, tempX + xOffset].isBlack && !(board[tempY + yOffset, tempX + xOffset] is EmptySquare)) //add error handling for negative integers
                            {
                                break;
                            }
                            else
                            {
                                currentPiece.legalMoves.Add((tempY + yOffset) + " " + (tempX + xOffset));
                                activeDirection += n;

                                if (currentPiece.isBlack != board[tempY + yOffset, tempX + xOffset].isBlack && !(board[tempY + yOffset, tempX + xOffset] is EmptySquare))
                                {
                                    break;
                                }

                                if (yOffset == 0)
                                {
                                    xOffset += n;
                                }
                                else if (xOffset == 0)
                                {
                                    yOffset += n;
                                }
                            }
                        }
                        else
                        {
                            tempY = y;
                            tempX = x;
                            isPositiveOperation = false;
                            break;
                        }
                    }
                }
            }
            return currentPiece;
        }

        private void GenerateBishopMoves(Piece currentPiece)
        {

        }

        private void GenerateKingMoves(Piece currentPiece)
        {

        }

        private void GeneratePawnMoves(Piece currentPiece)
        {

        }

        private void GenerateKnightMoves(Piece currentPiece)
        {

        }
    }
}
