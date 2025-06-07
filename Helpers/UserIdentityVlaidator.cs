using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace complaints_back.Helpers
{
    public class UserIdentityValidator<TUser> : UserValidator<TUser> where TUser : class
    {
        public UserIdentityValidator(IdentityErrorDescriber errors = null) : base(errors)
        {
        }

        public override async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            var result = await base.ValidateAsync(manager, user);

            var errors = new List<IdentityError>(result.Errors);
            errors.RemoveAll(e => e.Code == "DuplicateUserName");

            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}
