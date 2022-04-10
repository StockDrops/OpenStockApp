using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.Extensions.Options;
using OpenStockApp.Email.Models.Email;

namespace OpenStockApp.Email.Models.Context;

public class EmailDbContext : DbContext
{
    private readonly byte[] _encryptionKey;

    //private readonly byte[] _encryptionIV = ...;

    private readonly IEncryptionProvider _provider;
    private readonly string _dbConnectionString;

    public DbSet<EmailUser> EmailUsers => Set<EmailUser>();

    //public EmailDbContext() { }

    public EmailDbContext(IOptions<EmailOptions> emailOptions)
    {

        if (emailOptions.Value.ConnectionString == null)
            throw new ArgumentNullException(nameof(emailOptions.Value.ConnectionString));
        if (emailOptions.Value.EncryptionKey == null)
            throw new ArgumentNullException(nameof(emailOptions.Value.EncryptionKey));

        _encryptionKey = Convert.FromBase64String(emailOptions.Value.EncryptionKey);
        _dbConnectionString = emailOptions.Value.ConnectionString;
        _provider = new AesProvider(_encryptionKey);

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseEncryption(_provider);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_dbConnectionString);
        base.OnConfiguring(optionsBuilder);
    }
}
///// <summary>
///// This class allows running "dotnet database update" without a parameterless constructor on AppDbContext
///// </summary>
//public class EmailDbContextFactory : IDesignTimeDbContextFactory<EmailDbContext>
//{
//    public EmailDbContext CreateDbContext(string[] args)
//    {
//        var optionsBuilder = new DbContextOptionsBuilder<EmailDbContext>();
//        return new EmailDbContext(optionsBuilder.Options);
//    }
//}
