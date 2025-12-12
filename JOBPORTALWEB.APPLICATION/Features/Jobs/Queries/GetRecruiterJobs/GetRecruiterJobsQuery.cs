using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using JOBPORTALWEB.APPLICATION.DTOs.Common;
using JOBPORTALWEB.DOMAIN.Entities;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetRecruiterJobs
{
    public class GetRecruiterJobsQuery : IRequest<PaginatedList<Job>>
    {
        public Guid RecruiterId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetRecruiterJobsQuery(Guid recruiterId, int pageNumber, int pageSize)
        {
            RecruiterId = recruiterId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}