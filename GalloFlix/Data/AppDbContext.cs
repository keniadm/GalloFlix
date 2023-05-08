using GalloFlix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GalloFlix.Data;
public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Genre> Genres { get; set;}
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieComment> MovieComments { get; set; } 
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<MovieRating> MovieRating { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) //void não tem retorno
    {
        base.OnModelCreating(builder);
        AppDbSeed appDbSeed = new(builder);

        //FluentAPI
        #region Personalização do Identity
        builder.Entity<IdentityUser>(b => {
            b.ToTable("Users");
        });
        builder.Entity<IdentityUserClaim<string>>(b => {
            b.ToTable("UserClaims");
        });
        builder.Entity<IdentityUserLogin<string>>(b => {
            b.ToTable("UserLogins");
        });
        builder.Entity<IdentityUserToken<string>>(b => {
            b.ToTable("UserTokens");
        });
        builder.Entity<IdentityRole>(b => {
            b.ToTable("Roles");
        });
        builder.Entity<IdentityRoleClaim<string>>(b => {
            b.ToTable("RoleClaims");
        });
        builder.Entity<IdentityUserRole<string>>(b => {
            b.ToTable("UserRoles");
        });
        #endregion

        #region Many To Many - MovieComment
        builder.Entity<MovieComment>()
            .HasOne(mc => mc.Movie)
            .WithMany(m => m.Comments)
            .HasForeignKey(mc => mc.MovieId);

        builder.Entity<MovieComment>()
            .HasOne(mc => mc.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(mc => mc.UserId);
        #endregion

        #region Many To Many - MovieGenre
        // Definição de Chave Primária Composta
        builder.Entity<MovieGenre>().HasKey(
            mg => new { mg.MovieId, mg.GenreId }
        );

        builder.Entity<MovieGenre>()
            .HasOne(mg => mg.Movie)
            .WithMany(m => m.Genres)
            .HasForeignKey(mg => mg.MovieId);

        builder.Entity<MovieGenre>()
            .HasOne(mg => mg.Genre)
            .WithMany(g => g.Movies)
            .HasForeignKey(mg => mg.GenreId);
        #endregion

        #region Many To Many - MovieRating
        builder.Entity<MovieRating>().HasKey(
            mr => new { mr.MovieId, mr.UserId }
        );

        builder.Entity<MovieRating>()
            .HasOne(mr => mr.Movie)
            .WithMany(m => m.Ratings)
            .HasForeignKey(mr => mr.MovieId);

        builder.Entity<MovieRating>()
            .HasOne(mr => mr.User)
            .WithMany(u => u.Ratings)
            .HasForeignKey(mr => mr.UserId);
        #endregion
    }
}