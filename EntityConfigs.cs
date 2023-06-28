using LearnIOAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageLearningAPI;

public class EntityConfigs
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(u => u.Courses)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);
        }
    }

    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasMany(c => c.Routines)
                .WithOne(r => r.Course)
                .HasForeignKey(r => r.CourseId);
        }
    }

    public class RoutineConfiguration : IEntityTypeConfiguration<Routine>
    {
        public void Configure(EntityTypeBuilder<Routine> builder)
        {
            builder.HasMany(r => r.Assignments)
                .WithOne(i => i.Routine)
                .HasForeignKey(i => i.RoutineId);
        }
    }
    
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.HasMany(r => r.Images)
                .WithOne(i => i.Assignment)
                .HasForeignKey(i => i.AssignmentId);
        }
    }
}