using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.DeleteJob
{
    public class DeleteJobCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public Guid RecruiterId { get; set; }

        public DeleteJobCommand(int id, Guid recruiterId)
        {
            Id = id;
            RecruiterId = recruiterId;
        }
    }
}
