using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using JOBPORTALWEB.APPLICATION.DTOs.User;

namespace JOBPORTALWEB.APPLICATION.Features.Users.Queries.GetRecruiterDashboard
{
    public class GetRecruiterDashboardQuery : IRequest<RecruiterDashboardDto>
    {
        public Guid RecruiterId { get; set; }

        public GetRecruiterDashboardQuery(Guid recruiterId)
        {
            RecruiterId = recruiterId;
        }
    }
}