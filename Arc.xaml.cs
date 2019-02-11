using System.Windows.Controls;

namespace PetriNetSimu
{
    /// <summary>
    /// Interaction logic for Arc.xaml
    /// </summary>
    public partial class Arc : UserControl
    {
        public UserControl InputFrom { get; private set; }
        public UserControl OutputTo { get; private set; }

        public Arc()
        {
            InitializeComponent();
            arcWeight.Text = "1";
        }        

        public void setInput(UserControl userControl)
        {
            InputFrom = userControl;
        }

        public void setOutput(UserControl userControl)
        {
            OutputTo = userControl;
        }
    }
}