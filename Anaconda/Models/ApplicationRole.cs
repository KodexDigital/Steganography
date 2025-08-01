using Microsoft.AspNetCore.Identity;

namespace Anaconda.Models
{
    public class ApplicationRole : IdentityRole<Guid> { public override Guid Id { get; set; } }
}
