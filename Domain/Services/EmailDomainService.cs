using Hengeler.Application.DTOs.Booking;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Hengeler.Domain.Services;

public class EmailDomainService : IEmailDomainService
{
  private readonly EmailSettings _emailSettings;

  public EmailDomainService(EmailSettings emailSettings)
  {
    _emailSettings = emailSettings;
  }

  public async Task SendSuccessPaymentEmailAsync(string to, Booking booking)
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
  <li>Buchungs-ID: {booking.Id}</li>
</ul>
<p>Wir freuen uns darauf, Sie begrüßen zu dürfen!</p>";

    await SendEmailAsync(to, subject, body, isHtml: true);
  }

  public async Task SendFailedPaymentEmailAsync(string to)
  {
    var subject = "Zahlung fehlgeschlagen oder erstattet";
    var body = @"
<h3>Ihre Zahlung war nicht erfolgreich</h3>
<p>Bitte kontaktieren Sie den Support, wenn Sie glauben, dass es sich um einen Fehler handelt.</p>
<p>Ihr Geld sollte so bald wie möglich zurückerstattet werden.</p>
<p>Vielen Dank für Ihr Verständnis.</p>";

    await SendEmailAsync(to, subject, body, isHtml: true);
  }

  private async Task SendEmailAsync(string to, string subject, string body, bool isHtml)
  {
    var email = new MimeMessage();
    email.From.Add(MailboxAddress.Parse(_emailSettings.SenderEmail));
    email.To.Add(MailboxAddress.Parse(to));
    email.Subject = subject;

    if (isHtml)
      email.Body = new TextPart("html") { Text = body };
    else
      email.Body = new TextPart("plain") { Text = body };

    using var smtp = new SmtpClient();
    try
    {
      await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
      await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
      await smtp.SendAsync(email);
    }
    finally
    {
      await smtp.DisconnectAsync(true);
    }
  }
}
