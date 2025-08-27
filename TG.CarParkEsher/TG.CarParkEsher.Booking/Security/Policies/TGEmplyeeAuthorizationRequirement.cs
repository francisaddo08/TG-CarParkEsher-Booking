using Microsoft.AspNetCore.Authorization;

namespace TG.CarParkEsher.Booking
{
    public sealed class TGEmplyeeAuthorizationRequirement : IAuthorizationRequirement
    {
        public TGEmplyeeAuthorizationRequirement()
        {
        }
    }
}