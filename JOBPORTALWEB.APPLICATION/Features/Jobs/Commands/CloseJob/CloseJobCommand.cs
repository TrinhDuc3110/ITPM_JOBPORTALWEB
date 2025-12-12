using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.CloseJob
{
    public class CloseJobCommand : IRequest<bool>
    {
        public int JobId { get; set; }
        public Guid RecruiterId { get; set; }

        public CloseJobCommand(int jobId, Guid recruiterId)
        {
            JobId = jobId;
            RecruiterId = recruiterId;
        }
    }
}