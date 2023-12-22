using System.ComponentModel.DataAnnotations;
using System.Data;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc;

namespace VbApi.Controllers;

public class Employee {
     public string Name { get; set; }
     public DateTime DateOfBirth { get; set; }
     public string Email { get; set; }
     public string Phone { get; set; }
     public double HourlySalary { get; set; }

}
public class EmployeeValidator : AbstractValidator<Employee>
{

    public EmployeeValidator()
    {
        RuleFor(x=>x.Name).NotEmpty().WithMessage("Name is required")
                            .Length(10,250).WithMessage("Name must be 10 and 250 characters");
        RuleFor(x=>x.Email).NotEmpty().WithMessage("Emailis required")
                            .EmailAddress().WithMessage("Email. address is not valid");
        RuleFor(x=>x.Phone).NotEmpty().WithMessage("Phone is required")
            .Matches(@"^\+(?:[0-9] ?){6,14}[0-9]$").WithMessage("Phone is not valid.");
        RuleFor(x => x.HourlySalary)
                .InclusiveBetween(50, 400)
                .SetValidator(new MinLegalSalaryRequiredValidator());
            
            RuleFor(x => x.DateOfBirth)
                .Must(BeValidBirthDate)
                .WithMessage("Birthdate is not valid.");
    }

    private bool BeValidBirthDate (DateTime dateOfBirth)
    {
        var minAllowedBirthDate= DateTime.Today.AddYears(-65);
        return minAllowedBirthDate <= dateOfBirth;
    }

     public class MinLegalSalaryRequiredValidator : PropertyValidator
{
    private readonly double _minJuniorSalary;
    private readonly double _minSeniorSalary;

    public MinLegalSalaryRequiredValidator(double minJuniorSalary = 50, double minSeniorSalary = 200)
        : base("Minimum hourly salary is not valid.")
    {
        _minJuniorSalary = minJuniorSalary;
        _minSeniorSalary = minSeniorSalary;
    }

    protected override bool IsValid(PropertyValidatorContext context)
    {
        var employee = (Employee)context.Instance;
        var dateOfBirth = employee.DateOfBirth;
        var hourlySalary = (double)context.PropertyValue;
      

        var isValidSalary = dateOfBirth <= DateTime.Today.AddYears(-30)
            ? hourlySalary >= _minSeniorSalary
            : hourlySalary >= _minJuniorSalary;

        return isValidSalary;
    }
}


}
[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    public EmployeeController()
    {
    }

  [HttpPost]
    public IActionResult Post([FromBody] Employee value)
    {
        var validator = new EmployeeValidator();
        var validationResult = validator.Validate(value);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        // Do something with the valid Employee object
        // ...

        return Ok(value);
    }
}