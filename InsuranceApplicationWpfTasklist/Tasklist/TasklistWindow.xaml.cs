using System;
using System.Windows;
using System.Windows.Controls;
using Camunda;
using System.ComponentModel;
using System.Reflection;

namespace InsuranceApplicationWpfTasklist
{
    /// <summary>
    /// Interaktionslogik für TasklistWindow.xaml
    /// </summary>
    public partial class TasklistWindow : Window
    {

        public CamundaClient Camunda { get; }

        public TasklistWindow()
        {
            InitializeComponent();
            DataContext = this;
            Camunda = new CamundaClient();
            Camunda.Startup();
            reloadTasks();
            Closing += OnWindowClosing;
        }

        public void reloadTasks()
        {
            var tasks = Camunda.HumanTaskService().LoadTasks();
            taskListView.ItemsSource = tasks;
            /*
            Assembly thisExe = Assembly.GetEntryAssembly();
            var htmlStream = thisExe.GetManifestResourceStream("InsuranceApplicationWpfTasklist.Tasklist.diagram.html");
            DiagramBrowser.NavigateToStream(htmlStream);
            */
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            Camunda.Shutdown();
        }

        private void buttonReload_Click(object sender, RoutedEventArgs e)
        {
            reloadTasks();
        }

        private void taskListView_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("A ListViewItem was double clicked on task: " + taskListView.SelectedItem);
        }

        private void taskListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HumanTask task = (HumanTask)taskListView.SelectedItem;
            if (task == null)
            {
                taskFormFrame.Content = null;
                return;
            }
            try {
                CamundaTaskForm taskFormPage = (CamundaTaskForm)Activator.CreateInstance(Type.GetType(task.formKey));
                taskFormPage.initialize(this, task);
                taskFormFrame.Content = taskFormPage;
            } catch (Exception ex) {
                // Could not load form - maybe no task for .NET tasklist!
                // TODO: Do something?
                taskFormFrame.Content = null;
            }
        }

        private void buttonStartInsuranceApplication_Click(object sender, RoutedEventArgs e)
        {
            // TODO Load from process definition
            ProcessDefinition processDefinition = new ProcessDefinition();
            processDefinition.key = "insuranceApplication.WPF";
            processDefinition.startFormKey = "InsuranceApplicationWpfTasklist.TaskForms.NewInsuranceApplication";

            CamundaStartForm startFormPage = (CamundaStartForm)Activator.CreateInstance(Type.GetType(processDefinition.startFormKey));
            startFormPage.initialize(this, processDefinition);
            taskFormFrame.Content = startFormPage;

        }
    }
}
