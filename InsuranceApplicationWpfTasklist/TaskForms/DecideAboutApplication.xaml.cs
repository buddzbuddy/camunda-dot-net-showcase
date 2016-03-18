using System.Collections.Generic;
using System.Windows.Controls;
using Camunda;
using System.Windows;

namespace InsuranceApplicationWpfTasklist.TaskForms
{
    /// <summary>
    /// Interaktionslogik für AntragPruefen.xaml
    /// </summary>
    public partial class DecideAboutApplication : Page, CamundaTaskForm
    {
        private CamundaClient Camunda;
        private HumanTask Task;
        public Dictionary<string, object> TaskVariables { get; set; }
        public Dictionary<string, object> NewVariables { get; set; }

        public DecideAboutApplication()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void initialize(CamundaClient Camunda, HumanTask task)
        {
            this.Camunda = Camunda;
            this.Task = task;
            TaskVariables = Camunda.HumanTaskService().LoadVariables(task.id);
            NewVariables = new Dictionary<string, object>();
            NewVariables.Add("approved", false);
        }

        private void buttonCompleteTaskl_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Camunda.HumanTaskService().Complete(Task.id, NewVariables);
            Visibility = Visibility.Hidden;
        }
    }
}
