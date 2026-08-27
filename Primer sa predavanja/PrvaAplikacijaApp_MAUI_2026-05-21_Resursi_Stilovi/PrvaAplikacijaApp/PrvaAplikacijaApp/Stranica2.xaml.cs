namespace PrvaAplikacijaApp;

public partial class Stranica2 : ContentPage
{
	public Stranica2()
	{
		InitializeComponent();
	}

    private void btn11_Clicked(object sender, EventArgs e)
    {
		this.Resources["ZeleniView"] = this.Resources["ZutiView"];
    }
}