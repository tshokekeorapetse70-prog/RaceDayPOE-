using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController, Route("api"), Authorize]
public class EventInfoController(RaceDayContext db) : ControllerBase
{
    [HttpGet("weather")]
    public async Task<IActionResult> Weather([FromQuery] int? eventId)
    {
        var q = db.Weather.AsNoTracking().AsQueryable();

        if (eventId.HasValue)
            q = q.Where(x => x.EventID == eventId);

        return Ok(await q.ToListAsync());
    }

    [HttpPost("weather"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> AddWeather(WeatherRecord w)
    {
        if (!await db.Events.AnyAsync(e => e.EventID == w.EventID))
            return BadRequest("Event does not exist.");

        w.WeatherID = 0;
        db.Weather.Add(w);
        await db.SaveChangesAsync();

        return Ok(w);
    }

    [HttpGet("routes")]
    public async Task<IActionResult> Routes([FromQuery] int? eventId)
    {
        var q = db.Routes.AsNoTracking().AsQueryable();

        if (eventId.HasValue)
            q = q.Where(x => x.EventID == eventId);

        return Ok(await q.ToListAsync());
    }

    [HttpPost("routes"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> AddRoute(RouteRecord r)
    {
        if (!await db.Events.AnyAsync(e => e.EventID == r.EventID))
            return BadRequest("Event does not exist.");

        r.RouteID = 0;
        db.Routes.Add(r);
        await db.SaveChangesAsync();

        return Ok(r);
    }
}