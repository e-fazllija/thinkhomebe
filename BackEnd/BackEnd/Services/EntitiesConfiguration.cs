using System.Reflection.Emit;
using System.Reflection.Metadata;
using BackEnd.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    public static class EntitiesConfiguration
    {

        public static ModelBuilder ConfigureEntities(this ModelBuilder builder)
        {
            builder.Entity<RealEstateProperty>()
                .HasOne(c => c.Customer).WithMany(c => c.RealEstateProperties);

            builder.Entity<RealEstateProperty>()
                .HasOne(c => c.Agent).WithMany(c => c.RealEstateProperties).HasForeignKey(p => p.AgentId);

            builder.Entity<RealEstateProperty>()
                .HasMany(c => c.Photos).WithOne(e => e.RealEstateProperty);

            return builder;
        }

        #region[Date Localization methods and Params]

        public static DateTime FromLocalToUtc(DateTime date)
        {

            return date.ToUniversalTime();
        }

        public static DateTime? FromLocalToUtcNullable(DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToUniversalTime();
            }
            return null;
        }

        #endregion[Date Localization methods and Params]

    }
}
