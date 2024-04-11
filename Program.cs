using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.AccessControl;
using Dt.Kiuss.Supervisor.Domain.Utils.File;
using Dt.Kpsirs.Common.File;
using Dt.Kpsirs.Common.File.Files;
using Dt.Kpuirs.Common.File.Dto;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace ConsoleApp1
{
    public static class FileUpload
    {
        private static FileStore fs;
        private static bool IsWindows()
        {
            return OperatingSystem.IsWindows();
        }

        private static async Task Main(string[] args)
        {
            // подключаемся к хранилищу
            //Menu();
            FileStoreKpsirs fs = new FileStoreKpsirs("bucket-kpsirs");
            try
            {
                // загружаем 2 объекта
                Guid fileId = new Guid("4d65eaa9-6b39-4037-b36b-bc2bc4461612");
                Guid drillingProjectId = new Guid("67f5e5e2-a7dc-4bcd-b793-bd4a9a614a09");
                byte[] fileContent = File.ReadAllBytes("D:\\practice\\data\\67f5e5e2-a7dc-4bcd-b793-bd4a9a614a09\\4d65eaa9-6b39-4037-b36b-bc2bc4461612.txt");
                await fs.CreateFile(fileId, drillingProjectId, fileContent, "txt", (FileType)2);
                fileId = new Guid("6cfc3e97-c13a-4f08-b5c3-a2c0136fbe58");
                fileContent = File.ReadAllBytes("D:\\practice\\data\\67f5e5e2-a7dc-4bcd-b793-bd4a9a614a09\\6cfc3e97-c13a-4f08-b5c3-a2c0136fbe58.jpg");
                await fs.CreateFile(fileId, drillingProjectId, fileContent, "jpg", (FileType)3);

                // удаляем 1 объект
                fs.DeleteFile(fileId, drillingProjectId, "jpg", (FileType)3);

                //загружаем 1 объект
                fileId = new Guid("4d65eaa9-6b39-4037-b36b-bc2bc4461612");
                FileContentDto content = await fs.LoadFile(fileId, drillingProjectId, "txt", (FileType)2);
                Console.WriteLine("Успешно!");
            }
            catch (Exception e)
            {
                throw new Exception("Не получилось подключиться к хранилищу по введенным логину и паролю");
            }
        }

        private static void Load()
        {
            // сбор данных о получаемом файле
            //fs.ListOfBuckets();
            Console.WriteLine("");
            // вызов главного меню
            Menu();
        }
        private static void Menu()
        {
            Console.WriteLine("Введите логин и пароль для подключения к хранилищу minIO:");
            fs = new FileStore("bucket-kiuss");
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1 - получить содержимое файла");
            Console.WriteLine("2 - сохранить файл");
            Console.WriteLine("3 - удалить файл");
            int action;
            action = Convert.ToInt32(Console.ReadLine());
            switch (action)
            {
                case 1: Load(); break;
                //case 2: Load(); break;
                //case 3: Load(); break;
                default: Console.WriteLine("Выход из программы"); break;
            }
        }
    }
}

