using System.Collections.Generic;
using System.Windows.Controls;

namespace PetriNetSimu
{
    /// <summary>
    /// Interaction logic for Transition.xaml
    /// </summary>
    public partial class Transition : UserControl
    {
        public static int TCounter { get; private set; }
        private List<Place> InputFrom = new List<Place>();
        private List<Place> OutputTo = new List<Place>();

        public Transition()
        {
            InitializeComponent();
            IncreaseTCounter();
            transitionName.Text = "Transition" + Transition.TCounter;
            Canvas.SetLeft(this, 50);
            Canvas.SetTop(this, 50);
            Canvas.SetZIndex(this, 0);
        }

        public void IncreaseTCounter()
        {
            TCounter++;
        }

        public int InputCount()
        {
            return InputFrom.Count;
        }

        public int OutputCount()
        {
            return OutputTo.Count;
        }

        public Place getInput(int i)
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

        public Place getOutput(int i)
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

        public void InputAdd(Place place)
        {
            InputFrom.Add(place);
        }

        public void OutputAdd(Place place)
        {
            OutputTo.Add(place);
        }

        public void InputRemove(Place place)
        {
            for (int j = InputFrom.Count - 1; j >= 0; j--)
            {
                if (InputFrom[j] == place)
                {
                    InputFrom.Remove(InputFrom[j]);
                }
            }
        }

        public void OutputRemove(Place place)
        {
            for (int j = OutputTo.Count - 1; j >= 0; j--)
            {
                if (OutputTo[j] == place)
                {
                    OutputTo.Remove(OutputTo[j]);
                }
            }
        }
    }
}
