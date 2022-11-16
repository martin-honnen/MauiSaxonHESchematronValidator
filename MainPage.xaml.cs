using MauiSaxonHESchematronValidator.ViewModels;

namespace MauiSaxonHESchematronValidator;

public partial class MainPage : ContentPage
{

	public MainPage(SaxonSchematronValidatorViewModel saxonSchematronValidatorViewModel)
	{
		InitializeComponent();

		BindingContext = saxonSchematronValidatorViewModel;
	}

}

