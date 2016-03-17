using System.Collections.Generic;

namespace Camunda
{
    public class Variable
    {
        // lower case to generate JSON we need
        public string type { get; set; }
        public object value { get; set; }
        public object valueInfo { get; set; }
    }

}
