using System.Collections.Generic;
using System.Windows.Controls;

namespace PetriNetSimu
{
    /// <summary>
    /// Interaction logic for Place.xaml
    /// </summary>
    public partial class Place : UserControl
    {
        public static int PCounter { get; private set; }
        private List<Transition> InputFrom = new List<Transition>();
        private List<Transition> OutputTo = new List<Transition>();

        public Place()
        {
            InitializeComponent();
            IncreasePCounter();
            placeName.Text = "Place" + Place.PCounter;
            numberToken.Text = "1";
            Canvas.SetLeft(this, 50);
            Canvas.SetTop(this, 50);
            Canvas.SetZIndex(this, 0);
        }

        public void IncreasePCounter()
        {
            PCounter++;
        }

        public int InputCount()
        {
            return InputFrom.Count;
        }

        public int OutputCount()
        {
            return OutputTo.Count;
        }

        public Transition getInput(int i)
        {
            if (i <= InputCount() - 1)
            {
                return InputFrom[i];
            }
            else
            {
                return null;
            }
        }

        public Transition getOutput(int i)
        {
            if (i <= OutputCount() - 1)
            {
                return OutputTo[i];
            }
            else
            {
                return null;
            }
        }

        public void InputAdd(Transition transition)
        {
            InputFrom.Add(transition);
        }

        public void OutputAdd(Transition transition)
        {
            OutputTo.Add(transition);
        }

        public void InputRemove(Transition transition)
        {
            for (int j = InputFrom.Count - 1; j >= 0; j--)
            {
                if (InputFrom[j] == transition)
                {
                    InputFrom.Remove(InputFrom[j]);
                }
            }
        }

        public void OutputRemove(Transition transition)
        {
            for (int j = OutputTo.Count - 1; j >= 0; j--)
            {
                if (OutputTo[j] == transition)
                {
                    OutputTo.Remove(OutputTo[j]);
                }
            }
        }
    }
}
