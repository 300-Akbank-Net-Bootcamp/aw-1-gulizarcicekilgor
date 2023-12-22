using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace VbApi.Controllers;

public class Staff
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public decimal? HourlySalary { get; set; }
}

public class StaffValidator : AbstractValidator<Staff>
{
    public StaffValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.")
                            .Length(10, 250).WithMessage("Name must be between 10 and 250 characters.");

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                             .EmailAddress().WithMessage("Email address is not valid.");

        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required.")
                             .Matches(@"^\+(?:[0-9] ?){6,14}[0-9]$").WithMessage("Phone is not valid.");

        RuleFor(x => x.HourlySalary).InclusiveBetween(30, 400).WithMessage("Hourly salary must be between 30 and 400.");
    }
}



[Route("api/[controller]")]
[ApiController]
public class StaffController : ControllerBase
{
    private readonly IValidator<Staff> _validator;

        public StaffController(IValidator<Staff> validator)
        {
        _validator = validator;
        }

    [HttpPost]
    public IActionResult Post([FromBody] Staff value)
    {
        var validationResult = _validator.Validate(value);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(error => error.ErrorMessage));
        }

        // Do something with the valid Staff object
        // ...

        return Ok(value);
    }
}
