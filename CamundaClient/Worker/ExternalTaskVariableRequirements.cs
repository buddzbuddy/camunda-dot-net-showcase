namespace CamundaClient.Worker
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)
    ]
    public class ExternalTaskVariableRequirements : System.Attribute
    {
        public string[] VariablesToFetch;

        public ExternalTaskVariableRequirements(params string[] VariablesToFetch)
        {
            this.VariablesToFetch = VariablesToFetch;
        }
    }
}
