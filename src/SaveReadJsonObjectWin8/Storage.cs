using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace SaveReadJsonObjectWin8
{
    public class PersonStorage : StorageBase<Person>
    {
        protected override string FolderName
        {
            get { return "People"; }
        }
    }

    public abstract class StorageBase<T> where T : Identifiers
    {
        protected abstract string FolderName { get; }
        private const string FileFormat = ".json";
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };

        protected virtual async Task<StorageFolder> StorageFolder()
        {
            return await ApplicationData.Current.LocalFolder.CreateFolderAsync(FolderName, CreationCollisionOption.OpenIfExists);
        }

        public async Task<T> Get(Guid id)
        {
            if (!await DoesFileExist(id))
                return default(T);

            var file = await GetStorageFile(id);
            var fileContent = await FileIO.ReadTextAsync(file);

            return fileContent == null ? default(T) : JsonConvert.DeserializeObject<T>(fileContent, _serializerSettings);
        }

        public async Task<T> Save(T data)
        {
            StorageFile file;
            if (await DoesFileExist(data.Id))
            {
                file = await GetStorageFile(data.Id);
            }
            else
            {
                var folder = await StorageFolder();
                file = await folder.CreateFileAsync(data.Id + FileFormat);
            }

            await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(data, _serializerSettings));

            return data;
        }

        public async void Delete(Guid id)
        {
            if (!await DoesFileExist(id))
                return;

            var file = await GetStorageFile(id);
            await file.DeleteAsync();
        }

        protected async Task<bool> DoesFileExist(Guid id)
        {
            return await GetStorageFile(id) != null;
        }

        protected async Task<StorageFile> GetStorageFile(Guid id)
        {
            var folder = await StorageFolder();
            var files = await folder.GetFilesAsync();

            return files.FirstOrDefault(file => file.Name == id.ToString() + FileFormat);
        }

        public async Task<ObservableCollection<T>> All()
        {
            var all = new ObservableCollection<T>();
            var folder = await StorageFolder();
            var files = await folder.GetFilesAsync();

            foreach (var file in files)
            {
                var fileContent = await FileIO.ReadTextAsync(file);
                var item = JsonConvert.DeserializeObject<T>(fileContent, _serializerSettings);
                all.Add(item);
            }

            return all;
        }
    }
}
