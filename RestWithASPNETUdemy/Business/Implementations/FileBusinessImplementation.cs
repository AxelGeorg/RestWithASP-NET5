using Microsoft.AspNetCore.Http;
using RestWithASPNETUdemy.Data.VO;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RestWithASPNETUdemy.Business.Implementations
{
    public class FileBusinessImplementation : IFileBusiness
    {
        private readonly string _basePath;
        private readonly IHttpContextAccessor _context;

        public FileBusinessImplementation(IHttpContextAccessor context)
        {
            _context = context;
            _basePath = Directory.GetCurrentDirectory() + "\\UploadDir\\";
        }

        public byte[] GetFile(string fileName)
        {
            var filePath = _basePath + fileName;
            return File.ReadAllBytes(filePath);
        }

        public async Task<FileDetailsVO> SaveFileToDisk(IFormFile file)
        {
            FileDetailsVO fileDetail = new FileDetailsVO();

            string fileType = Path.GetExtension(file.FileName);
            HostString baseUrl = _context.HttpContext.Request.Host;

            if (fileType.ToLower() == ".pdf" || fileType.ToLower() == ".jpg" || 
                fileType.ToLower() == ".png" || fileType.ToLower() == ".jpeg")
            {
                string docName = Path.GetFileName(file.FileName);
                if (file != null && file.Length > 0)
                {
                    string destination = Path.Combine(_basePath, "", docName);
                    fileDetail.DocumentName = docName;
                    fileDetail.DocumentType = fileType;
                    fileDetail.DocumentUrl = Path.Combine(baseUrl + "/api/file/v1" + fileDetail.DocumentName);

                    using var stream = new FileStream(destination, FileMode.Create);

                    await file.CopyToAsync(stream);
                }
            }

            return fileDetail;
        }

        public async Task<List<FileDetailsVO>> SaveFileToDisk(IList<IFormFile> files)
        {
            List<FileDetailsVO> list = new List<FileDetailsVO>();
            foreach (IFormFile file in files) 
                list.Add(await SaveFileToDisk(file));

            return list;
        }
    }
}
