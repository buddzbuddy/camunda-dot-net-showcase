using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Camunda;

namespace InsuranceApplicationWpfTasklist.TaskForms
{
    /// <summary>
    /// Interaktionslogik für NewInsuranceApplication.xaml
    /// </summary>
    public partial class NewInsuranceApplication : Page, CamundaStartForm
    {
        private CamundaClient Camunda;
        private ProcessDefinition ProcessDefinition;
        public Dictionary<string, object> Variables { get; set;}

        public NewInsuranceApplication()
        {
            Variables = new Dictionary<string, object>();
            InitializeComponent();
            DataContext = this;
        }

        public void initialize(CamundaClient Camunda, ProcessDefinition ProcessDefinition)
        {
            this.Camunda = Camunda;
            this.ProcessDefinition = ProcessDefinition;
        }

        private void buttonStartProcessInstance_Click(object sender, RoutedEventArgs e)
        {
            Camunda.BpmnWorkflowService().StartProcessInstance(ProcessDefinition.key, Variables);
            Visibility = Visibility.Hidden;
        }
    }
}
