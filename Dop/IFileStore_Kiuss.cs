using System;
using System.Threading.Tasks;
//using Dt.Kiuss.Core.Model.File;
//using Microsoft.AspNetCore.Http;
using Dt.Kpuirs.Common.File.Dto;

namespace Dt.Kiuss.Supervisor.Domain.Utils.File;

/// <summary>
/// Провайдер хранилища файлов
/// </summary>
public interface IFileStore
{
    /// <summary>
    /// Получение содержимого файла
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="drillingProjectId">Идентификатор ПБ</param>
    /// <param name="fileName">Наименование файла</param>
    /// <returns>Содержимое файла</returns>
    Task<FileContentDto> LoadFile(Guid fileId, Guid drillingProjectId, string fileName);

    /// <summary>
    /// Создание файла
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="drillingProjectId">Идентификатор ПБ</param>
    /// <param name="fileContent">Содержимое файла</param>
    /// <returns>Результат асинхронной операции</returns>
    //Task CreateFile(Guid fileId, Guid drillingProjectId, IFormFile fileContent);

    /// <summary>
    /// Создание файла
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="drillingProjectId">Идентификатор ПБ</param>
    /// <param name="fileContent">Содержимое файла</param>
    /// <param name="fileName">Наименование файла</param>
    /// <returns>Результат асинхронной операции</returns>
    Task CreateFile(Guid fileId, Guid drillingProjectId, byte[] fileContent, string fileName);

    /// <summary>
    /// Удаление файла
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="drillingProjectId">Идентификатор ПБ</param>
    /// <param name="fileName">Наименование файла</param>
    void DeleteFile(Guid fileId, Guid drillingProjectId, string fileName);
}