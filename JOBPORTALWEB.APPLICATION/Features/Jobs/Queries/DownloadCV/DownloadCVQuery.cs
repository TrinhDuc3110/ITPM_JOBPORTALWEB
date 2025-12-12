using MediatR;
using System;
using System.IO;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.DownloadCV
{
    public class DownloadCVQuery : IRequest<(string FilePath, string FileName, Guid OwnerId)?>
    {
        public int ApplicationId { get; set; }
        public Guid RecruiterId { get; set; }

        public DownloadCVQuery(int applicationId, Guid recruiterId)
        {
            ApplicationId = applicationId;
            RecruiterId = recruiterId;
        }
    }
}