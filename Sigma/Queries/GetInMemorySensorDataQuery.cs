using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sigma.Queries
{
    
    public class GetInMemorySensorDataQuery : IRequest<List<SensorDataModels.SensorMetaData>>
    {
        public string DeviceId { get; set; }

#nullable enable
        public string? SensorType { get; set; }
#nullable disable

        public DateTime StartDate { get; set; }
    }
}