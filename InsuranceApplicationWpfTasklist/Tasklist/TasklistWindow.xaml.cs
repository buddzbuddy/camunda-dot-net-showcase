using System;
using System.Windows;
using System.Windows.Controls;
using Camunda;
using System.ComponentModel;
using System.Reflection;
using InsuranceApplicationWpfTasklist.Tasklist;

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

        public void showDetails(string name, object content, Boolean firstTab) 
        {
            if (firstTab)
            {
                taskFormTabControl.Items.Clear();
            }

            var frame = new Frame();
            frame.Content = content;

            var tab = new TabItem();
            tab.FontSize = 16;
            tab.Header = name;
            tab.Content = frame;

            taskFormTabControl.Items.Add(tab);
            taskFormTabControl.Visibility = Visibility.Visible;
            if (firstTab)
            {
                taskFormTabControl.SelectedIndex = 0;
            }
        }

        public void hideDetails()
        {
            taskFormTabControl.Items.Clear();
            taskFormTabControl.Visibility = Visibility.Hidden;
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
                showDetails("Task Form", taskFormPage, true);
                showDetails("Task Details", new TaskDetails(task), false);
            } catch (Exception ex) {
                // Could not load form - maybe no task for .NET tasklist!
                hideDetails();
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
            showDetails("Start New Process Instance Form", startFormPage, true);
        }
    }
}
