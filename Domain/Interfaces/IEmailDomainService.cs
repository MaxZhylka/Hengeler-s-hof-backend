
using Hengeler.Application.DTOs.Booking;

namespace Hengeler.Domain.Interfaces;
public interface IEmailDomainService
{
  Task SendSuccessPaymentEmailAsync(string to, CreateStripeSessionDto createStripeSessionDto);
  Task SendFailedPaymentEmailAsync(string to);
}