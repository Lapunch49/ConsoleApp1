using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.AccessControl;
//using Dt.Kiuss.Supervisor.Domain.Utils.File;
using Dt.Kpsirs.Common.File;
using Dt.Kpsirs.Common.File.Files;
using Dt.Kpuirs.Common.File.Dto;
using Dt.File.Store;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace ConsoleApp1
{
    public static class FileUpload
    {
        private static FileStoreKpsirs fs;

        private static async Task Main(string[] args)
        {
            // подключаемся к хранилищу
            Menu();
        }

        // Обработаем все файлы в переданном каталоге,
        // выполним повторный поиск по всем найденным каталогам и обработаем файлы, которые они содержат.
        public static void ProcessDirectory(string targetDirectory)
        {
            // Обработаем список файлов, найденных в каталоге.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            // Выполним повторный поиск по подкаталогам этого каталога.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }
        // перемещаем файлы в minIO
        public static async Task ProcessFile(string path)
        {
            //"расщепляем" путь к файлу для передачи аргументов в функцию Create
            char[] delimiterChars = { '\\', '.'};

            string[] partsOfPath = path.Split(delimiterChars);
            int lengthOfPath = partsOfPath.Length;

            // загружаем файл в хранилище
            var fileName = partsOfPath[lengthOfPath - 1];
            Guid fileId = new Guid(partsOfPath[lengthOfPath-2]);
            FileType fileType = getFileType(partsOfPath[lengthOfPath - 3]);
            Guid drillingProjectId = new Guid(partsOfPath[lengthOfPath - 4]);
            byte[] fileContent = System.IO.File.ReadAllBytes(path);
            await fs.CreateFile(fileId, drillingProjectId, fileContent, fileName, fileType);

        }
        public static FileType getFileType(string strFileType)
        {
            switch (strFileType)
            {
                case "Rdrill": return (FileType)1; break;
                case "Attachment": return (FileType)2;
                case "Image": return (FileType)3;
                case "Report": return (FileType)4;
                default: break;
            }
            return (FileType)0;
        }

        private static void Menu()
        {
            fs = new FileStoreKpsirs();
            var path = fs.get_directory();

            if (Directory.Exists(path))
            {
                //path является путем к папке
                ProcessDirectory(path);
            }
            else
            {
                Console.WriteLine($"{path} - неверный путь к папке");
            }

            Console.WriteLine("\n");
            string action;
            action = Console.ReadLine();

        }
    }
}

