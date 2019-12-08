using Apsis.Abstractions.Repository;
using Apsis.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apsis.Repository
{
    public class UploadRepository : IUploadRepository
    {
        readonly IRepository _dataRepository;
        public UploadRepository(IRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public async Task<Upload> GetUpload(Guid fileId)
        {
            try
            {
                return (await _dataRepository.QueryAsync<Upload>(SqlQueries.GetUploadByUploadedFileId, new { id = fileId })).FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Log
                return null;
            }
        }

        public async Task<int> UploadFileAsync(Upload upload)
        {
            try
            {
                return await _dataRepository.ExecuteScalarAsync<int>(SqlQueries.AddUpload
                    , new { id = upload.Id, filename = upload.FileName, filepath = upload.FilePath });
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return 0;
            }
        }

        public async Task<IEnumerable<Upload>> GetUploadsDetails(IEnumerable<Guid> uploadedFileId)
        {
            try
            {
                return (await _dataRepository.QueryAsync<Upload>(SqlQueries.GetMultipleUploadsByUploadedFileId, new { id = uploadedFileId }))?.ToList();
            }
            catch (Exception ex)
            {
                // Log
                return null;
            }
        }
    }
}

