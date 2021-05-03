using Chess.PieceFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Controller controller;
        public MainWindow()
        {
            InitializeComponent();
            controller = Controller.InitMainController();
            //CreateBoard();

        }

        //Event handler for creating the board. Instead of including it in the constructor for Mainwindow, 
        //the board is created ONLY when the grid is fully loaded.
        private void SetupBoard(object sender, RoutedEventArgs e)
        {
            SetPiecesToBoard(GeneratePieces());
            controller.GenerateLegalMoves();

        }

        private void MovePiece(object sender, RoutedEventArgs e)
        {

            if (e.Source is Piece piece)
            {
                controller.SaveCoordinates(Grid.GetRow(piece), Grid.GetColumn(piece));
                //display legal moves
                if (!controller.IsDestinationPieceSelected())
                {
                    DisplayLegalMoves(piece);
                }
                
                CanPlayerMakeMove();
            }
        }

        //todo document method
        private void DisplayLegalMoves(Piece piece)
        {
            List<string> legalMoves = piece.legalMoves;
            string[] currentPosition = new string[2];
            for (int i = 0; i < legalMoves.Count; i++)
            {
                currentPosition = legalMoves[i].Split(' ');
                Piece pieceToUpdate = (Piece)Board.Children.Cast<UIElement>().First(pieceOnBoard
                    => Grid.GetRow(pieceOnBoard) == int.Parse(currentPosition[0]) && Grid.GetColumn(pieceOnBoard) == int.Parse(currentPosition[1]));
                pieceToUpdate.Background = Brushes.Red;
            }

        }

        //todo document method
        private void HideLegalMoves(Piece piece)
        {
            List<string> legalMoves = piece.legalMoves;
            string[] currentPosition = new string[2];

            for (int i = 0; i < legalMoves.Count; i++)
            {
                currentPosition = legalMoves[i].Split(' ');
                Piece pieceToUpdate = (Piece)Board.Children.Cast<UIElement>().First(pieceOnBoard
                    => Grid.GetRow(pieceOnBoard) == int.Parse(currentPosition[0]) && Grid.GetColumn(pieceOnBoard) == int.Parse(currentPosition[1]));
                pieceToUpdate.Background = Brushes.Transparent;
            }
        }

        private void CanPlayerMakeMove()
        {
            if (controller.IsDestinationPieceSelected())
            {
                if (controller.IsMoveLegal()) //controller.IsMoveLegal()
                {
                    HideLegalMoves(controller.GetSourcePiece());
                    UpdateBoard(); //move piece
                    controller.ResetSelectedPieceValues();
                }
                else //user tried making an illegal move
                {
                    controller.ResetSelectedPieceValues();
                    HideLegalMoves(controller.GetSourcePiece());
                }
                
            }
        }

        //todo document method
        private void RemovePiece(List<int> coordinates)
        {
            var piece = Board.Children.Cast<UIElement>().First(pieceOnBoard
                => Grid.GetRow(pieceOnBoard) == coordinates[0] && Grid.GetColumn(pieceOnBoard) == coordinates[1]);

            Grid.SetColumn(piece, coordinates[1]);
            Grid.SetRow(piece, coordinates[0]);
            Board.Children.Remove(piece);
        }

        //todo document method
        private void UpdateBoard()
        {
            List<int> sourceCoordinates = controller.GetSourceCoordinates();
            List<int> destinationCoordinates = controller.GetDestinationCoordinates();
            if (sourceCoordinates != destinationCoordinates)
            {
                RemovePiece(sourceCoordinates); //remove source piece
                RemovePiece(destinationCoordinates); //remove destination piece

                var emptySquare = controller.CreatePiece('S', true);
                emptySquare.Click += new RoutedEventHandler(GetPositionOfSquare);

                //set pieces to board
                SetPieceToBoard(sourceCoordinates, emptySquare);
                SetPieceToBoard(destinationCoordinates, controller.GetSourcePiece());

                //reflect the visual change to the internal board
                controller.UpdateMovesOnBoard();
            }
            

        }

        private void SetPieceToBoard(List<int> coordinates, Piece piece)
        {
            Grid.SetRow(piece, coordinates[0]);
            Grid.SetColumn(piece, coordinates[1]);
            Board.Children.Add(piece);
        }

        public void CreateBoard() //stub, is not possible to do currently.
        {
            // Board.Children.Add(controller.CreateGrid());

        }

        private Piece[,] GeneratePieces()
        {
            return controller.CreatePieces();
        }

        private void SetPiecesToBoard(Piece[,] pieces)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (pieces[i, j] != null)
                    {
                        if (pieces[i, j] is EmptySquare)
                        {
                            pieces[i, j].Click += new RoutedEventHandler(GetPositionOfSquare);
                        }
                        else
                        {
                            pieces[i, j].Click += new RoutedEventHandler(MovePiece);
                        }

                        Piece currentPiece = pieces[i, j];

                        Grid.SetRow(currentPiece, i);
                        Grid.SetColumn(currentPiece, j);

                        Board.Children.Add(currentPiece);
                    }
                }
            }
        }

        private void GetPositionOfSquare(object sender, RoutedEventArgs e)
        {
            if (e.Source is EmptySquare square)
            {
                controller.SavePositionOfSquare(Grid.GetRow(square), Grid.GetColumn(square));
                CanPlayerMakeMove();
            }
        }
    }
}
