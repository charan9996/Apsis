using Apsis.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apsis.Abstractions.Repository
{
    public interface IUploadRepository
    {
        Task<Upload> GetUpload(Guid fileId);
        Task<int> UploadFileAsync(Upload upload);
        Task<IEnumerable<Upload>> GetUploadsDetails(IEnumerable<Guid> uploadedFileId);
    }
}
