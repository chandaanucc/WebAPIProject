using System;
using FluentValidation;
using DataLayer.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DataLayer.Models
{
    public class Registration
    {
         
        public int RegistrationId { get; set; }
        
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        //public string Token { get; set; } 
        // Add any additional fields related to user registration as needed
        public List<MediaUpload>? MediaUpload ;
        // public List<ImageRecord>? Filerecord ;

    }
}
public class RegistrationValidator : AbstractValidator<Registration>
{
    public RegistrationValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$")
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one digit");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");
    }
}
