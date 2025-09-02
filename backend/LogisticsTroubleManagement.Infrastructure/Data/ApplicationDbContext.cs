using LogisticsTroubleManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}

	public DbSet<User> Users => Set<User>();
	public DbSet<Incident> Incidents => Set<Incident>();
	public DbSet<Attachment> Attachments => Set<Attachment>();
	public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
	public DbSet<Effectiveness> Effectiveness => Set<Effectiveness>();
	
	// マスタデータのDbSet
	public DbSet<TroubleType> TroubleTypes => Set<TroubleType>();
	public DbSet<DamageType> DamageTypes => Set<DamageType>();
	public DbSet<Warehouse> Warehouses => Set<Warehouse>();
	public DbSet<ShippingCompany> ShippingCompanies => Set<ShippingCompany>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
		base.OnModelCreating(modelBuilder);
	}


}
