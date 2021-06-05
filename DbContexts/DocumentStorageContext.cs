using Models.Enums;
using ViewModels;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DbContexts
{
    public class DocumentStorageContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DocumentStorageContext(DbContextOptions options):base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>().HasKey(model => model.Id);
            modelBuilder.Entity<User>().HasKey(model => model.Id);
            modelBuilder.Entity<Tag>().HasKey(model => model.Id);
            modelBuilder.Entity<Role>().HasKey(model => model.Id);
            modelBuilder.Entity<User>().HasIndex(user => user.Username).IsUnique();
            modelBuilder.Entity<User>().HasOne(user => user.Role);

            modelBuilder.Entity<Document>().HasMany(document => document.Tags).WithMany(tag => tag.Documents);

            modelBuilder.Entity<Role>().HasData(
                new Role()
                {
                    Id = Guid.NewGuid(),
                    Name = "Директор"
                },
                new Role()
                {
                    Id = Guid.NewGuid(),
                    Name = "Делопроизваодитель"
                },
                new Role()
                {
                    Id = Guid.NewGuid(),
                    Name = "Методист"
                },
                new Role()
                {
                    Id = Guid.NewGuid(),
                    Name = "Художественный руководитель"
                }
                );
        }

        public async Task<List<Document>> GetFullDocuments() 
        {
            return await Documents.Include(document => document.Tags).ToListAsync();
        }

        public async Task<List<User>> GetFullUsers()
        {
            return await Users.Include(user => user.Role).ToListAsync();
        }

        public async Task<Document> GetFullDocument(Guid id)
        {
            return await Documents.Include(document => document.Tags).FirstOrDefaultAsync(doc => id == doc.Id);
        }

        public async Task<User> GetFullUser(Guid id)
        {
            return await Users.Include(user => user.Role).FirstOrDefaultAsync(user => id == user.Id);
        }

        
    }
}
