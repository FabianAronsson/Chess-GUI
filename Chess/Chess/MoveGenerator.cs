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
        private bool validateKingMoves = false;

        public Piece[,] GeneratePseudoLegalMoves(Piece[,] board, List<int> enPassantSquare) //board is binded to Internalboard.
        {
            this.board = (Piece[,])board.Clone();
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

            if (!validateKingMoves)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        var currentPiece = board[i, j];
                        if (board[i, j].typeOfPiece.Equals("King"))
                        {
                            board[i, j] = GenerateKingMoves(currentPiece, i, j);
                        }
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

        //document
        private Piece GenerateKingMoves(Piece currentPiece, int y, int x)
        {
            List<string> kingMoves = GenerateLateralKingMoves(currentPiece, y, x);
            kingMoves.AddRange(GenerateDiagonalKingMoves(currentPiece, y, x));
            kingMoves.AddRange(GetKingCastleCoordinates(currentPiece, y, x));

            //due to a "bug" with "AddRange", duplicate values were assigned, creating a need for distinct values
            kingMoves = kingMoves.Distinct().ToList();


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
            currentPiece.legalMoves = RemoveIllegalKingCaptureSquares(kingMoves, currentPiece);
            return currentPiece;
        }


        //removes capture squares that is protected by another square, document more
        private List<string> RemoveIllegalKingCaptureSquares(List<string> kingMoves, Piece king)
        {
            string[] stringKingMoves = new string[2];
            var tempBoard = (Piece[,])board.Clone();
            int kingY;
            int kingX;
            List<string> illegalCoordinates = new List<string>();
            PieceFactory.PieceFactory factory = new PieceFactory.PieceFactory();

            for (int i = 0; i < kingMoves.Count; i++)
            {
                stringKingMoves = kingMoves[i].Split();
                kingY = int.Parse(stringKingMoves[0]);
                kingX = int.Parse(stringKingMoves[1]);

                if (!(board[kingY, kingX] is EmptySquare))
                {
                    if (king.isBlack != tempBoard[kingY, kingX].isBlack)
                    {
                        validateKingMoves = true;
                        var tempPiece = tempBoard[kingY, kingX];
                        tempBoard[kingY, kingX] = factory.CreatePiece('S', true) ;
                        var newBoard = GeneratePseudoLegalMoves(tempBoard, new List<int> { 9, 9 });
                        validateKingMoves = false;
                        if (IsCaptureSquareProtected(newBoard, kingMoves[i]))
                        {
                            illegalCoordinates.Add(kingMoves[i]);
                            
                        }
                        tempBoard[kingY, kingX] = tempPiece;
                    }
                }

            }
            for (int i = 0; i < illegalCoordinates.Count; i++)
            {
                kingMoves.Remove(illegalCoordinates[i]);
            }

            validateKingMoves = false;

            return kingMoves;
        }

        //Checks if a square the king is attacking is protected or not.
        private bool IsCaptureSquareProtected(Piece[,] board, string currentMove)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!(board[i, j] is EmptySquare))
                    {
                        if (board[i, j].legalMoves.Contains(currentMove))
                        {
                            return true;
                        }
                    }
                    
                }
            }
            return false;
        }


        //document, takes advantage of the fact that kings can ONLY castle in a set position in normal chess,
        //for variants such as Chess960, other more elaborate methods need to be applied due to the 
        //bizarre situations that can occur
        private List<string> GetKingCastleCoordinates(Piece currentPiece, int y, int x)
        {
            King king = currentPiece as King;
            List<string> castleCoordinates = new List<string>(2);
            if (king.isBlack)
            {
                if (y == 0 && x == 4)
                {
                    if (board[0, 7] is Rook && board[0, 7].isBlack)
                    {
                        if (board[0, x + 1] is EmptySquare && board[0, x + 2] is EmptySquare)
                        {
                            if (king.canCastleK)
                            {
                                castleCoordinates.Add("0 6");
                            }
                        }
                    }

                    if (board[0, 0] is Rook && board[0, 0].isBlack)
                    {
                        if (board[0, x - 1] is EmptySquare && board[0, x - 2] is EmptySquare)
                        {
                            if (king.canCastleQ)
                            {
                                castleCoordinates.Add("0 2");
                            }
                        }
                    }
                }
            }
            else
            {
                if (y == 7 && x == 4)
                {
                    if (board[7, 7] is Rook && !board[7, 7].isBlack)
                    {
                        if (board[7, x + 1] is EmptySquare && board[7, x + 2] is EmptySquare)
                        {
                            if (king.canCastleK)
                            {
                                castleCoordinates.Add("7 6");
                            }
                        }

                        if (board[7, 0] is Rook && !board[7, 0].isBlack)
                        {
                            if (board[7, x - 1] is EmptySquare && board[7, x + 2] is EmptySquare)
                            {
                                if (king.canCastleQ)
                                {
                                    castleCoordinates.Add("7 2");
                                }
                            }
                        }
                    }
                }
            }

            return castleCoordinates;
        }

        //document
        private List<string> RemoveIllegalSquaresForKing(List<string> kingMoves, Piece selectedPiece)
        {
            List<string> coordinatesToBeDeleted = new List<string>(kingMoves.Count);
            for (int i = 0; i < kingMoves.Count; i++)
            {
                string kingmove = kingMoves[i];
                for (int j = 0; j < selectedPiece.legalMoves.Count; j++)
                {
                    //if any of the kings pseudo-legal squares is equal to ANY of the selectedPiece's 
                    //legal squares, then that means the pseudo-legal move is illegal.
                    if (kingmove.Equals(selectedPiece.legalMoves[j]))
                    {
                        coordinatesToBeDeleted.Add(kingmove);

                        //If any of the kings first x-axis moves comes through in the previous 
                        //if statement then it must mean that the next move also is an illegal 
                        //square since a king cannot castle through a check.
                        if (kingmove.Equals(0 + " " + 5) || kingmove.Equals(0 + " " + 3) || kingmove.Equals(7 + " " + 5) || kingmove.Equals(7 + " " + 3))
                        {
                            coordinatesToBeDeleted.Add(CheckCastlingSquares(kingmove));
                        }

                        break;
                    }
                }
            }

            //delete previous pseudo-legal moves for the current king
            for (int i = 0; i < coordinatesToBeDeleted.Count; i++)
            {
                kingMoves.Remove(coordinatesToBeDeleted[i]);
            }

            return kingMoves;
        }

        public string CheckCastlingSquares(string castlingSquare)
        {
            //positive x-axis for black king
            if (castlingSquare.Equals(0 + " " + 5))
            {
                return 0 + " " + 6;
            }
            //negative x-axis for black king
            else if (castlingSquare.Equals(0 + " " + 3))
            {
                return 0 + " " + 2;
            }
            //positive x-axis for white king
            else if (castlingSquare.Equals(7 + " " + 5))
            {
                return 7 + " " + 6;
            }
            //no other move but the white kings final castle move 
            //can be the last one, which means the negative x-axis for the white king
            else
            {
                return 7 + " " + 2;
            }
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