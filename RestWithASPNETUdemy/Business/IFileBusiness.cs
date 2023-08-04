using Microsoft.AspNetCore.Http;
using RestWithASPNETUdemy.Data.VO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestWithASPNETUdemy.Business
{
    public interface IFileBusiness
    {
        public byte[] GetFile(string fileName);

        public Task<FileDetailsVO> SaveFileToDisk(IFormFile file);

        public Task<List<FileDetailsVO>> SaveFileToDisk(IList<IFormFile> file);
    }
}
