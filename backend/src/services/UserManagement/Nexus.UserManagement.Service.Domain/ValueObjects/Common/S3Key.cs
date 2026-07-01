using System.Collections.Immutable;

namespace Nexus.UserManagement.Service.Domain.ValueObjects.Common
{
    public readonly record struct S3Key
    {
        public string Value { get; }

        public string Bucket { get; }
        public ImmutableArray<string> Folders { get; } 
        public string FileName { get; }

        private S3Key(string bucket, ImmutableArray<string> folders, string fileName)
        {
            if (string.IsNullOrWhiteSpace(bucket))
                throw new ArgumentException("Бакет, где будет храниться файл должен быть указан.", nameof(bucket));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Нужно указать название файла.", nameof(fileName));

            if (folders.Any(f => string.IsNullOrWhiteSpace(f)))
                throw new ArgumentException("Порядок папок должен быть либо пустым, либо иметь в себе элементы.", nameof(folders));

            var foldersString = string.Join('/', folders);
            
            Bucket = bucket;
            Folders = folders;
            FileName = fileName;

            Value = Folders.Length > 0 
                ? $"{Bucket}:{string.Join('/', Folders)}/{FileName}"
                : $"{Bucket}:{FileName}";
        }

        public static S3Key Create(string bucket, string[] folders, string fileName)
        {
            ArgumentNullException.ThrowIfNull(bucket);
            ArgumentNullException.ThrowIfNull(folders);
            ArgumentNullException.ThrowIfNull(fileName);

            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            return new (bucket, [.. folders], $"{Guid.CreateVersion7()}{extension}");
        }

        public static S3Key Restore(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Значение не может быть пустым", nameof(value));

            int colonIndex = value.IndexOf(':');
            if (colonIndex <= 0)
                throw new FormatException($"Не правильный формат ключа S3: '{value}'. Отсутствует ':' разделитель.");

            var bucket = value.Substring(0, colonIndex).Trim();
            var pathPart = value.Substring(colonIndex + 1).Trim();

            if (string.IsNullOrWhiteSpace(pathPart))
                throw new FormatException("Путь к файлу или имя файла не должны быть пустыми.");

            int lastSlashIndex = pathPart.LastIndexOf('/');
            string fileName;
            string pathWithoutFile;

            if (lastSlashIndex == -1)
            {
                fileName = pathPart;
                pathWithoutFile = string.Empty;
            }
            else
            {
                fileName = pathPart.Substring(lastSlashIndex + 1);
                pathWithoutFile = pathPart.Substring(0, lastSlashIndex);
            }
            
            if (string.IsNullOrWhiteSpace(fileName))
                throw new FormatException("Укажите название файла.");

            var folders = string.IsNullOrEmpty(pathWithoutFile)
                ? ImmutableArray<string>.Empty
                : pathWithoutFile.Split('/', StringSplitOptions.RemoveEmptyEntries).ToImmutableArray();

            return new(bucket, folders, fileName);
        }

        public string GetObjectKey() => Folders.Length > 0 ? $"{FolderPath}/{FileName}" : FileName;

        public string FolderPath => Folders.Length > 0 ? string.Join('/', Folders) : string.Empty;

        public bool IsInRoot => Folders.Length == 0;

        public override string ToString() => Value;
    }
}