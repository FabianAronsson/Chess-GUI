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

        /// <summary>
        /// Generates legal moves for every piece in. King moves are generated separetely 
        /// because they require additional validation.
        /// </summary>
        /// <param name="board"> The current board.</param>
        /// <param name="enPassantSquare">The square en passant is valid on.</param>
        /// <returns></returns>
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

            //The second validation for king moves only occur when this boolean is set to true.
            //When it is, a simulated move is played out on a simulated board to test if the move is legal or not.
            if (!validateKingMoves)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        var currentPiece = board[i, j];
                        if (board[i, j].typeOfPiece.Equals("King"))
                        {
                            board[i, j] = GenerateKingMoves(currentPiece as King, i, j);
                        }
                    }
                }
            }

            board = (Piece[,])RemoveOppositeKingAttackingSquares(board).Clone();
            return board; //not actually neccessary
        }

        private void ResetPreviousLegalMoves(Piece[,] board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j].legalMoves.Clear();

                    if (board[i, j] is Pawn pawn)
                    {
                        pawn.attckingSquares.Clear();
                    }
                    else if (board[i, j] is Rook rook)
                    {
                        rook.attckingSquares.Clear();
                    }
                    else if (board[i, j] is Bishop bishop)
                    {
                        bishop.attckingSquares.Clear();
                    }
                    else if (board[i, j] is Queen queen)
                    {
                        queen.attckingSquares.Clear();
                    }
                    else if (board[i, j] is King king)
                    {
                        king.attckingSquares.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Generates queen moves. Since a queen is just a rook and a bishop combined, their respective methods can be used for the queen.
        /// </summary>
        /// <param name="currentPiece">The queen</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="x">X-coordinate</param>
        /// <returns></returns>
        private Piece GenerateQueenMoves(Piece currentPiece, int y, int x)
        {
            List<string> legalMoves = new List<string>();

            //since a queen is just a bishop and a rook combinded, their respective methods can be used to generate pseudo-legal moves
            Piece lateralPiece = GenerateLateralMoves(currentPiece, y, x, 7);
            Piece diagonalPiece = GenerateDiagonalMoves(currentPiece, y, x, 7);

            legalMoves.AddRange(lateralPiece.legalMoves);
            legalMoves.AddRange(diagonalPiece.legalMoves);
            currentPiece.legalMoves = legalMoves;

            return currentPiece;
        }

        /// <summary>
        /// Generates lateral piece moves. This for example for queens, rooks and kings.
        /// </summary>
        /// <param name="currentPiece"></param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="maxLoopValue">The maximum value the piece can move to.</param>
        /// <returns></returns>
        private Piece GenerateLateralMoves(Piece currentPiece, int y, int x, int maxLoopValue)
        {
            //It says for Rooks, but this is merely to signal the constructor of the move model to assign lateral move values.
            Rook startPiece = currentPiece as Rook;
            var move = new MoveModel(startPiece, y, x);

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
                        move.XrayY = 0;
                        move.XrayX = 1;
                        break;
                    case 2: //South
                        move.YOffset = 1;
                        move.XOffset = 0;
                        move.ActiveDirection = y;
                        move.IsPositiveOperation = true;
                        move.XrayY = 1;
                        move.XrayX = 0;
                        break;
                    case 3: //West
                        move.YOffset = 0;
                        move.XOffset = -1;
                        move.ActiveDirection = x;
                        move.DirValue = -1;
                        move.MaxValue = -1;
                        move.N = -1;
                        move.IsPositiveOperation = false;
                        move.XrayY = 0;
                        move.XrayX = -1;
                        break;
                    default:
                        break;
                }
                for (int j = 0; j < maxLoopValue; j++)
                {
                    if (!move.IsPositiveOperation)
                    {
                        //These direction are for negative values.
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
                                    //However, if the piece is a ray piece, then they also attack beyond the next piece, with regards to kings.
                                    //This means that the piece is still attacking even if it cannot specifically move there.
                                    //The attacking square values are only used are only used when validating king moves.
                                    if (currentPiece is Rook rook)
                                    {
                                        if (board[move.TempY + move.YOffset, move.TempX + move.XOffset] is King)
                                        {
                                            rook.attckingSquares.Add((move.TempY + move.YOffset + move.XrayY) + " " + (move.TempX + move.XOffset + move.XrayX));
                                        }
                                    }
                                    else if (currentPiece is Queen queen)
                                    {
                                        if (board[move.TempY + move.YOffset, move.TempX + move.XOffset] is King)
                                        {
                                            queen.attckingSquares.Add((move.TempY + move.YOffset + move.XrayY) + " " + (move.TempX + move.XOffset + move.XrayX));
                                        }
                                    }

                                    break;
                                }

                                //Increment the offset depending on the current direction.
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
                        //The logic below is the same as above, with the difference being in what direction it is looking.
                        //These direction are for positive values.
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
                                    if (currentPiece is Rook rook)
                                    {
                                        if (board[move.TempY + move.YOffset, move.TempX + move.XOffset] is King)
                                        {
                                            rook.attckingSquares.Add((move.TempY + move.YOffset + move.XrayY) + " " + (move.TempX + move.XOffset + move.XrayX));
                                        }

                                    }
                                    else if (currentPiece is Queen queen)
                                    {
                                        if (board[move.TempY + move.YOffset, move.TempX + move.XOffset] is King)
                                        {
                                            queen.attckingSquares.Add((move.TempY + move.YOffset + move.XrayY) + " " + (move.TempX + move.XOffset + move.XrayX));
                                        }
                                    }

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

        /// <summary>
        /// Generates diagonal moves. This is for example queens, bishops and kings.
        /// 
        /// Delegate code comes from: https://stackoverflow.com/questions/25676074/is-it-possible-to-store-a-math-operation-in-a-variable-and-call-on-that-vari 
        /// </summary>
        /// <param name="currentPiece">The piece to generate legal moves for</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="maxLoopValue"> The maximum value the piece can move to</param>
        /// <returns>The maximum value the piece can move to.</returns>
        private Piece GenerateDiagonalMoves(Piece currentPiece, int y, int x, int maxLoopValue)
        {
            //It says for Bishops, but this is merely to signal the constructor of the move model to assign diagonal move values.
            Bishop startPiece = currentPiece as Bishop;
            var move = new MoveModel(startPiece, y, x);

            //Delegates for choosing what operation to use
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
                        move.XrayY = move.YOffset;
                        move.XrayX = move.XOffset;
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
                        move.XrayY = move.YOffset;
                        move.XrayX = move.XOffset;
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
                        move.XrayY = move.YOffset;
                        move.XrayX = move.XOffset;
                        operationY = lessThanDelegate;
                        operationX = greaterThanDelegate;
                        break;
                    default:
                        break;
                }

                for (int j = 0; j < maxLoopValue; j++)
                {
                    //Checks so that the current position is not out of bounds.
                    if (operationY(move.ActiveDirectionY + move.DirValueY, move.MaxValueY) && operationX(move.ActiveDirectionX + move.DirValueX, move.MaxValueX))
                    {
                        //Checks whether or not the next square is a piece of the same color and if its a piece on the square.
                        if (currentPiece.isBlack == board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                        {
                            break;
                        }
                        else
                        {
                            //If not then add that position to the piece.
                            currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                            move.ActiveDirectionY += move.NY;
                            move.ActiveDirectionX += move.NX;

                            //If the next position is a piece, then no more moves can be generated for that direction.
                            if (currentPiece.isBlack != board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                            {
                                //However, if the piece is a ray piece, then they also attack beyond the next piece, with regards to kings.
                                //This means that the piece is still attacking even if it cannot specifically move there.
                                //The attacking square values are only used are only used when validating king moves.
                                if (currentPiece is Bishop bishop)
                                {
                                    if (board[move.TempY + move.YOffset, move.TempX + move.XOffset] is King)
                                    {
                                        bishop.attckingSquares.Add((move.TempY + move.YOffset + move.XrayY) + " " + (move.TempX + move.XOffset + move.XrayX));
                                    }
                                }
                                if (currentPiece is Queen queen)
                                {
                                    if (board[move.TempY + move.YOffset, move.TempX + move.XOffset] is King)
                                    {
                                        queen.attckingSquares.Add((move.TempY + move.YOffset + move.XrayY) + " " + (move.TempX + move.XOffset + move.XrayX));
                                    }
                                }
                                break;
                            }
                            //Increment the offset by a constant in a specific direction that depends on which direction the method is currently searching.
                            move.XOffset += move.NX;
                            move.YOffset += move.NY;
                        }
                    }
                }
            }
            return currentPiece;
        }

        /// <summary>
        /// Generates knight moves.
        /// 
        /// Delegate code comes from: https://stackoverflow.com/questions/25676074/is-it-possible-to-store-a-math-operation-in-a-variable-and-call-on-that-vari 
        /// </summary>
        /// <param name="currentPiece"></param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="x">X-coordinate</param>
        /// <returns></returns>
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
                    //Each number represents a specific square where the knight can move to.
                    //A knight can only move to very specific squares. Which means each direction can easily be set for each move.
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


                for (int j = 0; j < 2; j++)
                {
                    //First it checks if the current search is not outside the legal bounds
                    if ((operationY(move.TempY + move.YOffset, move.MaxValueY) && operationX(move.TempX + move.XOffset, move.MaxValueX)) && (move.TempY + move.YOffset > -1 && move.TempX + move.XOffset > -1))
                    {
                        //It then checks so that the current search is not of the same piece color and also if it is a piece.
                        if (!(currentPiece.isBlack == board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare)))
                        {
                            currentPiece.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                        }
                    }

                    //Then mirror the values to the opposite position for a knight
                    move.YOffset *= move.NY; //Multiplied with -1 to change the direction to the opposite side.
                    move.XOffset *= move.NX; //- || -
                    move.MaxValueY = move.NewMaxValueY;
                    move.MaxValueX = move.NewMaxValueX;
                    operationY = newOperationY;
                    operationX = newOperationX;

                }
            }
            return currentPiece;
        }

        /// <summary>
        /// Generates legal moves for pawns.
        /// 
        /// Delegate code comes from: https://stackoverflow.com/questions/25676074/is-it-possible-to-store-a-math-operation-in-a-variable-and-call-on-that-vari 
        /// </summary>
        /// <param name="currentPiece"></param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="x">X-coordinate</param>
        /// <param name="enPassantCoordinate"></param>
        /// <returns></returns>
        private Piece GeneratePawnMoves(Piece currentPiece, int y, int x, List<int> enPassantCoordinate)
        {
            Pawn pawn = currentPiece as Pawn;
            //Assuming it is a black pawn those values are set, otherwise it is set to white on line 555.
            var move = new MoveModel(pawn, y, x, true);

            bool isDiagonalMove = false;

            //Delegates for choosing what operation to use
            Func<int, int, bool> greaterThanDelegate = (a, b) => a > b;
            Func<int, int, bool> lessThanDelegate = (a, b) => a < b;
            var operationY = lessThanDelegate;
            var operationX = lessThanDelegate;

            var newOperationX = greaterThanDelegate;
            var newOppositeOperationX = lessThanDelegate;

            //Set position specific values for white
            if (!pawn.isBlack)
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
                        if (pawn.isBlack == board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
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
                                if (pawn.isBlack != board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                                {
                                    break;
                                }
                                //Check if pawn can do double move on the second and seventh rank.
                                if (y == 1 || y == 6)
                                {
                                    if (pawn.canDoubleMove && move.CanDoubleMove && j == 1)
                                    {
                                        pawn.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
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
                                pawn.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                                move.YOffset += move.NY;
                                move.ActiveDirectionY += move.NY;
                            }
                            else
                            {
                                //If the next position for a diagonal move is a piece of the opposite color, add it to the list of legalmoves.
                                if (pawn.isBlack != board[move.TempY + move.YOffset, move.TempX + move.XOffset].isBlack && !(board[move.TempY + move.YOffset, move.TempX + move.XOffset] is EmptySquare))
                                {
                                    pawn.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                                }
                                //Should a coordinate be an en passant square then add that coordinate
                                else if (enPassantCoordinate[0] == (move.TempY + move.YOffset) && enPassantCoordinate[1] == (move.TempX + move.XOffset))
                                {
                                    if (pawn.isBlack != board[enPassantCoordinate[0] + 1, enPassantCoordinate[1]].isBlack)
                                    {
                                        pawn.legalMoves.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
                                    }

                                }
                                //If no normally legal moves are found for the pawn, then the attacking positions for a pawn is saved for king move validation.
                                else
                                {
                                    pawn.attckingSquares.Add((move.TempY + move.YOffset) + " " + (move.TempX + move.XOffset));
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
                if (pawn.isBlack)
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
            return pawn;
        }

        /// <summary>
        /// Generates king moves.
        /// </summary>
        /// <param name="currentPiece"></param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="x">X-coordinate</param>
        /// <returns></returns>
        private Piece GenerateKingMoves(King currentPiece, int y, int x)
        {
            //A king is technically a rook and a bishop, just restricted based on how many squares it can move to.
            List<string> kingMoves = GenerateLateralKingMoves(currentPiece, y, x);
            kingMoves.AddRange(GenerateDiagonalKingMoves(currentPiece, y, x));
            kingMoves.AddRange(GetKingCastleCoordinates(currentPiece, y, x));

            kingMoves.Distinct().ToList();
            List<string> attackingSquares = new List<string>();
            attackingSquares.AddRange(kingMoves);

            //Removes duplicate values from attacking squares.
            for (int i = 0; i < attackingSquares.Count; i++)
            {
                for (int j = 0; j < attackingSquares.Count - 1; j++)
                {
                    if (attackingSquares[0].Equals(attackingSquares[j + 1]))
                    {
                        attackingSquares.Remove(attackingSquares[0]);

                        break;
                    }
                }
            }

            currentPiece.attckingSquares = new List<string>(attackingSquares);

            //due to a "bug" with "AddRange", duplicate values were assigned, creating a need for distinct values
            kingMoves = kingMoves.Distinct().ToList();

            //Remove illegal squares for the current king.
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!(board[i, j] is EmptySquare))
                    {
                        var selectedPiece = board[i, j];

                        //If the king's color is not the same as the the selectedpiece's color.
                        if (currentPiece.isBlack != board[i, j].isBlack)
                        {
                            kingMoves = RemoveIllegalSquaresForKing(kingMoves, selectedPiece);
                        }
                    }
                }
            }
            currentPiece.legalMoves = RemoveIllegalKingCaptureSquares(kingMoves, currentPiece);

            return currentPiece;
        }


        /// <summary>
        /// Removes capture squares for the king that is protected by another square.
        /// </summary>
        /// <param name="kingMoves"></param>
        /// <param name="king"></param>
        /// <returns></returns>
        private List<string> RemoveIllegalKingCaptureSquares(List<string> kingMoves, Piece king)
        {
            var tempBoard = (Piece[,])board.Clone();
            int kingY;
            int kingX;

            List<string> illegalCoordinates = new List<string>();
            PieceFactory.PieceFactory factory = new PieceFactory.PieceFactory();

            for (int i = 0; i < kingMoves.Count; i++)
            {
                //Gets the current move that should be tested
                string[] stringKingMoves = kingMoves[i].Split();

                //Convert the coordinates to ints
                kingY = int.Parse(stringKingMoves[0]);
                kingX = int.Parse(stringKingMoves[1]);

                //If one of the king's pseudo-legal moves is a piece, then...
                if (!(board[kingY, kingX] is EmptySquare))
                {
                    //Check if the move is a piece of the opposite color
                    if (king.isBlack != tempBoard[kingY, kingX].isBlack)
                    {
                        //Initiate the validation of kings by setting a boolean to true.
                        //Afterwards the "new" position that would occur if the king moved
                        //to the denoted position, gets tested using slightly altered
                        //move parameters to see if any piece of the opposite color
                        //attacks the square the king "would have" moved to if it was a legal position.
                        //Should any position coincide with each other, then remove that coordinate from
                        //the king's legal moves.
                        validateKingMoves = true;
                        var tempPiece = tempBoard[kingY, kingX];
                        tempBoard[kingY, kingX] = factory.CreatePiece('S', true);
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

            //Removes any illegal squares that the previous method returned.
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
                        else if (board[i, j] is King king)
                        {
                            if (king.attckingSquares.Contains(currentMove))
                            {
                                return true;
                            }
                        }
                        else if (board[i, j] is Pawn pawn)
                        {
                            if (pawn.attckingSquares.Contains(currentMove))
                            {
                                return true;
                            }
                        }
                    }

                }
            }
            return false;
        }

        /// <summary>
        /// Takes advantage of the fact that kings can ONLY castle in a set position in normal chess,
        ///  for variants such as Chess960, other more elaborate methods need to be applied due to the 
        ///  bizarre situations that can occur 
        /// 
        /// All the values are for standard castle positions in chess.
        /// 
        /// </summary>
        /// <param name="currentPiece"></param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>

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

        /// <summary>
        /// Removes any squares that the opposite king attacks.
        /// </summary>
        /// <param name="chessBoard"></param>
        /// <returns></returns>
        private Piece[,] RemoveOppositeKingAttackingSquares(Piece[,] chessBoard)
        {
            King king = null;
            King oppositeKing = null;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    //Get both king coordinates
                    if (chessBoard[i, j] is King && !chessBoard[i, j].isBlack)
                    {
                        king = (King)chessBoard[i, j];
                    }
                    else if (chessBoard[i, j] is King && chessBoard[i, j].isBlack)
                    {
                        oppositeKing = (King)chessBoard[i, j];
                    }
                }

            }

            if (!(king == null || oppositeKing == null)) //due to a bug, the kings sometimes does not appear on the board, this also means that some illegal moves are legal
            {
                King currentKing = king;
                bool isBothKingsValidated = false;

                for (int i = 0; i < currentKing.attckingSquares.Count; i++)
                {
                    for (int j = 0; j < oppositeKing.attckingSquares.Count; j++)
                    {
                        //If any squares of the current king that the opposite king attacks equals one another, then, remove that coordinate.
                        if (currentKing.attckingSquares.Contains(oppositeKing.attckingSquares[j]))
                        {
                            currentKing.legalMoves.Remove(oppositeKing.attckingSquares[j]);
                        }
                    }

                    //Swap the values for both kings, so that the opposite colored king is tested instead.
                    if (i == currentKing.legalMoves.Count && !isBothKingsValidated)
                    {
                        currentKing = oppositeKing;
                        currentKing.attckingSquares.Distinct().ToList();
                        oppositeKing = king;
                        i = 0;
                        isBothKingsValidated = true;
                    }
                }
            }

            return chessBoard;
        }

        /// <summary>
        /// Removes any squares that other pieces attacks.
        /// </summary>
        /// <param name="kingMoves"></param>
        /// <param name="selectedPiece"></param>
        /// <returns></returns>
        private List<string> RemoveIllegalSquaresForKing(List<string> kingMoves, Piece selectedPiece)
        {
            List<string> coordinatesToBeDeleted = new List<string>(kingMoves.Count);

            //Since a pawn only can attack diagonally, it's special case need to be
            //addressed first as a pawns legal move is not the move which it can atttack with.
            if (selectedPiece is Pawn pawn)
            {
                for (int i = 0; i < pawn.attckingSquares.Count; i++)
                {
                    if (kingMoves.Contains(pawn.attckingSquares[i]))
                    {
                        coordinatesToBeDeleted.Add(pawn.attckingSquares[i]);

                    }
                }
            }
            else
            {
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
            }

            //Removes squares that ray pieces attacks. This also includes squares that are on the other side of the king if the ray piece attacks that square.
            if (selectedPiece is Rook || selectedPiece is Bishop || selectedPiece is Queen)
            {
                coordinatesToBeDeleted.AddRange(RemoveXraySquaresFromKing(selectedPiece, kingMoves));
                coordinatesToBeDeleted.Distinct().ToList();
            }



            //Deletes any position that other pieces attacks
            for (int i = 0; i < coordinatesToBeDeleted.Count; i++)
            {
                kingMoves.Remove(coordinatesToBeDeleted[i]);
            }

            return kingMoves;
        }

        /// <summary>
        /// Removes xray attacking squares from the king.
        /// The logic is the same as any other square removing methods,
        /// with the slight difference being that it is for specifically ray pieces.
        /// </summary>
        /// <param name="selectedPiece"></param>
        /// <param name="kingMoves"></param>
        /// <returns></returns>
        private List<string> RemoveXraySquaresFromKing(Piece selectedPiece, List<string> kingMoves)
        {
            List<string> coordinatesToBeDeleted = new List<string>(kingMoves.Count);

            if (selectedPiece is RayPiece rayPiece)
            {
                for (int i = 0; i < rayPiece.attckingSquares.Count; i++)
                {
                    if (kingMoves.Contains(rayPiece.attckingSquares[i]))
                    {
                        coordinatesToBeDeleted.Add(rayPiece.attckingSquares[i]);
                    }
                }
            }


            return coordinatesToBeDeleted;
        }

        /// <summary>
        /// Returns illegal castling squares.
        /// </summary>
        /// <param name="castlingSquare"></param>
        /// <returns></returns>
        private string CheckCastlingSquares(string castlingSquare)
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

        /// <summary>
        /// Generates lateral moves for the king.
        /// </summary>
        /// <param name="currentPiece"></param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private List<string> GenerateLateralKingMoves(Piece currentPiece, int y, int x)
        {
            List<string> legalMoves = new List<string>();
            legalMoves.AddRange(GenerateLateralMoves(currentPiece, y, x, 1).legalMoves);

            return legalMoves;
        }

        /// <summary>
        /// Generates diagonal moves for the king.
        /// </summary>
        /// <param name="currentPiece"></param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private List<string> GenerateDiagonalKingMoves(Piece currentPiece, int y, int x)
        {
            List<string> legalMoves = new List<string>();
            legalMoves.AddRange(GenerateDiagonalMoves(currentPiece, y, x, 1).legalMoves);

            return legalMoves;
        }
    }
}