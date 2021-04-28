using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public PieceFactory.Piece [,] CreatePieces()
        {
            PieceFactory.Piece[,] pieces = new PieceFactory.Piece[7,7];
            char[] FENotation = ConvertStringToCharArray(model.FENotation);
            int xIndex = 0;
            int yIndex = 0;
            int index = 0;
            while (true) // todo check if code works logically
            { 
                if (char.IsDigit(FENotation[index])){
                    if (Convert.ToInt32(FENotation[index]) <= 8)
                    {
                        xIndex = 0;
                    }
                    else
                    {
                        xIndex += Convert.ToInt32(FENotation[index]);
                    }
                }
                else if (FENotation[index].Equals('/'))
                {
                    yIndex++;
                }
                else if (xIndex == 8) // refactor code? maybe?
                {
                    xIndex = 0;
                }
                else if(FENotation[index].Equals(' '))
                {
                    break;
                }
                else
                {
                    pieces[yIndex, xIndex] = TranslateFEN(FENotation[index]);
                    xIndex++;
                }
                index++; 
            }
            return pieces;
        }

        public PieceFactory.Piece TranslateFEN (char FENotation)
        {
            switch (FENotation)
            {
                case 'r':
                    return new PieceFactory.Rook
                    {
                        isBlack = true
                    };
                case 'n':
                    return new PieceFactory.Knight
                    {
                        isBlack = true
                    };
                case 'b':
                    return new PieceFactory.Bishop
                    {
                        isBlack = true
                    };
                case 'q':
                    return new PieceFactory.Queen
                    {
                        isBlack = true
                    };
                case 'k':
                    return new PieceFactory.King
                    {
                        isBlack = true
                    };
                case 'p':
                    return new PieceFactory.Pawn
                    {
                        isBlack = true
                    };
                case 'R':
                    return new PieceFactory.Rook
                    {
                        isBlack = false
                    };
                case 'N':
                    return new PieceFactory.Knight
                    {
                        isBlack = false
                    };
                case 'B':
                    return new PieceFactory.Bishop
                    {
                        isBlack = false
                    };
                case 'Q':
                    return new PieceFactory.Queen
                    {
                        isBlack = false
                    };
                case 'K':
                    return new PieceFactory.King
                    {
                        isBlack = false
                    };
                case 'P':
                    return new PieceFactory.Pawn
                    {
                        isBlack = false
                    };
                default:
                    return null;
                    
            }
        }

        public char[] ConvertStringToCharArray (string str)
        {
            return str.ToCharArray();
        }


       // public BitmapImage image = new BitmapImage(new Uri("./../Images/wPawn.png", UriKind.Relative));

        

       
    }
}
