using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.Dtos;
using FluentValidation;

namespace complaints_back.Validations
{
    public class UserRegisterValidation : AbstractValidator<UserRegisterDto>
    {
        public UserRegisterValidation()
        {
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email is required");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Password is required");
        }

    }

    public class UserLoginValidation : AbstractValidator<UserLoginDto>
    {
        public UserLoginValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email is required").NotNull();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Password is required").Length(6, 100).NotNull();
        }

    }
}