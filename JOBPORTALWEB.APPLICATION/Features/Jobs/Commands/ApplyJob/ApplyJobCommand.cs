using MediatR;
using JOBPORTALWEB.APPLICATION.DTOs.Job;
using System;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.ApplyJob
{
    public class ApplyJobCommand : IRequest<int>
    {
        public int JobId { get; set; }
        public Guid JobSeekerId { get; set; }
        public ApplyJobRequest DTO { get; set; }
        public string CvPath { get; set; }

        public ApplyJobCommand(int jobId, Guid jobSeekerId, ApplyJobRequest dto, string cvPath)
        {
            JobId = jobId;
            JobSeekerId = jobSeekerId;
            DTO = dto;
            CvPath = cvPath;
        }
    }
}