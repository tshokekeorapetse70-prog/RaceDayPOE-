using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController, Route("api/participants"), Authorize]
public class ParticipantsController(RaceDayContext db) : ControllerBase
{
    [HttpGet, Authorize(Roles = "Organiser")]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.Participants.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var p = await db.Participants.FindAsync(id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost, Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(Participant p)
    {
        p.ParticipantID = 0;

        db.Participants.Add(p);
        await db.SaveChangesAsync();

        return Created($"/api/participants/{p.ParticipantID}", p);
    }

    [HttpPut("{id:int}"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, Participant input)
    {
        var p = await db.Participants.FindAsync(id);

        if (p is null)
            return NotFound();

        p.FirstName = input.FirstName;
        p.LastName = input.LastName;
        p.Email = input.Email;
        p.Phone = input.Phone;

        await db.SaveChangesAsync();

        return Ok(p);
    }
}