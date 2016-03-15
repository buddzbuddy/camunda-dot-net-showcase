using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Camunda
{
    public class ExternalTask
    {
        public string ActivityId { get; set; }
        public string ActivityInstanceId { get; set; }
        public string Id { get; set; }
        public Dictionary<String, Variable> Variables { get; set; }
    }

    public class Variable
    {
        public String Type { get; set; }
        public String Value { get; set; }
        public String ValueInfo { get; set; }
    }





}
