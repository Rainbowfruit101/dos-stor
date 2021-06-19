using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

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
            modelBuilder.Entity<Document>().HasMany(document => document.Tags).WithMany(tag => tag.Documents);
            modelBuilder.Entity<Document>().HasMany(document => document.OwnRoles).WithMany(role => role.AllowDocuments);
            
            modelBuilder.Entity<Tag>().HasKey(model => model.Id);
            
            modelBuilder.Entity<User>().HasKey(model => model.Id);
            modelBuilder.Entity<User>().HasIndex(user => user.Username).IsUnique();
            modelBuilder.Entity<User>().HasOne(user => user.Role).WithMany(role => role.Users);

            modelBuilder.Entity<Role>().HasKey(model => model.Id);

            var director = new Role()
            {
                Id = Guid.NewGuid(),
                Name = "Директор"
            };
            var worker = new Role()
            {
                Id = Guid.NewGuid(),
                Name = "Делопроизваодитель"
            };
            var meth = new Role()
            {
                Id = Guid.NewGuid(),
                Name = "Методист"
            };
            var hudruk = new Role()
            {
                Id = Guid.NewGuid(),
                Name = "Художественный руководитель"
            };
            modelBuilder.Entity<Role>().HasData(director, worker, meth, hudruk);

            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = Guid.NewGuid(),
                    Username = "Some user 1",
                    Password = "qqswqgz3DUsPnFxjHzkAxmFVCNBemPLgOHjKGv4xfBI=",
                    //CurrentRole = worker
                }
            );
        }

        public async Task<List<Document>> GetFullDocuments() 
        {
            return await Documents.Include(document => document.Tags).Include(document => document.OwnRoles).ToListAsync();
        }

        public async Task<List<User>> GetFullUsers()
        {
            return await Users.Include(user => user.Role).ToListAsync();
        }

        public async Task<Document> GetFullDocument(Guid id)
        {
            return await Documents
                .Include(document => document.Tags)
                .Include(document => document.OwnRoles)
                .FirstOrDefaultAsync(doc => id == doc.Id);
        }

        public async Task<User> GetFullUser(Guid id)
        {
            return await Users.Include(user => user.Role).FirstOrDefaultAsync(user => id == user.Id);
        }

        public async Task<Tag> GetFullTag(Guid id)
        {
            return await Tags.Include(t => t.Documents).FirstOrDefaultAsync(t => t.Id == id);
        }
        
        public async Task<Role> GetFullRole(Guid id)
        {
            return await Roles.Include(r => r.AllowDocuments).FirstOrDefaultAsync(r => r.Id == id);
        }
        
        public async Task<Tag> FindTagByName(string name) => await Tags.FirstOrDefaultAsync(t => t.Name == name);
    }
}
