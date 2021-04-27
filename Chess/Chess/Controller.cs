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
        
        
        public PieceFactory.Piece [,] CreatePieces()
        {
            PieceFactory.Piece[,] pieces = new PieceFactory.Piece[7,7];
            char[] FENotation = ConvertStringToCharArray(model.FENotation);
               
            for (int i = 0; i < 63; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (i < 15)
                    {
                        pieces[i,j] = TranslateFEN(FENotation[i]);
                    }
                }
                
            }
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
