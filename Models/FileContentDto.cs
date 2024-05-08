namespace Dt.Kpsirs.Common.File.Dto;

/// <summary>
/// Содержимое файла
/// </summary>
public class FileContentDto
{
    /// <summary>
    /// Наименование файла
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Файл
    /// </summary>
    public byte[] Buffer { get; }

    public FileContentDto(string fileName, byte[] buffer)
    {
        FileName = fileName;
        Buffer = buffer;
    }
}