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
            CreateBoard();
            //SetPiecesToBoard(GeneratePieces());
        }

        public void CreateBoard()
        {
            Board.Children.Add(controller.CreateGrid());
            
        }

        public PieceFactory.Piece[,] GeneratePieces()
        {
            return controller.CreatePieces();
        }

        public void SetPiecesToBoard(Piece[,] pieces)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    /*Grid.SetColumn(i); 
                    Grid.SetRow(j);*/
                }
            }
            
        }
    }
}
