using Minesweeper.Views;

namespace Minesweeper
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void ShowGameSettings()
        {
            var gameSelectPage = new GameSelect();
            gameSelectPage.OnGameStartRequested += StartGame;

            await Navigation.PushModalAsync(gameSelectPage);
        }

        private void StartGame(int rows, int columns, int bombs)
        {
            // Logic to start the game with these settings
        }
        private async void OnStartGameClicked(object sender, EventArgs e)
        {
            // Your logic to show the GameSelect page
            var gameSelectPage = new GameSelect();
            // Subscribe to an event or set up other logic as needed
            await Navigation.PushModalAsync(gameSelectPage);
        }


    }
}
