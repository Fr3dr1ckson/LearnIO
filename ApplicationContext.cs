using LearnIOAPI.Models;
using Microsoft.EntityFrameworkCore;
using static LanguageLearningAPI.EntityConfigs;

namespace LearnIOAPI;

    public sealed class ApplicationContext : DbContext 
    {
        public DbSet<User> Users { get; set; }
        
        public DbSet<Course> Courses { get; set; }
        
        public DbSet<Routine> Routines { get; set; }
        
        public DbSet<Image> Images { get; set; }
        
        public DbSet<Audio> Audios { get; set; }
        
        public DbSet<Assignment> Assignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new CourseConfiguration());
            modelBuilder.ApplyConfiguration(new RoutineConfiguration());
            modelBuilder.ApplyConfiguration(new AssignmentConfiguration());
        }
        public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=CourseWork;Username=postgres;Password=greenbaby2014");
        }
    }
