using System.Globalization;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Devices.Sensors;

namespace MyLocation;

public partial class MainPage : ContentPage
{
    private const string DefaultButtonText = "Get My Location";
    private readonly IGeolocation geolocation;
    private readonly IContacts contacts;
    private readonly ISms sms;

    public MainPage(IGeolocation geolocation, IContacts contacts, ISms sms)
    {
        InitializeComponent();
        this.geolocation = geolocation;
        this.contacts = contacts;
        this.sms = sms;
    }

    private async void OnGetLocationClicked(object? sender, EventArgs e)
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
            SendToButton.IsEnabled = true;
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

    private async void OnSendToClicked(object? sender, EventArgs e)
    {
        if (IsBusy)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(LatitudeEntry.Text) || string.IsNullOrWhiteSpace(LongitudeEntry.Text))
        {
            await DisplayAlert("Χωρίς Συντεταγμένες", "Πάτησε πρώτα «Get My Location».", "OK");
            return;
        }

        try
        {
            var status = await Permissions.RequestAsync<Permissions.ContactsRead>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Απαιτείται Άδεια", "Χρειάζεται πρόσβαση στις επαφές για να επιλέξεις παραλήπτη.", "OK");
                return;
            }

            var contact = await contacts.PickContactAsync();
            if (contact is null)
            {
                return;
            }

            var phoneNumber = await ResolvePhoneNumberAsync(contact);
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                await DisplayAlert("Χωρίς Τηλέφωνο", "Η επαφή που επέλεξες δεν έχει αριθμό τηλεφώνου.", "OK");
                return;
            }

            var latitude = LatitudeEntry.Text;
            var longitude = LongitudeEntry.Text;
            var message = new SmsMessage(
                $"Η τοποθεσία μου: {latitude}, {longitude}{Environment.NewLine}https://maps.google.com/?q={latitude},{longitude}",
                phoneNumber);

            await sms.ComposeAsync(message);
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Μη Υποστηριζόμενο", "Η συσκευή δεν υποστηρίζει αποστολή SMS ή επιλογή επαφών.", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Απαιτείται Άδεια", "Χρειάζεται πρόσβαση στις επαφές για να επιλέξεις παραλήπτη.", "OK");
        }
        catch (Exception)
        {
            await DisplayAlert("Σφάλμα Αποστολής", "Κάτι πήγε στραβά κατά την αποστολή του μηνύματος. Δοκίμασε ξανά.", "OK");
        }
    }

    private async Task<string?> ResolvePhoneNumberAsync(Contact contact)
    {
        var phones = contact.Phones
            .Select(phone => phone.PhoneNumber)
            .Where(number => !string.IsNullOrWhiteSpace(number))
            .Distinct()
            .ToList();

        if (phones.Count == 0)
        {
            return null;
        }

        if (phones.Count == 1)
        {
            return phones[0];
        }

        var selection = await DisplayActionSheet("Επίλεξε αριθμό", "Άκυρο", null, phones.ToArray()!);
        return string.IsNullOrWhiteSpace(selection) || selection == "Άκυρο" ? null : selection;
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
