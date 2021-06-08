using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class DocumentFileService
    {
        private readonly DirectoryInfo RootDirectory;

        public DocumentFileService(DirectoryInfo rootDirectory)
        {
            RootDirectory = rootDirectory;
        }

        // CRUD - create/update
        public async Task Save(Document document, Stream stream)
        {
            var docDir = RootDirectory.CreateSubdirectory(document.Id.ToString());

            var fileStream = File.Open(Path.Combine(docDir.FullName, document.Name), FileMode.OpenOrCreate);

            await stream.CopyToAsync(fileStream);
            fileStream.Close();
        }

        //CRUD - read
        public Stream Get(Document document)
        {
            var fileStream = File.OpenRead(Path.Combine(RootDirectory.FullName, document.Id.ToString(), document.Name));

            return fileStream;
        }

        //CRUD - delete
        public void Delete(Document document)
        {
            var docFilePath = Path.Combine(RootDirectory.FullName, document.Id.ToString(), document.Name);
            if (File.Exists(docFilePath))
            {
                File.Delete(docFilePath);
            }
        }
    }
}
