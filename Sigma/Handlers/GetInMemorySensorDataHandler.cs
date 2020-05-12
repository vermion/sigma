using MediatR;
using Sigma.Backgroundservices;
using Sigma.Queries;
using Sigma.SensorDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sigma.Handlers
{   
    public class GetInMemorySensorDataHandler : IRequestHandler<GetInMemorySensorDataQuery, List<SensorMetaData>>
    {

        private readonly RetrieveSensorDataClient _retrieveSensorDataClient;

        public GetInMemorySensorDataHandler(RetrieveSensorDataClient retrieveSensorDataClient)
        {
            _retrieveSensorDataClient = retrieveSensorDataClient;
        }

        Task<List<SensorMetaData>> IRequestHandler<GetInMemorySensorDataQuery, List<SensorMetaData>>.Handle(GetInMemorySensorDataQuery request, CancellationToken cancellationToken)
        {
            if (request.SensorType == null)
            {
                var sensorData = _retrieveSensorDataClient._metaSensorData.Where(x => x.DeviceID == request.DeviceId && x.MeasurementDay.Date.ToString("yyyy-MM-dd") == request.StartDate.Date.ToString("yyyy-MM-dd"));
                if (sensorData != null)
                    return Task.FromResult(sensorData.ToList());
                else
                    return null;
            }

            var result = _retrieveSensorDataClient._metaSensorData.Where(x => x.GetType().Name.ToLower() == request.SensorType && x.DeviceID == request.DeviceId && x.MeasurementDay.Date.ToString("yyyy-MM-dd") == request.StartDate.Date.ToString("yyyy-MM-dd"));
            if (result == null)
                return null;
            else if (request.SensorType == "temperature")
            {
                var temp = (Temperature)result.FirstOrDefault();
                temp.ConvertCelsiusToFahrenheit();

                return Task.FromResult(result.ToList());
            }
            else
            {
                return Task.FromResult(result.ToList());
            }
        }
    }
}

