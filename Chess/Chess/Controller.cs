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
            if (model.YSourceCoordinate != y && model.XSourceCoordinate != x )
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
                }
            }
        }

        public bool IsPieceSelected()
        {
            return model.IsPieceSelected;
        }

        public bool IsMoveLegal()
        {
            Piece piece = model.InternalBoard[model.YSourceCoordinate, model.XSourceCoordinate];
            for (int i = 0; i < piece.legalMoves.Count; i++)
            {
                if (piece.legalMoves[i] == ConvertIntToString(model.DestinationY, model.DestinationX))
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
