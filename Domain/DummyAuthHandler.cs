using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Hengeler.Domain;

[Obsolete("This is used to mock authentication, for real authentication are used JweAuthenticationHandler")]

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
