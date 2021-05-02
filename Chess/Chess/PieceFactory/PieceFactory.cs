using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chess.PieceFactory
{
    public class PieceFactory : IPieceFactory
    {
        public Piece CreatePiece(char typeOfPiece, bool isBlack)
        {
            switch (typeOfPiece)
            {
                case 'r':
                    return new Rook
                    {
                        typeOfPiece = "Rook",
                        isBlack = isBlack,
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri("./../Images/Rook.png", UriKind.Relative)),
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    };
                case 'n':
                    return new Knight
                    {
                        typeOfPiece = "Knight",
                        isBlack = isBlack,
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri("./../Images/Knight.png", UriKind.Relative)),
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    };
                case 'b':
                    return new Bishop
                    {
                        typeOfPiece = "Bishop",
                        isBlack = isBlack,
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri("./../Images/Bishop.png", UriKind.Relative)),
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    };
                case 'q':
                    return new Queen
                    {
                        typeOfPiece = "Queen",
                        isBlack = isBlack,
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri("./../Images/Queen.png", UriKind.Relative)),
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    };
                case 'k':
                    return new King
                    {
                        typeOfPiece = "King",
                        isBlack = isBlack,
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri("./../Images/King.png", UriKind.Relative)),
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    };
                case 'p':
                    return new Pawn
                    {
                        typeOfPiece = "Pawn",
                        isBlack = isBlack,
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri("./../Images/Pawn.png", UriKind.Relative)),
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    };
                case 'R':
                    return new Rook
                    {
                        typeOfPiece = "Rook",
                        isBlack = isBlack,
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri("./../Images/wRook.png", UriKind.Relative)),
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    };
                case 'N':
                    return new Knight
                    {
                        typeOfPiece = "Knight",
                        isBlack = isBlack,
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri("./../Images/wKnight.png", UriKind.Relative)),
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    };
                case 'B':
                    return new Bishop
                    {
                        typeOfPiece = "Bishop",
                        isBlack = isBlack,
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri("./../Images/wBishop.png", UriKind.Relative)),
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    };
                case 'Q':
                    return new Queen
                    {
                        typeOfPiece = "Queen",
                        isBlack = isBlack,
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri("./../Images/wQueen.png", UriKind.Relative)),
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    };
                case 'K':
                    return new King
                    {
                        typeOfPiece = "King",
                        isBlack = isBlack,
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri("./../Images/wKIng.png", UriKind.Relative)),
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    };
                case 'P':
                    return new Pawn
                    {
                        typeOfPiece = "Pawn",
                        isBlack = isBlack,
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri("./../Images/wPawn.png", UriKind.Relative)),
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    };
                    
                case 'S':
                    return new EmptySquare
                    {
                        typeOfPiece = "Square",
                        isSpecialPiece = isBlack, //Inaccurate naming, but works
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                        //OverridesDefaultStyle = true
                    };
                default:
                    return null;
            }
        }
    }
}
