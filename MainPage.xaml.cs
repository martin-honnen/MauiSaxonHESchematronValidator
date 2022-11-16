using MauiSaxonHESchematronValidator.ViewModels;

namespace MauiSaxonHESchematronValidator;

public partial class MainPage : ContentPage
{

	public MainPage(SaxonSchematronValidatorViewModel saxonSchematronValidatorViewModel)
	{
		InitializeComponent();

		BindingContext = saxonSchematronValidatorViewModel;
	}

    private async void SelectLocalSchematronBtn_ClickedAsync(object sender, EventArgs e)
    {
        try
        {
            FileResult selectedSchematron = await FilePicker.PickAsync(
                new PickOptions()
                {
                    PickerTitle = "Select local Schematron file",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/xml" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".sch", ".xml" } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "sch", "xml" } }, // UTType values
                    })
                });

            if (selectedSchematron != null)
            {
                (BindingContext as SaxonSchematronValidatorViewModel).SchemaUri = selectedSchematron.FullPath;
            }
        }
        catch (Exception ex) { }

    }

    private async void SelectLocalXmlBtn_ClickedAsync(object sender, EventArgs e)
    {
        try
        {
            FileResult selectedXml = await FilePicker.PickAsync(
                new PickOptions()
                {
                    PickerTitle = "Select local Schematron file",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/xml" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".xml" } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "xml" } }, // UTType values
                    })
                });

            if (selectedXml != null)
            {
                (BindingContext as SaxonSchematronValidatorViewModel).InstanceUri = selectedXml.FullPath;
            }
        }
        catch (Exception ex) { }
    }
}

