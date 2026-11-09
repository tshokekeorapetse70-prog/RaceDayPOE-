using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController(RaceDayContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? eventId)
    {
        var query = db.Categories.AsNoTracking().AsQueryable();

        if (eventId.HasValue)
            query = query.Where(x => x.EventID == eventId.Value);

        return Ok(await query.ToListAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(Category input)
    {
        if (!await db.Events.AnyAsync(
                x => x.EventID == input.EventID))
            return BadRequest("Event does not exist.");

        input.CategoryID = 0;
        db.Categories.Add(input);
        await db.SaveChangesAsync();

        return Created($"/api/categories/{input.CategoryID}", input);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, Category input)
    {
        var item = await db.Categories.FindAsync(id);

        if (item is null)
            return NotFound();

        item.CategoryName = input.CategoryName;
        item.Distance = input.Distance;
        item.MaxParticipants = input.MaxParticipants;

        await db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Categories.FindAsync(id);

        if (item is null)
            return NotFound();

        db.Categories.Remove(item);
        await db.SaveChangesAsync();

        return NoContent();
    }
}