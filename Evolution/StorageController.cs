using Evolution.Model;
using LibGit2Sharp;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Evolution
{
    public class StorageController : IDisposable
    {
        private const string SourceLib = "../../../../../evolution-log.git/";
        private const string FileDataSource = "dna.txt";
        private const string FilePath = SourceLib + FileDataSource;
        private const string SignatureName = "Evolution";
        private const string SignatureEmail = "@signature";
        private readonly Repository _repository;
        public StorageController()
        {
            Repository.Init(SourceLib);
            _repository = new Repository(SourceLib);
            Commit lastCommit = _repository.Head.Tip;
            if (lastCommit == null)
            {
                Save(FieldProcessor.InitDefaultField());
            }
        }

        public Generation LoadLatest()
        {
            string dataStr = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<Generation>(dataStr);
        }

        public void Save(Field field)
        {
            Generation generation = StorageControllerExtension.ToStorageData(field);

            generation.RandomSeed = DateTime.UtcNow.ToFileTimeUtc().GetHashCode();
            generation.Processors = ProcessorFactory.IndexedProcessors.OrderBy(p => p.Value).Select(p => p.Key.GetType().FullName).ToList();

            string dataStr = JsonConvert.SerializeObject(generation, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None
            });
            File.WriteAllText(FilePath, dataStr);
            Commands.Stage(_repository, "*");
            Signature signature = new Signature(SignatureName, SignatureEmail, DateTimeOffset.Now);
            string prevGeneration = _repository.Head.Tip?.Message;
            int v = 0;
            if (prevGeneration != null)
            {
                v = int.Parse(prevGeneration.Split("-")[1]) + 1;
            }
            _repository.Commit("generation-" + v, signature, signature);
        }

        void IDisposable.Dispose()
        {
            _repository.Dispose();
        }
    }
}
