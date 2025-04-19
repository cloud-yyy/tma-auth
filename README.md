# TmaAuth

A C# library for validating Telegram Mini App initialization data.

## Overview

TmaAuth is a C# library for working with Telegram Mini App initialization data. It provides functionality to:

- Parse initialization data from a query string format
- Validate the initialization data using the bot token
- Sign initialization data for testing or other purposes

## Installation

Add the NuGet package to your project:

```bash
dotnet add package TmaAuth
```

## Usage

### Validating Initialization Data

The validator helps you verify if the initialization data received from Telegram is authentic and hasn't been tampered with.

```csharp
// Create validator instance
var validator = new TmaInitDataValidator();

// Validate initialization data with 24-hour expiration
string initData = "query_id=AAHdF6IQAAAAAN0XohDhrOrc&user=%7B%22id%22%3A279058397%2C%22first_name%22%3A%22John%22%2C%22last_name%22%3A%22Doe%22%7D&auth_date=1713542400&hash=abc123...";
bool isValid = validator.Validate(initData, "YOUR_BOT_TOKEN", TimeSpan.FromHours(24));

if (isValid)
{
    Console.WriteLine("Initialization data is valid");
}
else
{
    Console.WriteLine("Initialization data is invalid");
}
```

### Creating Signatures

The signer helps you create valid signatures for testing or when you need to generate initialization data programmatically.

```csharp
// Create signer instance
var signer = new TmaInitDataSigner();

// Create a signature from parameters
var parameters = new Dictionary<string, string>
{
    { "user", "{\"id\":123456789,\"first_name\":\"John\",\"last_name\":\"Doe\"}" },
    { "query_id", "AAHdF6IQAAAAAN0XohDhrOrc" },
    { "auth_date", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
};
string hash = signer.Sign(parameters, "YOUR_BOT_TOKEN");
```

## License

This library is open source and available under the MIT license.