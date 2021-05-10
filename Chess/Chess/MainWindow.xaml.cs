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
        private Controller controller;
        private ViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            controller = Controller.InitMainController();
            viewModel = new ViewModel();
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
            if (viewModel.RunEventHandler)
            {
                if (e.Source is Piece piece)
                {

                    controller.SaveCoordinates(Grid.GetRow(piece), Grid.GetColumn(piece));
                    //display legal moves
                    if (!controller.IsDestinationPieceSelected())
                    {
                        if (piece.isBlack == controller.GetTurnOrder())
                        {
                            DisplayLegalMoves(piece);

                        }
                    }

                    CanPlayerMakeMove();

                }
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
                pieceToUpdate.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.7 };
            }
        }

        //todo document method
        private void HideLegalMoves(Piece piece)
        {
            List<string> legalMoves = new List<string>(piece.legalMoves);
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

                    if (controller.GetIsPromotion())
                    {
                        ShowPopup();
                    }
                    controller.PlaySound(controller.GetSourcePiece().isBlack, false);
                    UpdateBoard(); //move piece

                    

                    controller.GenerateLegalMoves();
                    controller.PlaySound(controller.GetDestinationPiece().isBlack, true);
                    controller.ResetPieceValues();
                }
                else //user tried making an illegal move
                {
                    controller.GenerateLegalMoves();
                    HideLegalMoves(controller.GetSourcePiece());
                   
                    controller.ResetPieceValues();
                }

            }
        }

        //todo document method
        private void UpdateBoard()
        {
            List<int> sourceCoordinates = controller.GetSourceCoordinates();
            List<int> destinationCoordinates = controller.GetDestinationCoordinates();
            if (sourceCoordinates != destinationCoordinates)
            {
                SetSpecialCaseVisuals();

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

        //todo, document method
        private void SetSpecialCaseVisuals()
        {
            List<int> specialCaseCoordinates = controller.GetSpecialCaseCoordinates();
            if (specialCaseCoordinates[0] != 9) //ARBITRARY NUMBERS, does not matter what number it is, as long as it is not a number used by the board.
            {
                if (specialCaseCoordinates.Count == 2)
                {
                    RemovePiece(specialCaseCoordinates);
                    SetPieceToBoard(specialCaseCoordinates, controller.CreatePiece('S', true));
                    controller.ResetSpecialValues();
                }
                else if (specialCaseCoordinates.Count == 4)
                {
                    List<int> sourcePieceCoordinates = new List<int> { specialCaseCoordinates[0], specialCaseCoordinates[1] };
                    List<int> destinationPieceCoordinates = new List<int> { specialCaseCoordinates[2], specialCaseCoordinates[3] };
                    RemovePiece(sourcePieceCoordinates);
                    RemovePiece(destinationPieceCoordinates);

                    var square = controller.CreatePiece('S', true);
                    square.Click += new RoutedEventHandler(GetPositionOfSquare);
                    SetPieceToBoard(sourcePieceCoordinates, square);

                    SetPieceToBoard(destinationPieceCoordinates, controller.GetSpecificPiece(destinationPieceCoordinates[0], destinationPieceCoordinates[1]));
                }
            }
        }

        private void ShowPopup()
        {
            PromotionPopup.IsOpen = true;
            viewModel.RunEventHandler = false;
        }

        //document
        private void ChoosePieceButtonClick(object sender, RoutedEventArgs e)
        {
            List<int> destinationCoordinates = controller.GetDestinationCoordinates();
            Button button = e.Source as Button;
            if (destinationCoordinates[0] == 7)
            {

                switch (button.Tag.ToString())
                {
                    case "Rook":
                        Rook rook = (Rook)controller.CreatePiece('r', true);
                        rook.Click += new RoutedEventHandler(MovePiece);
                        controller.SetPromotionPiece(rook);
                        break;
                    case "Bishop":
                        Bishop bishop = (Bishop)controller.CreatePiece('b', true);
                        bishop.Click += new RoutedEventHandler(MovePiece);
                        controller.SetPromotionPiece(bishop);
                        break;
                    case "Knight":
                        Knight knight = (Knight)controller.CreatePiece('n', true);
                        knight.Click += new RoutedEventHandler(MovePiece);
                        controller.SetPromotionPiece(knight);
                        break;
                    case "Queen":
                        Queen queen = (Queen)controller.CreatePiece('q', true);
                        queen.Click += new RoutedEventHandler(MovePiece);
                        controller.SetPromotionPiece(queen);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (button.Tag.ToString())
                {
                    case "Rook":
                        Rook rook = (Rook)controller.CreatePiece('R', false);
                        rook.Click += new RoutedEventHandler(MovePiece);
                        controller.SetPromotionPiece(rook);
                        break;
                    case "Bishop":
                        Bishop bishop = (Bishop)controller.CreatePiece('B', false);
                        bishop.Click += new RoutedEventHandler(MovePiece);
                        controller.SetPromotionPiece(bishop);
                        break;
                    case "Knight":
                        Knight knight = (Knight)controller.CreatePiece('N', false);
                        knight.Click += new RoutedEventHandler(MovePiece);
                        controller.SetPromotionPiece(knight);
                        break;
                    case "Queen":
                        Queen queen = (Queen)controller.CreatePiece('Q', false);
                        queen.Click += new RoutedEventHandler(MovePiece);
                        controller.SetPromotionPiece(queen);
                        break;
                    default:
                        break;
                }
            }

            UpdateDestinationPromotionPiece();
            controller.UpdateMovesOnBoard();
            controller.GenerateLegalMoves();
            viewModel.RunEventHandler = true;
            PromotionPopup.IsOpen = false;
        }

        //document
        private void UpdateDestinationPromotionPiece()
        {
            RemovePiece(controller.GetDestinationCoordinates());
            SetPieceToBoard(controller.GetDestinationCoordinates(), controller.GetPromotionPiece());
        }



        //todo, document method
        private void RemovePiece(List<int> coordinates)
        {
            var piece = Board.Children.Cast<UIElement>().First(pieceOnBoard
                => Grid.GetRow(pieceOnBoard) == coordinates[0] && Grid.GetColumn(pieceOnBoard) == coordinates[1]);

            Grid.SetColumn(piece, coordinates[1]);
            Grid.SetRow(piece, coordinates[0]);
            Board.Children.Remove(piece);
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
            if (viewModel.RunEventHandler)
            {
                if (e.Source is EmptySquare square)
                {
                    controller.SavePositionOfSquare(Grid.GetRow(square), Grid.GetColumn(square));
                    CanPlayerMakeMove();
                }
            }
        }
    }
}
