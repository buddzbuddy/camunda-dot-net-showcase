using System.Collections.Generic;

namespace Camunda
{

    public interface ExternalTaskAdapter
    {
        void Execute(ExternalTask externalTask, ref Dictionary<string, object> resultVariables);
    }


}
