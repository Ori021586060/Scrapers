using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScraperCore.Repositories;

namespace WebScraperManager.Controllers
{
    [Route("[controller]")]
    public class DownloadController : Controller
    {
        private ArchiveRepository _archiveRepository = new ArchiveRepository();

        [Route("{id}/{filename?}")]
        public IActionResult Index(string id, string filename)
        {
            IActionResult result = null; ;
            var pathArchive = _archiveRepository.GetRootPath();
            var pathfile = $"{pathArchive}/{id}/{filename}";

            if (!System.IO.File.Exists(pathfile)) pathfile = _changeToLatestFilename(pathfile);

            System.IO.File.WriteAllText("1-download-files.log", $"pathfile:{pathfile}");

            if (System.IO.File.Exists(pathfile))
                result = PhysicalFile(pathfile
                    , "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    , Path.GetFileName(pathfile)
                    );
            else result = View("FileNotFound", filename);

            return result;
        }

        private string _changeToLatestFilename(string pathfile)
        {
            var path = Path.GetDirectoryName(pathfile);
            var filenameWoExt = Path.GetFileNameWithoutExtension(pathfile);
            var ext = Path.GetExtension(pathfile);

            var result = $"{path}/{filenameWoExt}-latest{ext}";

            return result;
        }
    }
}