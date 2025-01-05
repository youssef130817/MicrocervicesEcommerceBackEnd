using System;

namespace Order.API.Services;

public class TrackingNumberGenerator
{
    public static string GenerateTrackingNumber()
    {
        // Format: ORD-YYYYMMDD-XXXX où XXXX est un nombre aléatoire
        string dateComponent = DateTime.UtcNow.ToString("yyyyMMdd");
        string randomComponent = new Random().Next(1000, 9999).ToString();
        return $"ORD-{dateComponent}-{randomComponent}";
    }
}