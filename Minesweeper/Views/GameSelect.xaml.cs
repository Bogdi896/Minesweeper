using System;
using Microsoft.Maui.Controls;

namespace Minesweeper.Views
{
    public partial class GameSelect : ContentPage
    {

        public event Action<int, int, int> OnGameStartRequested;
        public GameSelect()
        {
            InitializeComponent();
            UpdateMaxBombs();
        }

        private async void OnStartGameClicked(object sender, EventArgs e)
        {
            int rows = (int)rowsSlider.Value;
            int columns = (int)colsSlider.Value;
            int bombs = (int)bombsSlider.Value;

            var gameScreenPage = new GameScreen(rows, columns, bombs);
            await Navigation.PushModalAsync(gameScreenPage);
            //ClosePage();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            ClosePage();
        }

        private async void ClosePage()
        {
            await Navigation.PopModalAsync();
        }
        private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            UpdateMaxBombs();
        }

        private void UpdateMaxBombs()
        {
            // Calculate the maximum number of bombs
            int maxBombs = CalculateMaxBombs((int)rowsSlider.Value, (int)colsSlider.Value);
            bombsSlider.Maximum = maxBombs;

            // Optional: Adjust the current value of the bombs slider if it exceeds the new maximum
            if (bombsSlider.Value > maxBombs)
            {
                bombsSlider.Value = maxBombs;
            }
        }

        private int CalculateMaxBombs(int rows, int cols)
        {
            // Calculate the maximum number of bombs based on rows and columns
            // For example, let's limit it to 40% of the total cells
            return (int)(rows * cols * 0.4);
        }

    }
}
