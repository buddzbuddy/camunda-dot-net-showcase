using System;
using System.Windows;
using System.Windows.Controls;
using Camunda;
using System.ComponentModel;
using InsuranceApplicationWpfTasklist.Tasklist;
using System.Linq;

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
            loadProcessDefinitions();

            Closing += OnWindowClosing;
        }

        private void loadProcessDefinitions()
        {
            var processDefinitions = Camunda.RepositoryService().LoadProcessDefinitions(true);
            processDefinitionListBox.Items.Clear();
            processDefinitionListBox.ItemsSource = processDefinitions.OrderBy(pd => pd.name).ToList(); // add them sorted by name

            processDefinitionListBox.DisplayMemberPath = "name";
        }

        public void reloadTasks()
        {
            var tasks = Camunda.HumanTaskService().LoadTasks();
            taskListView.ItemsSource = tasks.OrderByDescending(task => task.created).ToList(); // add them ordered by creation date
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

        private void showSelectedTask()
        {
            HumanTask task = (HumanTask)taskListView.SelectedItem;
            if (taskListView.SelectedIndex == -1 || task == null || task.formKey == null)
            {
                hideDetails();
                return;
            }
            try
            {
                CamundaTaskForm taskFormPage = (CamundaTaskForm)Activator.CreateInstance(Type.GetType(task.formKey));
                taskFormPage.initialize(this, task);
                showDetails("Task Form", taskFormPage, true);
                showDetails("Task Details", new TaskDetails(task), false);
            }
            catch (Exception ex)
            {
                // Could not load form - maybe no task for .NET tasklist!
                hideDetails();
            }
        }

        private void taskListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showSelectedTask();
        }

        private void buttonStartInsuranceApplication_Click(object sender, RoutedEventArgs e)
        {
            ProcessDefinition processDefinition = (ProcessDefinition)processDefinitionListBox.SelectedValue;
            if (processDefinitionListBox.SelectedIndex == -1 || processDefinition == null || processDefinition.startFormKey == null)
            {
                hideDetails();
                return;
            }
            try
            {
                CamundaStartForm startFormPage = (CamundaStartForm)Activator.CreateInstance(Type.GetType(processDefinition.startFormKey));
                startFormPage.initialize(this, processDefinition);
                showDetails("Start New '" + processDefinition.name + "'", startFormPage, true);
            }
            catch (Exception ex)
            {
                // Could not load form - maybe no form key defined for .NET tasklist!
                hideDetails();
            }
        }

        private void taskListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            showSelectedTask();
        }
    }
}
