using MediatR;
using JOBPORTALWEB.APPLICATION.DTOs.Job;
using System;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.CreateJob
{
    public class CreateJobCommand : IRequest<int>
    {
        public CreateJobRequest DTO { get; set; }
        public Guid RecruiterId { get; set; } // ID người dùng lấy từ Token

        public CreateJobCommand(CreateJobRequest dto, Guid recruiterId)
        {
            DTO = dto;
            RecruiterId = recruiterId;
        }
    }
}