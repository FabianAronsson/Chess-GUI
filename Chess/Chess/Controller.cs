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
        public Model model; 

        private Controller()
        {
            model = new Model();
        }
       
        public static Controller InitMainController()
        {
            return new Controller();
        }
        
        //todo document method
        public Piece [,] CreatePieces()
        {
            PieceFactory.PieceFactory factory = new PieceFactory.PieceFactory();
            Piece[,] pieces = new Piece[8,8];
            char[] FENotation = ConvertStringToCharArray(model.FENotation);
            int xIndex = 0;
            int yIndex = 0;
            int index = 0;
            while (true) 
            { 
                if (char.IsDigit(FENotation[index])){
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
                else if(FENotation[index].Equals(' '))
                {
                    break;
                }
                else
                {
                    pieces[yIndex, xIndex] = factory.CreatePiece(FENotation[index], IsFENPieceBlack(FENotation, index));
                    if(xIndex == 7)
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

        

        public bool IsFENPieceBlack (char [] FENChar, int index)
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
                if (piece.legalMoves[i] == destinationCoords)
                {
                    return true;
                }
            }
            return false;
        }

        public string ConvertIntToString(int y, int x)
        {
            return y + " " + x;
        }

        public void UpdateMovesOnBoard()
        {
            PieceFactory.PieceFactory factory = new PieceFactory.PieceFactory();
            Piece[,] pieces = model.InternalBoard;
            Piece sourcePiece = model.InternalBoard[model.YSourceCoordinate, model.XSourceCoordinate];

            pieces[model.YSourceCoordinate, model.XSourceCoordinate] = factory.CreatePiece('S', true);
            pieces[model.DestinationY, model.DestinationX] = sourcePiece;
            model.InternalBoard = pieces;

        }

        public void ResetSelectedPieceValues()
        {
            model.IsPieceSelected = false;
            model.IsDestinationPieceSelected = false;
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
            generate.GeneratePseudoLegalMoves(model.InternalBoard, model.IsItBlackToMove);
        }

        /*public Grid CreateGrid() // encapsulate code?
        {
            Grid board = new Grid
            {
                Name = "Board",
                ShowGridLines = true //for debugging purposes
            };
            for (int i = 0; i < 8; i++)
            {
                board.RowDefinitions.Add(new RowDefinition());
                board.ColumnDefinitions.Add(new ColumnDefinition());
            }
            ImageBrush background = new ImageBrush();
            background.ImageSource = new BitmapImage(new Uri("./../../Images/Board.png", UriKind.Relative));
            board.Background = background;
            model.ExternalBoard = board;
            return board;
        }*/


    }
}
