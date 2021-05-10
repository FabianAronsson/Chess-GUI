using Chess.PieceFactory;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        }

        //Event handler for creating the board. Instead of including it in the constructor for Mainwindow, 
        //the board is created ONLY when the grid is fully loaded.
        private void SetupBoard(object sender, RoutedEventArgs e)
        {
            SetPiecesToBoard(GeneratePieces());
            controller.GenerateLegalMoves();
        }

        /// <summary>
        /// Every time a piece is moved the coordinates of that piece is saved, and every subsequent call is saved as a destination coordinate.
        /// If it is the first time this method is called per sourcepiece then it will call on a method that displays the piece's legal moves. Finally
        /// it also calls on a method to check if the move is legal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Displays legal moves based on the piece's legal moves. Every square that corresponds to the piece's legal moves is painted red 
        /// to highlight to the user where the piece can move. 
        /// 
        /// Grid.Children getter from: https://stackoverflow.com/questions/1511722/how-to-programmatically-access-control-in-wpf-grid-by-row-and-column-index 
        /// </summary>
        /// <param name="piece">The piece's legal moves to update visually.</param>
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

        /// <summary>
        /// Does the complete opposite of the previous method, instead of painting legal squares it removes the red color and sets it 
        /// to transparent.
        /// 
        /// Grid.Children getter from: https://stackoverflow.com/questions/1511722/how-to-programmatically-access-control-in-wpf-grid-by-row-and-column-index 
        /// </summary>
        /// <param name="piece">The piece's legal moves to hide.</param>
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

        /// <summary>
        /// The bread and butter of the whole thing. This method checks several parameters to make sure that 
        /// the player can make its move. 
        /// </summary>
        private void CanPlayerMakeMove()
        {
            if (controller.IsDestinationPieceSelected())
            {
                if (controller.IsMoveLegal()) //Check if the move is legal
                {
                    HideLegalMoves(controller.GetSourcePiece());

                    if (controller.GetIsPromotion()) //If a pawn is on the last rank of either side, then it should promote
                    {
                        ShowPopup();
                    }
                    controller.PlaySound(controller.GetSourcePiece().isBlack, false); //If a move is of type check then this method is called.

                    UpdateBoard(); //moves pieces visually


                    controller.GenerateLegalMoves(); //Generates new legal moves based on the new position.
                    controller.PlaySound(controller.GetDestinationPiece().isBlack, true); //Should it not be a check the same method is called again, but with different paramters.
                    controller.ResetPieceValues();
                    IsGameOver();
                }
                else //user tried making an illegal move
                {
                    controller.GenerateLegalMoves();
                    HideLegalMoves(controller.GetSourcePiece());

                    controller.ResetPieceValues();
                }

            }
        }

        private void IsGameOver()
        {
            if (!controller.DoesNextPlayerHaveLegalMoves())
            {
                GameOver.IsOpen = true;
            }
        }

        private void ExitGame(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CloseGameOverPopup(object sender, RoutedEventArgs e)
        {
            GameOver.IsOpen = false;
        }

        /// <summary>
        /// This method works by first checking so that the user have not pressed the same piece twice, as it would result in a null exception. Afterwards
        /// special cases are set first, this includes castling, en passant and promotion. It then removes previous pieces and sets the new pieces to their
        /// corresponding locations. Lastly, the visual change is also change in the internal board.
        /// </summary>
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
                if (specialCaseCoordinates.Count == 2) //if the move is an en passant
                {
                    RemovePiece(specialCaseCoordinates);
                    SetPieceToBoard(specialCaseCoordinates, controller.CreatePiece('S', true));
                    controller.ResetSpecialValues();
                }
                else if (specialCaseCoordinates.Count == 4) //if the move is a castling move, castling moves always have 2 coordinates, hence why it is == 4
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

        /// <summary>
        /// This method sets the correct promotion piece, depending on what pawn reached what rank
        /// If it reached the seventh rank then a black piece is created. If it reached the zeroth rank then
        /// a white piece is creating. Every creation is based on the users action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            //Reflects the visual changes to the internal board.
            UpdateDestinationPromotionPiece();
            controller.UpdateMovesOnBoard();
            controller.GenerateLegalMoves();
            viewModel.RunEventHandler = true;
            PromotionPopup.IsOpen = false;
        }

        /// <summary>
        /// Sets the destination piece to its corresponding promotion piece.
        /// </summary>
        private void UpdateDestinationPromotionPiece()
        {
            RemovePiece(controller.GetDestinationCoordinates());
            SetPieceToBoard(controller.GetDestinationCoordinates(), controller.GetPromotionPiece());
        }



        /// <summary>
        /// Removes a specific piece from the board.
        /// 
        /// Code from: https://stackoverflow.com/questions/1511722/how-to-programmatically-access-control-in-wpf-grid-by-row-and-column-index 
        /// </summary>
        /// <param name="coordinates"></param>
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

        private Piece[,] GeneratePieces()
        {
            return controller.CreatePieces();
        }

        /// <summary>
        /// Sets all pieces visually like how they are positioned inside the pieces array.
        /// </summary>
        /// <param name="pieces">The array with the pieces</param>
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
