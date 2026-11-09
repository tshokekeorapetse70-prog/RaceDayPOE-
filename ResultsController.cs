using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController, Route("api/results"), Authorize]
public class ResultsController(RaceDayContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.Results.AsNoTracking().ToListAsync());

    [HttpPost, Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(Result r)
    {
        if (!await db.Registrations.AnyAsync(
                x => x.RegistrationID == r.RegistrationID))
        {
            return BadRequest("Registration does not exist.");
        }

        r.ResultsID = 0;
        db.Results.Add(r);
        await db.SaveChangesAsync();

        return Created($"/api/results/{r.ResultsID}", r);
    }

    [HttpPut("{id:int}"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, Result input)
    {
        var r = await db.Results.FindAsync(id);

        if (r is null)
            return NotFound();

        r.FinishTime = input.FinishTime;
        r.Position = input.Position;
        r.Pace = input.Pace;

        await db.SaveChangesAsync();

        return Ok(r);
    }
}