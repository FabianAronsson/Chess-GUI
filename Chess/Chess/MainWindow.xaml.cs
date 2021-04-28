﻿using System;
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
            SetPiecesToBoard(CreateBoard());
        }

        public PieceFactory.Piece[,] CreateBoard()
        {
            return controller.CreatePieces();
        }

        public void SetPiecesToBoard(PieceFactory.Piece[,] pieces)
        {
            Grid.SetColumn(y); //fix
            Grid.SetRow(x);
        }
    }
}
