using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CamundaClient.Dto;

namespace CamundaClient.Requests
{
    class CompleteRequest
    {
        public Dictionary<string, Variable> Variables { get; set; }
        public string WorkerId { get; set; }
    }
}
