namespace Authentication.API.Services;

public class EmailService
{
    public Task SendPasswordResetCode(string email, string code)
    {
        // Pour le test, on affiche simplement le code dans la console
        Console.WriteLine($"Code de réinitialisation pour {email}: {code}");
        Console.WriteLine($"Ce code expire dans 2 minutes");
        return Task.CompletedTask;
    }
}