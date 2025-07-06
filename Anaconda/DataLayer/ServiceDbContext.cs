using Anaconda.Models;
using Microsoft.EntityFrameworkCore;

namespace Anaconda.DataLayer
{
    public class ServiceDbContext(DbContextOptions<ServiceDbContext> options) : DbContext(options)
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<GeoLocation> GeoLocations { get; set; }
        public DbSet<VisitationInfo> VisitationInfos { get; set; }
        public DbSet<UserStat> UserStats { get; set; }
    }
}