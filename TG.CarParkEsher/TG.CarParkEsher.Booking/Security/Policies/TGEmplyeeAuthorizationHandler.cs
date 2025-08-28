using Microsoft.AspNetCore.Authorization;

namespace TG.CarParkEsher.Booking
{
    public sealed class TGEmplyeeAuthorizationHandler : AuthorizationHandler<TGEmplyeeAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TGEmplyeeAuthorizationRequirement requirement)
        {
            var bookSlot = context.User.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.BookSlot)?.Value;
            var viewSlots = context.User.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.ViewAvailableSlot)?.Value;
            var contactId = context.User.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.ContactId)?.Value;

            if (string.IsNullOrEmpty(bookSlot))
            {
                context.Fail();
                return Task.CompletedTask;
            }
            if (string.IsNullOrEmpty(viewSlots))
            {
                context.Fail();
                return Task.CompletedTask;
            }
            if (string.IsNullOrEmpty(contactId))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;

        }
    }
}
