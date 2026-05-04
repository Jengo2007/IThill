using IThill_academy.Models;
using Microsoft.EntityFrameworkCore;

namespace IThill_academy.Data;

public class ApplicationDbContext:DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {

    }
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<EmailCode> EmailCodes { get; set; }
    public DbSet<PendingRegistration> PendingRegistrations { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Email уникален в PendingRegistrations
        modelBuilder.Entity<PendingRegistration>()
            .HasIndex(p => p.Email)
            .IsUnique();
    }
}