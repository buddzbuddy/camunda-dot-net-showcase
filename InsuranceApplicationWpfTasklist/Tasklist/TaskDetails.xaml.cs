using Camunda;
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

namespace InsuranceApplicationWpfTasklist.Tasklist
{
    /// <summary>
    /// Interaktionslogik für TaskDetails.xaml
    /// </summary>
    public partial class TaskDetails : Page
    {
        public TaskDetails(HumanTask task)
        {
            InitializeComponent();

            taskDetailsListView.Items.Add(new TaskProperty("Task Id", task.id));
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
