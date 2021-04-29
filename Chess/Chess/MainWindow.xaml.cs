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

        public void MovePiece(object sender, RoutedEventArgs e)
        {
            if(!(e.Source is EmpySquare))
            {
                if(e.Source is Piece piece)
                {
                    controller.SaveCoordinates(Grid.GetRow(piece), Grid.GetColumn(piece));
                    //display legal moves
                    if (controller.IsPieceSelected())
                    {
                        if (controller.IsMoveLegal())
                        {
                            //move piece
                        }
                    }
                }
            }
        }
        public void CreateBoard()
        {
           // Board.Children.Add(controller.CreateGrid());
            
        }

        public Piece[,] GeneratePieces()
        {
            return controller.CreatePieces();
        }

        public void SetPiecesToBoard(Piece[,] pieces)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (pieces[i, j] != null)
                    {
                        pieces[i, j].Click += new RoutedEventHandler(MovePiece);
                        Piece currentPiece = pieces[i, j];
                        Grid.SetRow(currentPiece, i);
                        Grid.SetColumn(currentPiece, j);
                        Board.Children.Add(currentPiece);
                    }
                }
            }
        }
    }
}
