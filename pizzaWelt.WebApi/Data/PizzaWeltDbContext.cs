namespace PizzaWelt.Data
{
    public class PizzaWeltDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        private readonly DbContextOptions _options;

        public PizzaWeltDbContext(DbContextOptions<PizzaWeltDbContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
            _options = options;
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<ApplicationUserRoles> IdentityRoleModel { get; set; }
        public DbSet<UserAccounts> UserAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.ApplyConfiguration(new RoleConfiguration());

            modelBuilder.Entity<ApplicationUser>(c =>
            {
                c.HasIndex(b => b.UserAccountsId)
                    .IsUnique(true);
            });

            modelBuilder.Entity<UserAccounts>(c =>
            {
                c.HasOne<ApplicationUserRoles>()
                    .WithMany(b => b.UserAccounts)
                    .HasForeignKey("RoleId")
                    .IsRequired(true);
                c.HasIndex(b => b.Email)
                    .IsUnique(true);
            });
        }
    }
}
