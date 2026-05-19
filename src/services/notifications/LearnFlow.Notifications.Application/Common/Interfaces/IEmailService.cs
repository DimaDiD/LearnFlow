namespace LearnFlow.Notifications.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEnrollmentConfirmationAsync(
        string recipientEmail,
        string studentName,
        string courseTitle,
        CancellationToken ct = default);

    Task SendCertificateIssuedAsync(
        string recipientEmail,
        string studentName,
        string courseTitle,
        string certificateNumber,
        CancellationToken ct = default);
}