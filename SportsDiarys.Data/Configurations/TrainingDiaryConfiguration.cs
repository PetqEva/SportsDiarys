using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsDiarys.Models;
using static SportsDiarys.Common.ValidationConstants;

namespace SportsDiarys.Data.Configurations
{
    public class TrainingDiaryConfiguration : IEntityTypeConfiguration<TrainingDiary>

    {
        public void Configure(EntityTypeBuilder<TrainingDiary> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Date)
                .IsRequired();

            builder.Property(d => d.Notes)
                .HasMaxLength(DiaryNotesMaxLength);

            // 1 дневник на ден за потребител
            builder
                .HasIndex(d => new { d.UserProfileId, d.Date })
                .IsUnique();

            builder
                .HasMany(d => d.TrainingEntries)
                .WithOne(e => e.TrainingDiary)
                .HasForeignKey(e => e.TrainingDiaryId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
