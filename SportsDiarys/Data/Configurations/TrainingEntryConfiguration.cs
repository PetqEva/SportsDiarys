using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsDiarys.Models;
using static SportsDiarys.Common.ValidationConstants;

namespace SportsDiarys.Data.Configurations
{
    public class TrainingEntryConfiguration : IEntityTypeConfiguration<TrainingEntry>

    {
        public void Configure(EntityTypeBuilder<TrainingEntry> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.SportName)
                .IsRequired()
                .HasMaxLength(SportNameMaxLength);

            // Optional: precision for DistanceKm (portable across providers that support it)
            // builder.Property(e => e.DistanceKm).HasPrecision(6, 2);
        }


    }
}

