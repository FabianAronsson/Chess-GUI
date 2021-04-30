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
        }

        private void MovePiece(object sender, RoutedEventArgs e)
        {
            
                if(e.Source is Piece piece)
                {
                
                    controller.SaveCoordinates(Grid.GetRow(piece), Grid.GetColumn(piece));
                //display legal moves
                CanPlayerMakeMove();
            }
            
        }

        private void CanPlayerMakeMove()
        {
            if (controller.IsDestinationPieceSelected())
            {
                if (true) //controller.IsMoveLegal()
                {
                    UpdateBoard(); //move piece
                    controller.ResetSelectedPieceValues();
                }
            }
        }

        private void RemovePieces()
        {
            Board.Children.Clear();
        }

        private void UpdateBoard()
        {
            RemovePieces();
            SetPiecesToBoard(controller.UpdateMovesOnBoard());
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
                        if(pieces[i, j] is EmptySquare)
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
            if(e.Source is EmptySquare square)
            controller.SavePositionOfSquare(Grid.GetRow(square), Grid.GetColumn(square));
            CanPlayerMakeMove();
        }
    }
}
