namespace MyMvc.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=MyMvc;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}