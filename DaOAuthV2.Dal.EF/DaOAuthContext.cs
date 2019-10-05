using DaOAuthV2.Constants;
using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DaOAuthV2.Dal.EF
{
    public class DaOAuthContext : DbContext, IContext
    {
        public DaOAuthContext(DbContextOptions options) : base(options) { }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Code> Codes { get; set; }

        public DbSet<ClientType> ClientsTypes { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserClient> UsersClients { get; set; }

        public DbSet<Scope> Scopes { get; set; }

        public DbSet<ClientScope> ClientsScopes { get; set; }

        public DbSet<RessourceServer> RessourceServers { get; set; }

        public DbSet<ClientReturnUrl> ClientReturnUrls { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UsersRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("auth");

            modelBuilder.Entity<ClientReturnUrl>().ToTable("ClientReturnUrl");
            modelBuilder.Entity<ClientReturnUrl>().HasKey(rs => rs.Id);
            modelBuilder.Entity<ClientReturnUrl>().Property(p => p.ReturnUrl).HasColumnName("ReturnUrl").HasColumnType("nvarchar(max)").IsRequired();
            modelBuilder.Entity<ClientReturnUrl>().Property(p => p.ClientId).HasColumnName("FK_Client").HasColumnType("int").IsRequired();
            modelBuilder.Entity<ClientReturnUrl>().HasOne<Client>(c => c.Client).WithMany(g => g.ClientReturnUrls).HasForeignKey(c => c.ClientId);

            modelBuilder.Entity<RessourceServer>().ToTable("RessourceServer");
            modelBuilder.Entity<RessourceServer>().HasKey(rs => rs.Id);
            modelBuilder.Entity<RessourceServer>().Property(rs => rs.Id).HasColumnName("Id").HasColumnType("int").IsRequired();
            modelBuilder.Entity<RessourceServer>().Property(rs => rs.CreationDate).HasColumnName("CreationDate").HasColumnType("datetime").IsRequired();
            modelBuilder.Entity<RessourceServer>().Property(rs => rs.Description).HasColumnName("Description").HasColumnType("nvarchar(max)");
            modelBuilder.Entity<RessourceServer>().Property(rs => rs.Name).HasColumnName("Name").HasColumnType("nvarchar(256)").HasMaxLength(256).IsRequired();
            modelBuilder.Entity<RessourceServer>().Property(rs => rs.Login).HasColumnName("Login").HasColumnType("nvarchar(256)").HasMaxLength(256).IsRequired();
            modelBuilder.Entity<RessourceServer>().Property(rs => rs.ServerSecret).HasColumnName("ServerSecret").HasColumnType("varbinary(50)").HasMaxLength(50);
            modelBuilder.Entity<RessourceServer>().Property(rs => rs.IsValid).HasColumnName("IsValid").HasColumnType("bit").IsRequired();
            modelBuilder.Entity<RessourceServer>().HasMany<Scope>(c => c.Scopes).WithOne(c => c.RessourceServer);
            modelBuilder.Entity<RessourceServer>().HasIndex(rs => rs.Login).IsUnique();

            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>().HasKey(c => c.Id);
            modelBuilder.Entity<User>().Property(p => p.Id).HasColumnName("Id").HasColumnType("int").IsRequired();
            modelBuilder.Entity<User>().Property(p => p.BirthDate).HasColumnName("BirthDate").HasColumnType("datetime");
            modelBuilder.Entity<User>().Property(p => p.EMail).HasColumnName("Email").HasColumnType("nvarchar(320)").IsRequired();
            modelBuilder.Entity<User>().Property(p => p.CreationDate).HasColumnName("CreationDate").HasColumnType("datetime").IsRequired();
            modelBuilder.Entity<User>().Property(p => p.FullName).HasColumnName("FullName").HasColumnType("nvarchar(256)").HasMaxLength(256);
            modelBuilder.Entity<User>().Property(p => p.IsValid).HasColumnName("IsValid").HasColumnType("bit").IsRequired();
            modelBuilder.Entity<User>().Property(p => p.Password).HasColumnName("Password").HasColumnType("varbinary(50)").HasMaxLength(50);
            modelBuilder.Entity<User>().Property(p => p.UserName).HasColumnName("UserName").HasColumnType("nvarchar(32)").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<User>().Property(p => p.ValidationToken).HasColumnName("ValidationToken").HasColumnType("nvarchar(256)").HasMaxLength(256);
            modelBuilder.Entity<User>().HasMany<UserClient>(p => p.UsersClients).WithOne(uc => uc.User);
            modelBuilder.Entity<User>().HasMany<UserRole>(u => u.UsersRoles).WithOne(uc => uc.User);
            modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.EMail).IsUnique();

            modelBuilder.Entity<ClientType>().ToTable("ClientType");
            modelBuilder.Entity<ClientType>().HasKey(c => c.Id);
            modelBuilder.Entity<ClientType>().Property(p => p.Id).HasColumnName("Id").HasColumnType("int").IsRequired();
            modelBuilder.Entity<ClientType>().Property(p => p.Wording).HasColumnName("Wording").HasColumnType("nvarchar(256)").HasMaxLength(256).IsRequired();
            modelBuilder.Entity<ClientType>().HasData(
                new ClientType() { Id = 1, Wording = "public" },
                new ClientType() { Id = 2, Wording = "confidential" });

            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Client>().HasKey(c => c.Id);
            modelBuilder.Entity<Client>().Property(p => p.Id).HasColumnName("Id").HasColumnType("int").IsRequired();
            modelBuilder.Entity<Client>().Property(p => p.CreationDate).HasColumnName("CreationDate").HasColumnType("datetime2").IsRequired();
            modelBuilder.Entity<Client>().Property(p => p.Description).HasColumnName("Description").HasColumnType("nvarchar(max)");
            modelBuilder.Entity<Client>().Property(p => p.IsValid).HasColumnName("IsValid").HasColumnType("bit").IsRequired();
            modelBuilder.Entity<Client>().Property(p => p.Name).HasColumnName("Name").HasColumnType("nvarchar(256)").HasMaxLength(256).IsRequired();
            modelBuilder.Entity<Client>().Property(p => p.PublicId).HasColumnName("PublicId").HasColumnType("nvarchar(256)").HasMaxLength(256).IsRequired();
            modelBuilder.Entity<Client>().Property(p => p.ClientSecret).HasColumnName("ClientSecret").HasColumnType("nvarchar(256)").HasMaxLength(256);
            modelBuilder.Entity<Client>().HasMany<ClientScope>(c => c.ClientsScopes).WithOne(c => c.Client);
            modelBuilder.Entity<Client>().Property(p => p.ClientTypeId).HasColumnName("FK_ClientType").HasColumnType("int").IsRequired();
            modelBuilder.Entity<Client>().HasOne<ClientType>(c => c.ClientType).WithMany(ct => ct.Clients).HasForeignKey(ct => ct.ClientTypeId);
            modelBuilder.Entity<Client>().HasMany<ClientReturnUrl>(c => c.ClientReturnUrls).WithOne(c => c.Client);
            modelBuilder.Entity<Client>().Property(p => p.UserCreatorId).HasColumnName("FK_UserCreator").HasColumnType("int").IsRequired();
            modelBuilder.Entity<Client>().HasOne<User>(c => c.UserCreator).WithMany(u => u.CreatedClients).HasForeignKey(c => c.UserCreatorId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Client>().HasIndex(c => c.PublicId).IsUnique();

            modelBuilder.Entity<Scope>().ToTable("Scope");
            modelBuilder.Entity<Scope>().HasKey(s => s.Id);
            modelBuilder.Entity<Scope>().Property(p => p.Id).HasColumnName("Id").HasColumnType("int").IsRequired();
            modelBuilder.Entity<Scope>().Property(p => p.Wording).HasColumnName("Wording").HasColumnType("nvarchar(max)");
            modelBuilder.Entity<Scope>().Property(p => p.NiceWording).HasColumnName("NiceWording").HasColumnType("nvarchar(512)").HasMaxLength(512);
            modelBuilder.Entity<Scope>().HasMany<ClientScope>(c => c.ClientsScopes).WithOne(c => c.Scope);
            modelBuilder.Entity<Scope>().Property(p => p.RessourceServerId).HasColumnName("FK_RessourceServer").HasColumnType("int").IsRequired();
            modelBuilder.Entity<Scope>().HasOne<RessourceServer>(c => c.RessourceServer).WithMany(ct => ct.Scopes).HasForeignKey(ct => ct.RessourceServerId);
   
            modelBuilder.Entity<Code>().ToTable("Code");
            modelBuilder.Entity<Code>().HasKey(c => c.Id);
            modelBuilder.Entity<Code>().Property(p => p.Id).HasColumnName("Id").HasColumnType("int").IsRequired();
            modelBuilder.Entity<Code>().Property(p => p.CodeValue).HasColumnName("CodeValue").HasColumnType("nvarchar(256)").HasMaxLength(256).IsRequired();
            modelBuilder.Entity<Code>().Property(p => p.CodeChallengeValue).HasColumnName("CodeChallengeValue").HasColumnType("nvarchar(512)").HasMaxLength(512);
            modelBuilder.Entity<Code>().Property(p => p.CodeChallengeMethod).HasColumnName("CodeChallengeMethod").HasColumnType("nvarchar(8)").HasMaxLength(8);
            modelBuilder.Entity<Code>().Property(p => p.ExpirationTimeStamp).HasColumnName("ExpirationTimeStamp").HasColumnType("bigint").IsRequired();
            modelBuilder.Entity<Code>().Property(p => p.IsValid).HasColumnName("IsValid").HasColumnType("bit").IsRequired();
            modelBuilder.Entity<Code>().Property(p => p.Scope).HasColumnName("Scope").HasColumnType("nvarchar(max)").HasMaxLength(Int32.MaxValue);
            modelBuilder.Entity<Code>().Property(p => p.UserClientId).HasColumnName("FK_UserClient").HasColumnType("int").IsRequired();
            modelBuilder.Entity<Code>().HasOne<UserClient>(c => c.UserClient).WithMany(g => g.Codes).HasForeignKey(c => c.UserClientId);
            modelBuilder.Entity<Code>().HasIndex(s => s.CodeValue).IsUnique();

            modelBuilder.Entity<UserClient>().ToTable("UserClient");
            modelBuilder.Entity<UserClient>().HasKey(c => c.Id);
            modelBuilder.Entity<UserClient>().Property(p => p.Id).HasColumnName("Id").HasColumnType("int").IsRequired();
            modelBuilder.Entity<UserClient>().Property(p => p.ClientId).HasColumnName("FK_Client").HasColumnType("int").IsRequired();
            modelBuilder.Entity<UserClient>().HasOne<Client>(c => c.Client).WithMany(g => g.UsersClients).HasForeignKey(c => c.ClientId);
            modelBuilder.Entity<UserClient>().Property(p => p.UserId).HasColumnName("FK_User").HasColumnType("int").IsRequired();
            modelBuilder.Entity<UserClient>().HasOne<User>(c => c.User).WithMany(g => g.UsersClients).HasForeignKey(c => c.UserId);
            modelBuilder.Entity<UserClient>().Property(p => p.CreationDate).HasColumnName("CreationDate").HasColumnType("datetime").IsRequired();
            modelBuilder.Entity<UserClient>().Property(p => p.IsActif).HasColumnName("IsActif").HasColumnType("bit").IsRequired();
            modelBuilder.Entity<UserClient>().Property(p => p.RefreshToken).HasColumnName("RefreshToken").HasColumnType("nvarchar(max)").HasMaxLength(Int32.MaxValue);
            modelBuilder.Entity<UserClient>().HasMany<Code>(c => c.Codes).WithOne(c => c.UserClient);

            modelBuilder.Entity<ClientScope>().ToTable("ClientScope");
            modelBuilder.Entity<ClientScope>().HasKey(c => c.Id);
            modelBuilder.Entity<ClientScope>().Property(p => p.ClientId).HasColumnName("FK_Client").HasColumnType("int").IsRequired();
            modelBuilder.Entity<ClientScope>().HasOne<Client>(c => c.Client).WithMany(g => g.ClientsScopes).HasForeignKey(c => c.ClientId);
            modelBuilder.Entity<ClientScope>().Property(p => p.ScopeId).HasColumnName("FK_Scope").HasColumnType("int").IsRequired();
            modelBuilder.Entity<ClientScope>().HasOne<Scope>(c => c.Scope).WithMany(g => g.ClientsScopes).HasForeignKey(c => c.ScopeId);

            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Role>().HasKey(r => r.Id);
            modelBuilder.Entity<Role>().Property(r => r.Id).HasColumnName("Id").HasColumnType("int").IsRequired();
            modelBuilder.Entity<Role>().Property(r => r.Wording).HasColumnName("Wording").HasColumnType("nvarchar(256)").HasMaxLength(256).IsRequired();
            modelBuilder.Entity<Role>().HasMany<UserRole>(u => u.UsersRoles).WithOne(uc => uc.Role);
            modelBuilder.Entity<Role>().HasData(
                new Role() { Id = 1, Wording = RoleName.User },
                new Role() { Id = 2, Wording = RoleName.Administrator });

            modelBuilder.Entity<UserRole>().ToTable("UserRole");
            modelBuilder.Entity<UserRole>().HasKey(ur => ur.Id);
            modelBuilder.Entity<UserRole>().Property(ur => ur.Id).HasColumnName("Id").HasColumnType("int").IsRequired();
            modelBuilder.Entity<UserRole>().Property(ur => ur.UserId).HasColumnName("FK_User").HasColumnType("int").IsRequired();
            modelBuilder.Entity<UserRole>().HasOne<User>(ur => ur.User).WithMany(g => g.UsersRoles).HasForeignKey(c => c.UserId);
            modelBuilder.Entity<UserRole>().Property(ur => ur.RoleId).HasColumnName("FK_Role").HasColumnType("int").IsRequired();
            modelBuilder.Entity<UserRole>().HasOne<Role>(ur => ur.Role).WithMany(g => g.UsersRoles).HasForeignKey(c => c.RoleId);
        }

        public void Commit()
        {
            this.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
