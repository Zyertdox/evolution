using LibGit2Sharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

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
            var lastCommit = _repository.Head.Tip;
            if (lastCommit == null)
            {
                var records = CreateBase();
                Save(records);
            }
        }

        public Generation LoadLatest()
        {
            var dataStr = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<Generation>(dataStr);
        }

        public void Save(IList<CreatureRecord> records)
        {
            var generation = new Generation
            {
                RandomSeed = DateTime.UtcNow.ToFileTimeUtc().GetHashCode(),
                Records = records
            };

            var dataStr = JsonConvert.SerializeObject(generation);
            File.WriteAllText(FilePath, dataStr);
            Commands.Stage(_repository, "*");
            var signature = new Signature(SignatureName, SignatureEmail, DateTimeOffset.Now);
            var prevGeneration = _repository.Head.Tip?.Message;
            var v = 0;
            if (prevGeneration != null)
            {
                v = int.Parse(prevGeneration.Split("-")[1]) + 1;
            }
            _repository.Commit("generation-" + v, signature, signature);
        }

        private CreatureRecord[] CreateBase()
        {
            var record = new CreatureRecord
            {
                Id = Guid.NewGuid(),
                ParentId = null,
                Dna = new int[DnaInterpreter.DnaLength]
            };
            record.Dna[0] = 7;
            var records = new CreatureRecord[DnaProcessor.GenerationSetCount];
            for (var i = 0; i < DnaProcessor.GenerationSetCount; i++)
            {
                records[i] = record;
            }

            return records;
        }

        void IDisposable.Dispose()
        {
            _repository.Dispose();
        }
    }

    public class Generation
    {
        public int RandomSeed { get; set; }
        public IList<CreatureRecord> Records { get; set; }
    }

    public class CreatureRecord
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public IList<int> Dna { get; set; }
    }
}
