using Hengeler.Domain.Entities;

namespace Hengeler.Domain.Interfaces;
public interface IEmailDomainService
{
  Task SendSuccessPaymentEmailAsync(string to, Booking createStripeSessionDto);
  Task SendFailedPaymentEmailAsync(string to);
}