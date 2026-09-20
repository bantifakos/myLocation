using System.Globalization;
using Microsoft.Maui.Devices.Sensors;

namespace MyLocation;

public partial class MainPage : ContentPage
{
    private const string DefaultButtonText = "Get My Location";
    private readonly IGeolocation geolocation;

    public MainPage(IGeolocation geolocation)
    {
        InitializeComponent();
        this.geolocation = geolocation;
    }

    private async void OnGetLocationClicked(object sender, EventArgs e)
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            SetBusyState(true);

            var location = await geolocation.GetLastKnownLocationAsync();
            location ??= await geolocation.GetLocationAsync(
                new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));

            if (location is null)
            {
                await DisplayAlert("Location Unavailable", "We couldn't determine your current location. Please try again.", "OK");
                return;
            }

            LatitudeEntry.Text = location.Latitude.ToString(CultureInfo.InvariantCulture);
            LongitudeEntry.Text = location.Longitude.ToString(CultureInfo.InvariantCulture);
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Location Unsupported", "This device does not support location services.", "OK");
        }
        catch (FeatureNotEnabledException)
        {
            await DisplayAlert("Location Disabled", "Please enable location services and try again.", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Permission Required", "Location permission is required to fetch your current coordinates.", "OK");
        }
        catch (Exception)
        {
            await DisplayAlert("Location Error", "Something went wrong while getting your location. Please try again.", "OK");
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private void SetBusyState(bool isBusy)
    {
        IsBusy = isBusy;
        GetLocationButton.IsEnabled = !isBusy;
        GetLocationButton.Text = isBusy ? "Getting Location..." : DefaultButtonText;
        BusyIndicator.IsVisible = isBusy;
        BusyIndicator.IsRunning = isBusy;
    }
}
