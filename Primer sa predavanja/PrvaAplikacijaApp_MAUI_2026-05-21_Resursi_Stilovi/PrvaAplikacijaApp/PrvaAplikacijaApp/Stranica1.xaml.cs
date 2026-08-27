namespace PrvaAplikacijaApp;

public partial class Stranica1 : ContentPage
{
	public Stranica1()
	{
		InitializeComponent();

		this.btnOvoriStranicu2.Clicked +=
            (object sender, EventArgs args) =>
            //(sender, args) =>
            {
				this.Navigation.PushAsync(new Stranica2());
			};
	}
}