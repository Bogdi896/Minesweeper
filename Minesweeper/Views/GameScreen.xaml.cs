using Minesweeper.Models;
using Microsoft.Maui.Controls;

namespace Minesweeper.Views
{
    public partial class GameScreen : ContentPage
    {
        private int rows, columns, bombs;
        private bool isFlagMode = false;
        private GameBoard gameBoard;

        public GameScreen(int rows, int columns, int bombs)
        {
            InitializeComponent();
            this.rows = rows;
            this.columns = columns;
            this.bombs = bombs;

            InitializeGameGrid();
        }

        private void OnToggleModeClicked(object sender, EventArgs e)
        {
            isFlagMode = !isFlagMode;
            toggleModeButton.Text = $"Flag Mode: {(isFlagMode ? "On" : "Off")}";
        }

        private void InitializeGameGrid()
        {
            gameBoard = new GameBoard(rows, columns, bombs);

            minesweeperGrid.Children.Clear();
            minesweeperGrid.RowDefinitions.Clear();
            minesweeperGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < rows; i++)
                minesweeperGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int j = 0; j < columns; j++)
                minesweeperGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var cellButton = new ImageButton
                    {
                        BackgroundColor = Colors.Gray,
                        BorderColor = Colors.Black,
                        BorderWidth = 1,
                        CornerRadius = 5,
                        Aspect = Aspect.AspectFit,
                        WidthRequest = 40,
                        HeightRequest = 40,
                        BindingContext = gameBoard.Cells[i * columns + j],
                    };
                    cellButton.Clicked += OnCellClicked;

                    Grid.SetRow(cellButton, i);
                    Grid.SetColumn(cellButton, j);
                    minesweeperGrid.Children.Add(cellButton);
                }
            }
        }



        private async void OnCellClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton cellButton && cellButton.BindingContext is Minesweeper.Models.Cell cell)
            {
                int row = Grid.GetRow((BindableObject)sender);
                int col = Grid.GetColumn((BindableObject)sender);

                if (isFlagMode)
                {
                    // In flag mode, toggle the flag status of the cell
                    if (!cell.IsRevealed) // Only allow flagging if the cell is not revealed
                    {
                        cell.IsFlagged = !cell.IsFlagged;
                        UpdateButtonAppearance(cellButton, cell);
                    }
                }
                else
                {
                    // In reveal mode, reveal the cell if it is not flagged
                    if (!cell.IsFlagged && !cell.IsRevealed)
                    {
                        if (gameBoard.BombsPlaced)
                        {
                            gameBoard.RevealCell(row, col);
                        }
                        else
                        {
                            gameBoard.FirstCellClicked(row, col); // For the first click
                        }
                        UpdateUI(); // Update the entire grid UI after reveal

                        if (cell.IsMine)
                        {
                            await DisplayAlert("Game Over", "You clicked on a mine!", "OK");
                            await GoBackToMainPage();
                            return; // Exit the method to avoid further processing
                        }
                    }
                }

                // Check for win condition
                if (gameBoard.CheckWinCondition())
                {
                    await DisplayAlert("Congratulations!", "You have won the game!", "OK");
                    await GoBackToMainPage();
                }
            }
        }






        private async Task GoBackToMainPage()
        {
            Application.Current.MainPage = new MainPage();
        }


        private void UpdateUI()
        {
            foreach (var child in minesweeperGrid.Children)
            {
                if (child is ImageButton cellButton && cellButton.BindingContext is Minesweeper.Models.Cell cell)
                {
                    UpdateButtonAppearance(cellButton, cell);
                }
            }
        }





        private void UpdateButtonAppearance(ImageButton cellButton, Minesweeper.Models.Cell cell)
        {
            if (cell.IsRevealed)
            {
                if (cell.IsMine)
                {
                    cellButton.Source = "bomb.png"; // Assuming 'bomb.png' is the image for a bomb
                }
                else
                {
                    string imageName = cell.AdjacentMinesCount == 0 ? null : $"n{cell.AdjacentMinesCount}.png";
                    cellButton.Source = imageName; // Set image based on adjacent mines count
                }
                cellButton.IsEnabled = false; // Disable the button after it is revealed
            }
            else if (cell.IsFlagged)
            {
                cellButton.Source = "flag.png"; // Assuming 'flag.png' is the image for a flag
            }
            else
            {
                cellButton.Source = null; // No image when the cell is not revealed or flagged
                cellButton.IsEnabled = true; // Enable the button
            }
        }

    }
}
