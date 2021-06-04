using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllegraMVCApp.Models
{
    public class ServiceApiResponse
    {
        public string Title { get; set; }
        public string ErrorType { get; set; }
        public int Status { get; set; }
        public string ReasonPhrase { get; set; }
        public string TraceId { get; set; }
        public bool Success { get; set; }
        public object Result { get; set; }
    }
}
