using System.Collections.Generic;
using System.Windows.Controls;
using Camunda;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace InsuranceApplicationWpfTasklist.TaskForms
{
    /// <summary>
    /// Interaktionslogik für AntragPruefen.xaml
    /// </summary>
    public partial class DecideAboutApplication : Page, CamundaTaskForm
    {
        private TasklistWindow Tasklist;
        private HumanTask Task;
        public Dictionary<string, object> TaskVariables { get; set; }
        public Dictionary<string, object> NewVariables { get; set; }

        public DecideAboutApplication()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void initialize(TasklistWindow tasklist, HumanTask task)
        {
            this.Tasklist = tasklist;
            this.Task = task;
            TaskVariables = Tasklist.Camunda.HumanTaskService().LoadVariables(task.id);
            NewVariables = new Dictionary<string, object>();
            NewVariables.Add("approved", false);
        }

        private void buttonCompleteTaskl_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Tasklist.Camunda.HumanTaskService().Complete(Task.id, NewVariables);
            Tasklist.hideDetails();
            Tasklist.reloadTasks();
        }
    }
}
