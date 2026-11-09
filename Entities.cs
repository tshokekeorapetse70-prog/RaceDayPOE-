using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class UserAccount
{
    public int UserAccountId { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = "";

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = "";

    [Required]
    public string PasswordHash { get; set; } = "";

    [Required]
    public string Role { get; set; } = "Participant";
}

public class EventOrganizer
{
    public int OrganizerID { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
}

public class RaceEvent
{
    public int EventID { get; set; }
    public int OrganizerID { get; set; }
    public string EventName { get; set; } = "";
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = "";
    public string Description { get; set; } = "";
}

public class Category
{
    public int CategoryID { get; set; }
    public int EventID { get; set; }
    public string CategoryName { get; set; } = "";
    public decimal Distance { get; set; }
    public int MaxParticipants { get; set; }
}

public class Participant
{
    public int ParticipantID { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
}

public class Registration
{
    public int RegistrationID { get; set; }
    public int ParticipantID { get; set; }
    public int EventID { get; set; }
    public int CategoryID { get; set; }

    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    public string RegistrationStatus { get; set; } = "Pending";
}

public class Result
{
    public int ResultsID { get; set; }
    public int RegistrationID { get; set; }
    public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }
    public string? Pace { get; set; }
}

public class WeatherRecord
{
    public int WeatherID { get; set; }
    public int EventID { get; set; }
    public decimal Temperature { get; set; }
    public string Condition { get; set; } = "";
    public decimal WindSpeed { get; set; }
    public DateTime ForecastDate { get; set; }
}

public class RouteRecord
{
    public int RouteID { get; set; }
    public int EventID { get; set; }
    public decimal Distance { get; set; }
    public string RouteName { get; set; } = "";
    public string RouteDescription { get; set; } = "";
}