using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sigma.Models
{
    public class GeneralSettings
    {
        public int BackgrousServiceUpdateIntervalInMinutes { get; set; }
        public string AzureBlobConnectionString { get; set; }
    }
}
