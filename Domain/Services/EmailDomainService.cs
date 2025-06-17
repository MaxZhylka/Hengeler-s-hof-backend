using System.Net;
using System.Net.Mail;
using Hengeler.Application.DTOs.Booking;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;

namespace Hengeler.Domain.Services;

public class EmailDomainService : IEmailDomainService
{
  private readonly SmtpClient _smtpClient;
  private readonly string _fromAddress;

  public EmailDomainService(EmailSettings emailSettings)
  {
    _fromAddress = emailSettings.SenderEmail;
    _smtpClient = new SmtpClient(emailSettings.SmtpServer, emailSettings.Port)
    {
      Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
      EnableSsl = true
    };
  }

  public async Task SendSuccessPaymentEmailAsync(string to, CreateStripeSessionDto booking)
  {
    var subject = "Zahlung erfolgreich - Ihre Buchung ist bestätigt";
    var body = $@"
    <h3>Vielen Dank für Ihre Zahlung!</h3>
    <p>Details Ihrer Buchung:</p>
    <ul>
      <li>Zimmer-ID: {booking.RoomId}</li>
      <li>Anzahl der Tage: {booking.NumberOfDays}</li>
      <li>Preis: {booking.Price} EUR</li>
      <li>Startdatum: {booking.StartDate:yyyy-MM-dd}</li>
      <li>Enddatum: {booking.EndDate:yyyy-MM-dd}</li>
    </ul>
    <p>Wir freuen uns darauf, Sie begrüßen zu dürfen!</p>";

    await SendEmailAsync(to, subject, body, true);
  }

  public async Task SendFailedPaymentEmailAsync(string to)
  {
    var subject = "Zahlung fehlgeschlagen oder erstattet";
    var body = @"
    <h3>Ihre Zahlung war nicht erfolgreich</h3>
    <p>Bitte kontaktieren Sie den Support, wenn Sie glauben, dass es sich um einen Fehler handelt.</p>
    <p>Ihr Geld sollte so bald wie möglich zurückerstattet werden.</p>
    <p>Vielen Dank für Ihr Verständnis.</p>";

    await SendEmailAsync(to, subject, body, true);
  }

  private async Task SendEmailAsync(string to, string subject, string body, bool isHtml)
  {
    var mailMessage = new MailMessage(_fromAddress, to, subject, body)
    {
      IsBodyHtml = isHtml
    };

    try
    {
      await _smtpClient.SendMailAsync(mailMessage);
    }
    catch (SmtpException ex)
    {
      Console.WriteLine($"Error sending email: {ex.Message}");
    }
  }
}
