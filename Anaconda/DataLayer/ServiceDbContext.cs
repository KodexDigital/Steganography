using Anaconda.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Anaconda.DataLayer
{
    public class ServiceDbContext(DbContextOptions<ServiceDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<GeoLocation> GeoLocations { get; set; }
        public DbSet<VisitationInfo> VisitationInfos { get; set; }
        public DbSet<UserStat> UserStats { get; set; }
        public DbSet<StegStatelessFile> StegFiles { get; set; }
    }
}