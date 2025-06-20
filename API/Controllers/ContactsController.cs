using Hengeler.Application.DTOs.Contacts;
using Hengeler.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hengeler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController(IContactService contactService, IConfiguration configuration) : ControllerBase
{
  private readonly string _adminEmails = configuration["AdminEmails"] ?? "";

  [HttpGet]
  public async Task<ActionResult<ContactDto>> Get()
  {
    var contact = await contactService.GetContactAsync();
    return Ok(contact);
  }

  [Authorize]
  [HttpPut("/api/Contacts")]
  public async Task<ActionResult<ContactDto>> UpdateContacts([FromBody] UpdateContactDto dto)
  {
    var email = User.FindFirst("email")?.Value;

    if (!_adminEmails.Split(',').Contains(email))
    {
      Console.WriteLine(_adminEmails);
      Console.WriteLine(email);
      return Forbid();
    }
    var updated = await contactService.UpdateContactAsync(dto);
    return Ok(updated);
  }

  [Authorize]
  [HttpPut("/api/SocialMedia")]
  public async Task<ActionResult<ContactDto>> UpdateSocialMedia([FromBody] UpdateContactDto dto)
  {
    var email = User.FindFirst("email")?.Value;

    if (!_adminEmails.Split(',').Contains(email))
    {
      Console.WriteLine(_adminEmails);
      Console.WriteLine(email);
      return Forbid();
    }
    var updated = await contactService.UpdateSocialMediaAsync(dto);
    return Ok(updated);
  }
}