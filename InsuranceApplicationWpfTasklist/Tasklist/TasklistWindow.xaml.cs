using System;
using System.Windows;
using System.Windows.Controls;
using Camunda;
using System.ComponentModel;

namespace InsuranceApplicationWpfTasklist
{
    /// <summary>
    /// Interaktionslogik für TasklistWindow.xaml
    /// </summary>
    public partial class TasklistWindow : Window
    {

        private CamundaClient Camunda;

        public TasklistWindow()
        {
            InitializeComponent();
            DataContext = this;
            Camunda = new CamundaClient();
            Camunda.Startup();
            Closing += OnWindowClosing;
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            Camunda.Shutdown();           
        }

        private void buttonReload_Click(object sender, RoutedEventArgs e)
        {
            var tasks = Camunda.HumanTaskService().LoadTasks();
            taskListView.ItemsSource = tasks;
        }

        private void taskListView_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("A ListViewItem was double clicked on task: " + taskListView.SelectedItem);
        }

        private void taskListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HumanTask task = (HumanTask)taskListView.SelectedItem;
            CamundaTaskForm taskFormPage = (CamundaTaskForm) Activator.CreateInstance(Type.GetType(task.formKey));
            taskFormPage.initialize(Camunda, task);
            taskFormFrame.Content = taskFormPage;
        }

        private void buttonStartInsuranceApplication_Click(object sender, RoutedEventArgs e)
        {
            // TODO Load from process definition
            ProcessDefinition processDefinition = new ProcessDefinition();
            processDefinition.key = "insuranceApplication.NET";
            processDefinition.startFormKey = "InsuranceApplicationWpfTasklist.TaskForms.NewInsuranceApplication";

            CamundaStartForm startFormPage = (CamundaStartForm)Activator.CreateInstance(Type.GetType(processDefinition.startFormKey));
            startFormPage.initialize(Camunda, processDefinition);
            taskFormFrame.Content = startFormPage;

        }
    }
}
