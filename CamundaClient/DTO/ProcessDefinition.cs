namespace Camunda
{
    public class ProcessDefinition
    {
        public string id { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string version { get; set; }
        public string startFormKey { get; set; }

        public override string ToString()
        {
            return "ProcessDefinition [Id=" + id + ", Key=" + key + ", Name=" + name + "]";
        }
    }



}
