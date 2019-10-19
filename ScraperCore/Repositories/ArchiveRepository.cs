using Newtonsoft.Json;
using ScraperCore.Models;
using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScraperCore.Repositories
{
    public class ArchiveRepository
    {
        private ArchiveRepositoryStateModel _state { get; set; }
        public ArchiveRepository()
        {
            _initState();
            _initRepository(_state);
        }

        private void _initState()
        {
            var fileConfig = "archive-repository-config.json";

            if (!File.Exists(fileConfig))
            {
                _state = new ArchiveRepositoryStateModel();
                File.WriteAllText(fileConfig, JsonConvert.SerializeObject(_state, Formatting.Indented));
            }
            else
                _state = JsonConvert.DeserializeObject<ArchiveRepositoryStateModel>(File.ReadAllText(fileConfig));
        }

        private void _initRepository(ArchiveRepositoryStateModel state)
        {
            _checkDirectory(state.RootFolder);

            foreach(EnumScrapers scraper in Enum.GetValues(typeof(EnumScrapers)))
            {
                _checkDirectory($"{state.RootFolder}/{scraper}");
            }
        }

        private void _checkDirectory(string directory)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        }

        public bool Save(string filename, EnumScrapers scraper)
        {
            var result = true;

            var filenameWoExt = $"{Path.GetFileNameWithoutExtension(filename)}";
            var filenameExt = $"{Path.GetExtension(filename)}";
            var storeFilename = $"{filenameWoExt}-utc-{DateTime.UtcNow.ToString("MM-dd-yyyy-hh-mm-ss")}{filenameExt}";
            var storePathFile = $"{_state.RootFolder}/{scraper}/{storeFilename}";
            var storeFilenameLatest = $"{filenameWoExt}-latest{filenameExt}";
            var storePathFileLatest = $"{_state.RootFolder}/{scraper}/{storeFilenameLatest}";

            File.Copy(filename, storePathFile, overwrite: true);
            File.Copy(filename, storePathFileLatest, overwrite:true);

            return result;
        }

        public FileInfo[] GetFiles(EnumScrapers scraper)
        {
            FileInfo[] list = null;

            var pathFiles = $"{_state.RootFolder}/{scraper}";

            try
            {
                list = new DirectoryInfo(pathFiles).GetFiles();
            }
            catch { }

            return list;
        }

        public string GetRootPath()
        {
            //var fileConfig = "archive-repository-config.json";
            var fileInf = new FileInfo("archive-repository-config.json");

            var result = $"{fileInf.DirectoryName}/{_state.RootFolder}";

            return result;
        }
    }
}
