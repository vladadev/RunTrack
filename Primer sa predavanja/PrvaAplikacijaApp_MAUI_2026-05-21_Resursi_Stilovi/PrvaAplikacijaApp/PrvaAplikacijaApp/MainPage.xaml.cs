namespace PrvaAplikacijaApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
            this.labela_1.FontSize = 10;
        }

        private async void btnOtvoriStranicu1_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new Stranica1());
        }
    }
}
