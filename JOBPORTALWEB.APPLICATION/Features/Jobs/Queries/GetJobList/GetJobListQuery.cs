using JOBPORTALWEB.APPLICATION.DTOs.Common;
using JOBPORTALWEB.APPLICATION.DTOs.Job;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.DOMAIN.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetJobList
{
    public class GetJobListQuery : QueryParameters, IRequest<PaginatedList<Job>>
    {
        public bool? IsRemote { get; set; }
        public JobType? JobType { get; set; } 
        public decimal? MinSalary { get; set; }
        public int? JobLevel { get; set; }
        public Guid? CurrentUserId { get; set; }
    }
}
