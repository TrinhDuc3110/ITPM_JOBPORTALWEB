using MediatR;
using JOBPORTALWEB.APPLICATION.DTOs.Job;
using System.Collections.Generic;
using System;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetApplications
{
    public class GetApplicationsQuery : IRequest<List<JobApplicationDto>>
    {
        public Guid RecruiterId { get; set; }

        public GetApplicationsQuery(Guid recruiterId)
        {
            RecruiterId = recruiterId;
        }
    }
}