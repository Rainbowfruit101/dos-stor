using DocumentStorage.Enums;
using DocumentStorage.Models;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DocumentStorage
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

        //TODO: отрефакторить
        public async Task<List<Document>> SearchDocumentByName(string query) 
        {
            query = query.ToLower();
            return await Documents.Include(document => document.Tags).Where(document => document.Name.ToLower().Contains(query)).ToListAsync();
        }

        public async Task<List<Document>> SearchDocumentByDate(DateSearchViewModel model)
        {
            return await Documents.Where(document => Helpers.CheckDate(document.CreationTime, model)).ToListAsync();
        }

        public async Task<List<Document>> SearchDocumentsByTags(TagSearchViewModel model)
        {
            return model.Mode == TagSearchMode.Any ? await SearchDocumentsByAnyTags(model.Tags) : await SearchDocumentsByExactTags(model.Tags);
            
        }
        public async Task<List<Document>> SearchDocumentsByAnyTags(List<Tag> tags) 
        {
            var requestedTagIds = tags.Select(tag => tag.Id);
            return Tags.Include(tag => tag.Documents)
                .Where(tag => requestedTagIds.Contains(tag.Id))
                .Select(tag => tag.Documents)
                .Aggregate((accumulated,current)=> accumulated.Concat(current).ToList()); //функция сокращения Reduce
        }

        public async Task<List<Document>> SearchDocumentsByExactTags(List<Tag> tags)
        {
            var requestedTagIds = tags.Select(tag => tag.Id);
            var requestedCount = requestedTagIds.Count();
            return await Documents
                .Where(doc => requestedCount == doc.Tags
                    .Select(tag => tag.Id)
                    .Intersect(requestedTagIds)
                    .Count())
                .ToListAsync();
        }
    }
}
