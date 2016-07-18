using CamundaClient;
using CamundaClient.Dto;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace InsuranceApplicationWpfTasklist.Tasklist
{
    /// <summary>
    /// Interaktionslogik für TaskDetails.xaml
    /// </summary>
    public partial class TaskDetails : Page
    {

        private string cockpiturl;

        public TaskDetails(HumanTask task)
        {
            InitializeComponent();

            cockpiturl = CamundaEngineClient.COCKPIT_URL + "#/process-instance/" + task.processInstanceId + "/runtime";
            CockpitUrlText.Text = cockpiturl;

            taskDetailsListView.Items.Add(new TaskProperty("Process Instance Id", task.processInstanceId));
            taskDetailsListView.Items.Add(new TaskProperty("Process Definition Id", task.processDefinitionId));
            taskDetailsListView.Items.Add(new TaskProperty("Task Assignee", task.assignee));
            taskDetailsListView.Items.Add(new TaskProperty("Task Creation Date", String.Format("{0:dd/MM/yyyy HH:mm:ss}", task.created)));
            taskDetailsListView.Items.Add(new TaskProperty("Task Definition Key", task.taskDefinitionKey));
            taskDetailsListView.Items.Add(new TaskProperty("Task Description", task.description));
            taskDetailsListView.Items.Add(new TaskProperty("Task Due Date", String.Format("{0:dd/MM/yyyy HH:mm:ss}", task.due)));
            taskDetailsListView.Items.Add(new TaskProperty("Task Follow Up Date", String.Format("{0:dd/MM/yyyy HH:mm:ss}", task.followUp)));
            taskDetailsListView.Items.Add(new TaskProperty("Task Form Key", task.formKey));
            taskDetailsListView.Items.Add(new TaskProperty("Task Id", task.id));
            taskDetailsListView.Items.Add(new TaskProperty("Task Name", task.name));
            taskDetailsListView.Items.Add(new TaskProperty("Task Owner", task.owner));
            taskDetailsListView.Items.Add(new TaskProperty("Task Priority", task.priority));
        }

        private void NavigateToCockpit(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(cockpiturl);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(cockpiturl);
        }
    }

    public class TaskProperty {
        public string property { get;  }
        public string value { get; }
        public TaskProperty(string property, string value)
        {
            this.property = property;
            this.value = value;
        }
    }
}
