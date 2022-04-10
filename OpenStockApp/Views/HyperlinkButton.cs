using OpenStockApp.Core.Maui.Services;
using OpenStockApp.Services;

namespace OpenStockApp.Views;

public class HyperlinkButton : Button
{
    public static readonly BindableProperty UrlProperty = BindableProperty.Create(nameof(Url), typeof(string), typeof(HyperlinkLabel), null);

    public string Url
    {
        get { return (string)GetValue(UrlProperty); }
        set { SetValue(UrlProperty, value); }
    }

    public HyperlinkButton()
    {
        GestureRecognizers.Add(new TapGestureRecognizer
        {

            Command = new Command(async () =>
            {
                if (!string.IsNullOrEmpty(Url))
                    await Launcher.OpenAsync(Url);
                //if (ThemeSelectorService.GetCurrentTheme() == AppTheme.Dark)
                //{
                //    TextColor = Color.FromArgb("#ffffff");
                //}
                //else
                //{
                //    TextColor = Color.FromArgb("#4a148c");
                //}

            })
        });

    }
}

