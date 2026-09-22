# MyLocation

MyLocation is a .NET MAUI app written in C# that lets you fetch the device's current GPS coordinates and display them in two text boxes.

## Prerequisites

- .NET 9 SDK
- .NET MAUI workload: `dotnet workload install maui`
- A supported target platform and emulator/device (Android, iOS, Mac Catalyst, or Windows)

## Project Structure

- `MyLocation.sln` - solution file at the repository root
- `MyLocation/` - .NET MAUI application project

## Build and Run

From the repository root:

```bash
dotnet restore MyLocation.sln
dotnet build MyLocation.sln
```

Example platform run commands:

```bash
# Android
dotnet build -t:Run -f net9.0-android MyLocation/MyLocation.csproj

# Windows
dotnet build -t:Run -f net9.0-windows10.0.19041.0 MyLocation/MyLocation.csproj
```

## Location Permissions

The app requests location access when you tap **Get My Location**.

- **Android**: `ACCESS_COARSE_LOCATION`, `ACCESS_FINE_LOCATION`, and `ACCESS_NETWORK_STATE`
- **iOS / Mac Catalyst**: `NSLocationWhenInUseUsageDescription`
- **Windows**: `location` device capability

Make sure location services are enabled on the device or emulator before testing.
