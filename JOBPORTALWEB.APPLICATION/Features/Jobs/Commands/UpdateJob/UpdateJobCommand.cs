using MediatR;
using JOBPORTALWEB.APPLICATION.DTOs.Job;
using System;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.UpdateJob
{
    // Command trả về true/false để báo hiệu thành công/thất bại
    public class UpdateJobCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public UpdateJobRequest DTO { get; set; }
        public Guid RecruiterId { get; set; }

        public UpdateJobCommand(int id, UpdateJobRequest dto, Guid recruiterId)
        {
            Id = id;
            DTO = dto;
            RecruiterId = recruiterId;
        }
    }
}