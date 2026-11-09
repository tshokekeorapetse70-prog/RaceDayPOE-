using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController, Route("api/registrations"), Authorize]
public class RegistrationsController(RaceDayContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var q = db.Registrations.AsNoTracking().AsQueryable();

        if (User.IsInRole("Participant") &&
            int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var id))
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var p = await db.Participants
                .FirstOrDefaultAsync(x => x.Email == email);

            if (p is null)
                return Ok(Array.Empty<Registration>());

            q = q.Where(r => r.ParticipantID == p.ParticipantID);
        }

        return Ok(await q.ToListAsync());
    }

    [HttpPost, Authorize(Roles = "Participant")]
    public async Task<IActionResult> Create(Registration r)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        var p = await db.Participants
            .FirstOrDefaultAsync(x => x.Email == email);

        if (p is null)
        {
            return BadRequest(
                "Participant profile must be created by an organiser before registering.");
        }

        if (!await db.Events.AnyAsync(e => e.EventID == r.EventID) ||
            !await db.Categories.AnyAsync(
                c => c.CategoryID == r.CategoryID &&
                     c.EventID == r.EventID))
        {
            return BadRequest("Choose a valid event and category.");
        }

        r.ParticipantID = p.ParticipantID;
        r.RegistrationID = 0;
        r.RegistrationDate = DateTime.UtcNow;
        r.RegistrationStatus = "Pending";

        db.Registrations.Add(r);
        await db.SaveChangesAsync();

        return Created($"/api/registrations/{r.RegistrationID}", r);
    }

    [HttpPatch("{id:int}/status"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Status(
        int id, [FromQuery] string status)
    {
        if (status is not ("Pending" or "Confirmed" or "Cancelled"))
            return BadRequest("Invalid status.");

        var r = await db.Registrations.FindAsync(id);

        if (r is null)
            return NotFound();

        r.RegistrationStatus = status;
        await db.SaveChangesAsync();

        return Ok(r);
    }
}