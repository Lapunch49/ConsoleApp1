//using System;
//using System.Threading.Tasks;
//using Dt.Kpsirs.Common.File.Dto;
//using Microsoft.AspNetCore.Http;

//namespace Dt.Kpsirs.Common.File.Files;

///// <summary>
///// Провайдер хранилища файлов
///// </summary>
//public interface IFileStore
//{
//    /// <summary>
//    /// Получение содержимого файла
//    /// </summary>
//    /// <param name="fileId">Идентификатор файла</param>
//    /// <param name="drillingProgramId">Идентификатор ПБ</param>
//    /// <param name="fileName">Наименование файла</param>
//    /// <param name="fileType">Тип Файла</param>
//    /// <returns>Содержимое файла</returns>
//    Task<FileContentDto> LoadFile(Guid fileId, Guid drillingProgramId, string fileName, FileType fileType);

//    /// <summary>
//    /// Создание файла
//    /// </summary>
//    /// <param name="fileId">Идентификатор файла</param>
//    /// <param name="drillingProgramId">Идентификатор ПБ</param>
//    /// <param name="fileContent">Содержимое файла</param>
//    /// <param name="fileType">Тип Файла</param>
//    /// <returns>Task</returns>
//    Task CreateFile(Guid fileId, Guid drillingProgramId, IFormFile fileContent, FileType fileType);

//    /// <summary>
//    /// Создание файла
//    /// </summary>
//    /// <param name="fileId">Идентификатор файла</param>
//    /// <param name="drillingProgramId">Идентификатор ПБ</param>
//    /// <param name="fileContent">Содержимое файла</param>
//    /// <param name="fileName">Наименование файла</param>
//    /// <param name="fileType">Тип Файла</param>
//    /// <returns>Task</returns>
//    Task CreateFile(Guid fileId, Guid drillingProgramId, byte[] fileContent, string fileName, FileType fileType);

//    /// <summary>
//    /// Удаление файла
//    /// </summary>
//    /// <param name="fileId">Идентификатор файла</param>
//    /// <param name="drillingProgramId">Идентификатор ПБ</param>
//    /// <param name="fileName">Наименование файла</param>
//    /// <param name="fileType">Тип Файла</param>
//    void DeleteFile(Guid fileId, Guid drillingProgramId, string fileName, FileType fileType);
//}