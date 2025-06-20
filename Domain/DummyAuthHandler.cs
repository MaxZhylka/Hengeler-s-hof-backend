using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Hengeler.Domain;
public class DummyAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                        ILoggerFactory logger,
                        UrlEncoder encoder,
                        ISystemClock clock) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
  protected override Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    var principal = Context.User;
    var ticket = new AuthenticationTicket(principal, Scheme.Name);
    return Task.FromResult(AuthenticateResult.Success(ticket));
  }
}
