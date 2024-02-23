using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Q1_WindowsService;

public sealed class EmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailOptions)
    {
        _emailSettings = emailOptions.Value;
    }

    public void SendEmail(string filePath)
    {
        var now = DateTime.Now;
        using var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromAddress));
        message.To.Add(new MailboxAddress(_emailSettings.ToName, _emailSettings.ToAddress));
        message.Subject = $"PC Workload for {now}";

        var body = new TextPart(TextFormat.Plain)
        {
            Text = $"Kindly find the attached file for the Machine Workload on {now}"
        };

        var attachment = new MimePart("text", "plain")
        {
            Content = new MimeContent(File.OpenRead(filePath), ContentEncoding.Default),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName(filePath)
        };

        // now create the multipart/mixed container to hold the message text and the
        // image attachment
        var multipart = new Multipart("mixed");
        multipart.Add(body);
        multipart.Add(attachment);

        message.Body = multipart;

        using var client = new SmtpClient();
        client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

        // Note: only needed if the SMTP server requires authentication
        client.Authenticate(_emailSettings.GmailUsername, _emailSettings.GmailPassword);

        client.Send(message);
        client.Disconnect(true);
    }
}
