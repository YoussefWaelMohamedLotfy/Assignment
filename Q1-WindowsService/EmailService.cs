using MailKit.Net.Smtp;
using MimeKit;

namespace Q1_WindowsService;

public sealed class EmailService
{


    public void SendEmail()
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Youssef Wael", "youssefwael8@gmail.com"));
        message.To.Add(new MailboxAddress("Receipient Youssef", "youssefwael8@gmail.com"));
        message.Subject = $"PC Workload for {DateTime.Now}";

        using var client = new SmtpClient();
        client.Connect("smtp.gmail.com", 587, true);

        // Note: only needed if the SMTP server requires authentication
        client.Authenticate("youssefwael8", "bhzforufothdvnif");

        client.Send(message);
        client.Disconnect(true);
    }
}
