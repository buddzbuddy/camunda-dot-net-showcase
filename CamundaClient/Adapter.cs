using System.Collections.Generic;

namespace Camunda
{

    public interface Adapter
    {
        void Execute(ExternalTask externalTask, ref Dictionary<string, object> resultVariables);
    }


}
