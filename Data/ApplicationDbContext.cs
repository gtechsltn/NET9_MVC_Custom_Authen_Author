namespace MyMvc.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost;Database=MyMvcDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
}