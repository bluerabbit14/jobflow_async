using JobFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobFlow.Infrastructure.Persistence.Configurations;

public class JobAttemptConfiguration : IEntityTypeConfiguration<JobAttempt>
{
    public void Configure(EntityTypeBuilder<JobAttempt> builder)
    {
        builder.ToTable("JobAttempts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(x => x.Error)
            .HasMaxLength(4000);

        builder.HasIndex(x => x.JobId);
        builder.HasIndex(x => x.WorkerId);

        builder.HasOne(x => x.Worker)
            .WithMany(x => x.JobAttempts)
            .HasForeignKey(x => x.WorkerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
