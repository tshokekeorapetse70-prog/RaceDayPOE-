using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/events")]
[Authorize]
public class EventsController(RaceDayContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await db.Events.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await db.Events.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(RaceEvent input)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await db.UserAccounts.FindAsync(
            int.Parse(userId!));

        if (user is null)
            return Unauthorized();

        var organizer = await db.EventOrganizers
            .FirstOrDefaultAsync(x => x.Email == user.Email);

        if (organizer is null)
            return BadRequest(
                "Create a matching organiser profile before adding events.");

        input.EventID = 0;
        input.OrganizerID = organizer.OrganizerID;

        db.Events.Add(input);
        await db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(Get), new { id = input.EventID }, input);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, RaceEvent input)
    {
        var item = await db.Events.FindAsync(id);

        if (item is null)
            return NotFound();

        var email = User.FindFirstValue(ClaimTypes.Email);

        if (item.OrganizerID !=
            (await db.EventOrganizers
                .FirstOrDefaultAsync(x => x.Email == email))
                ?.OrganizerID)
        {
            return Forbid();
        }

        item.EventName = input.EventName;
        item.EventDate = input.EventDate;
        item.Location = input.Location;
        item.Description = input.Description;

        await db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Events.FindAsync(id);

        if (item is null)
            return NotFound();

        var email = User.FindFirstValue(ClaimTypes.Email);

        var organizer = await db.EventOrganizers
            .FirstOrDefaultAsync(x => x.Email == email);

        if (organizer is null ||
            organizer.OrganizerID != item.OrganizerID)
            return Forbid();

        db.Events.Remove(item);
        await db.SaveChangesAsync();

        return NoContent();
    }
}