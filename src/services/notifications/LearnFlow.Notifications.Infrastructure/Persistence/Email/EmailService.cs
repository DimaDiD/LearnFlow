using LearnFlow.Notifications.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace LearnFlow.Notifications.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _smtpHost = configuration["Email:SmtpHost"] ?? "localhost";
        _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "1025");
        _fromEmail = configuration["Email:FromEmail"] ?? "noreply@learnflow.com";
        _fromName = configuration["Email:FromName"] ?? "LearnFlow";
        _logger = logger;
    }

    public async Task SendEnrollmentConfirmationAsync(
        string recipientEmail,
        string studentName,
        string courseTitle,
        CancellationToken ct = default)
    {
        var subject = $"Welcome to {courseTitle}!";
        var body = $"""
            <html>
            <body>
                <h2>Enrollment Confirmation</h2>
                <p>Hello!</p>
                <p>You have successfully enrolled in <strong>{courseTitle}</strong>.</p>
                <p>Start learning today and track your progress in your dashboard.</p>
                <br/>
                <p>Best regards,<br/>LearnFlow Team</p>
            </body>
            </html>
            """;

        await SendEmailAsync(recipientEmail, subject, body, ct);
    }

    public async Task SendCertificateIssuedAsync(
        string recipientEmail,
        string studentName,
        string courseTitle,
        string certificateNumber,
        CancellationToken ct = default)
    {
        var subject = $"Congratulations! Certificate for {courseTitle}";
        var body = $"""
            <html>
            <body>
                <h2>Certificate of Completion</h2>
                <p>Congratulations!</p>
                <p>You have successfully completed <strong>{courseTitle}</strong>.</p>
                <p>Your certificate number: <strong>{certificateNumber}</strong></p>
                <p>You can verify your certificate at learnflow.com/certificates/{certificateNumber}</p>
                <br/>
                <p>Best regards,<br/>LearnFlow Team</p>
            </body>
            </html>
            """;

        await SendEmailAsync(recipientEmail, subject, body, ct);
    }

    private async Task SendEmailAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        CancellationToken ct)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_fromName, _fromEmail));
        message.To.Add(new MailboxAddress(string.Empty, recipientEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpHost, _smtpPort, false, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);

        _logger.LogInformation(
            "Email sent to {RecipientEmail} subject: {Subject}",
            recipientEmail, subject);
    }
}