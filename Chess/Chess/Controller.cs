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
                        xIndex = 0;
                    }
                    else
                    {
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

        public Button CreateEmptySquare()
        {
            return null;
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
