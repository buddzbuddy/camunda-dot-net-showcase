using System.Collections.Generic;
using System.Windows.Controls;
using Camunda;
using System;

namespace InsuranceApplicationWpfTasklist.TaskForms
{
    /// <summary>
    /// Interaktionslogik für AntragPruefen.xaml
    /// </summary>
    public partial class Escalate : Page, CamundaTaskForm
    {

        public Dictionary<string, object> Variables { get; set; }

        public Escalate()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void initialize(CamundaClient Camunda, HumanTask task)
        {
            Variables = Camunda.HumanTaskService().LoadVariables(task.id);
            Console.WriteLine(Variables);
        }

    }
}
