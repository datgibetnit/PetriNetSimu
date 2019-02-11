using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace PetriNetSimu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// It includes the functions being executed when pressing the different buttons of MainWindow.xaml and the functions necessary for user-interaction with created objects
    /// </summary>
    public partial class MainWindow : Window
    {
        int MouseOriginX, MouseOriginY, ControlOriginX, ControlOriginY,
            //minimum and maximum value of the numeric up-down-control
            minvalue = 0, maxvalue = 100;
        private List<Transition> AllTransitions = new List<Transition>();
        private List<Place> AllPlaces = new List<Place>();
        private List<Arc> AllArcs = new List<Arc>();
        Transition transitionSelected;
        Place placeSelected;
        Arc arcSelected;
        bool transitionActive = false, placeActive = false, arcActive = false;
        bool foundPlaceTarget = true, foundTransitionTarget = true, changingSelection = false, arcExists = false, hasEnoughToken = true;

        public MainWindow()
        {
            InitializeComponent();
            nameBox.Visibility = Visibility.Hidden;
            numberBox.Visibility = Visibility.Hidden;
            addArc.Visibility = Visibility.Hidden;
            deleteObject.Visibility = Visibility.Hidden;
        }

        private void addPlaceButton_Click(object sender, RoutedEventArgs e)
        {
            Place place = new Place();
            changingSelection = true;
            namingBox.Text = place.placeName.Text;
            nameBox.Visibility = Visibility.Visible;
            NUDTextBox.Text = place.numberToken.Text;
            numberBox.Visibility = Visibility.Visible;
            addArc.Visibility = Visibility.Visible;
            deleteObject.Visibility = Visibility.Visible;
            MainArea.Children.Add(place);
            AllPlaces.Add(place);
            placeSelected = place;
            transitionActive = false;
            placeActive = true;
            arcActive = false;
            place.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.control_MouseDown);
            place.MouseMove += new System.Windows.Input.MouseEventHandler(this.control_MouseMove);
            place.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.control_MouseUp);
        }

        private void addTransitionButton_Click(object sender, RoutedEventArgs e)
        {
            Transition transition = new Transition();            
            changingSelection = true;
            namingBox.Text = transition.transitionName.Text;
            nameBox.Visibility = Visibility.Visible;
            numberBox.Visibility = Visibility.Hidden;
            addArc.Visibility = Visibility.Visible;
            deleteObject.Visibility = Visibility.Visible;
            MainArea.Children.Add(transition);
            AllTransitions.Add(transition);
            transitionSelected = transition;
            transitionActive = true;
            placeActive = false;
            arcActive = false;
            transition.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.control_MouseDown);
            transition.MouseMove += new System.Windows.Input.MouseEventHandler(this.control_MouseMove);
            transition.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.control_MouseUp);
        }

        private void addArc_Click(object sender, RoutedEventArgs e)
        {
            Arc arc = new Arc();
            arcSelected = arc;
            nameBox.Visibility = Visibility.Hidden;
            changingSelection = true;
            NUDTextBox.Text = arc.arcWeight.Text;
            addPlace.Visibility = Visibility.Hidden;
            addTransition.Visibility = Visibility.Hidden;
            numberBox.Visibility = Visibility.Visible;
            addArc.Visibility = Visibility.Hidden;
            deleteObject.Visibility = Visibility.Visible;
            runNet.Visibility = Visibility.Hidden;
            AllArcs.Add(arc);
            Point position = Mouse.GetPosition(MainArea);
            if (transitionActive)
            {
                arc.connection.X1 = Canvas.GetLeft(transitionSelected) + 45;
                arc.connection.Y1 = Canvas.GetTop(transitionSelected) + 60;
                arc.setInput(transitionSelected);
                foundPlaceTarget = false;
            }
            else if (placeActive)
            {
                arc.connection.X1 = Canvas.GetLeft(placeSelected) + 85;
                arc.connection.Y1 = Canvas.GetTop(placeSelected) + 60;
                arc.setInput(placeSelected);
                foundTransitionTarget = false;
            }
            arcActive = true;
            transitionActive = false;
            placeActive = false;
            arc.connection.X2 = position.X;
            arc.connection.Y2 = position.Y;
            Canvas.SetLeft(arc.arcWeight, position.X);
            Canvas.SetTop(arc.arcWeight, position.Y - 20);
            MainArea.Children.Add(arc);
            arc.connection.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.control_MouseDown);          
        }

        private void DeleteControl_Click(object sender, RoutedEventArgs e)
        {
            //check which control is selected and execute the proper delete-commands
            if (placeActive)
            {
                //go through all the Arcs and delete the ones connected to the active Place
                for (int i = AllArcs.Count - 1; i >= 0; i--)
                {
                    if (AllArcs[i].InputFrom == placeSelected || AllArcs[i].OutputTo == placeSelected)
                    {
                        MainArea.Children.Remove(AllArcs[i]);
                        AllArcs.Remove(AllArcs[i]);
                    }
                }
                //go through all the Transitions and remove the connections to the active Place
                for (int i = AllTransitions.Count - 1; i>=0;i--)
                {
                    for (int j = AllTransitions[i].OutputCount() - 1; j >= 0; j--)
                    {
                        if (AllTransitions[i].getOutput(j) == placeSelected)
                        {
                            AllTransitions[i].OutputRemove(AllTransitions[i].getOutput(j));
                        }
                    }
                    for (int j = AllTransitions[i].InputCount() - 1; j >= 0; j--)
                    {
                        if (AllTransitions[i].getInput(j) == placeSelected)
                        {
                            AllTransitions[i].InputRemove(AllTransitions[i].getInput(j));
                        }
                    }
                }
                MainArea.Children.Remove(placeSelected);
                AllPlaces.Remove(placeSelected);
                placeActive = false;
            }
            else if (transitionActive)
            {
                //go through all the Arcs and delete the ones connected to the active Transition
                for (int i = AllArcs.Count - 1; i >= 0; i--)
                {
                    if (AllArcs[i].InputFrom == transitionSelected || AllArcs[i].OutputTo == transitionSelected)
                    {
                        MainArea.Children.Remove(AllArcs[i]);
                        AllArcs.Remove(AllArcs[i]);
                    }
                }
                //go through all the Places and remove the connections to the active Transition
                for (int i = AllPlaces.Count - 1; i >= 0; i--)
                {
                    for (int j = AllPlaces[i].OutputCount() - 1; j >= 0; j--)
                    {
                        if (AllPlaces[i].getOutput(j) == transitionSelected)
                        {
                            AllPlaces[i].OutputRemove(AllPlaces[i].getOutput(j));
                        }
                    }
                    for (int j = AllPlaces[i].InputCount() - 1; j >= 0; j--)
                    {
                        if (AllPlaces[i].getInput(j) == transitionSelected)
                        {
                            AllPlaces[i].InputRemove(AllPlaces[i].getInput(j));
                        }
                    }
                }
                MainArea.Children.Remove(transitionSelected);
                AllTransitions.Remove(transitionSelected);
                transitionActive = false;
            }
            else if (arcActive)
            {
                //in the controls the Arc is connected to, remove the connection
                if (arcSelected.InputFrom is Place)
                {                    
                    placeSelected = arcSelected.InputFrom as Place;
                    if (!(arcSelected.OutputTo == null))
                    {
                        transitionSelected = arcSelected.OutputTo as Transition;
                        for (int i = transitionSelected.InputCount() - 1; i >= 0; i--)
                        {
                            if (transitionSelected.getInput(i) == arcSelected.InputFrom)
                            {
                                transitionSelected.InputRemove(transitionSelected.getInput(i));
                            }
                        }
                    }
                    for (int i = placeSelected.OutputCount() - 1; i >= 0; i--)
                    {
                        if (placeSelected.getOutput(i) == arcSelected.OutputTo)
                        {
                            placeSelected.OutputRemove(placeSelected.getOutput(i));
                        }
                    }
                    foundTransitionTarget = true;
                }
                else if (arcSelected.InputFrom is Transition)
                {
                    transitionSelected = arcSelected.InputFrom as Transition;
                    if (!(arcSelected.OutputTo == null))
                    {
                        placeSelected = arcSelected.OutputTo as Place;
                        for (int i = placeSelected.InputCount() - 1; i >= 0; i--)
                        {
                            if (placeSelected.getInput(i) == arcSelected.InputFrom)
                            {
                                placeSelected.InputRemove(placeSelected.getInput(i));
                            }
                        }
                    }
                    for (int i = transitionSelected.InputCount() - 1; i >= 0; i--)
                    {
                        if (transitionSelected.getOutput(i) == arcSelected.OutputTo)
                        {
                            transitionSelected.OutputRemove(transitionSelected.getOutput(i));
                        }
                    }
                    foundPlaceTarget = true;
                }
                MainArea.Children.Remove(arcSelected);
                AllArcs.Remove(arcSelected);
                arcActive = false;
            }
            addPlace.Visibility = Visibility.Visible;
            addTransition.Visibility = Visibility.Visible;
            addArc.Visibility = Visibility.Hidden;
            deleteObject.Visibility = Visibility.Hidden;
            nameBox.Visibility = Visibility.Hidden;
            numberBox.Visibility = Visibility.Hidden;
        }

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            //if the text changed because another control was selected, do nothing
            if (changingSelection == true)
            {
                changingSelection = false;
            }
            //otherwise update the values of the selected controls' variables
            else if (placeActive)
            {
                placeSelected.placeName.Text = namingBox.Text;
                placeSelected.numberToken.Text = NUDTextBox.Text;
            }
            else if (transitionActive)
            {
                transitionSelected.transitionName.Text = namingBox.Text;
            }
            else if (arcActive)
            {                
                arcSelected.arcWeight.Text = NUDTextBox.Text;
            }
        }

        //The following function is from stackoverflow.com: https://stackoverflow.com/questions/841293/where-is-the-wpf-numeric-updown-control/2752538#2752538
        private void NUDButtonUP_Click(object sender, RoutedEventArgs e)
        {
            int number;
            if (NUDTextBox.Text != "")
            {
                number = Convert.ToInt32(NUDTextBox.Text);
            }
            else
            {
                number = 0;
            }
            if (number < maxvalue)
            {
                NUDTextBox.Text = Convert.ToString(number + 1);
            }
        }

        //The following function is from stackoverflow.com: https://stackoverflow.com/questions/841293/where-is-the-wpf-numeric-updown-control/2752538#2752538
        private void NUDButtonDown_Click(object sender, RoutedEventArgs e)
        {
            int number;
            if (NUDTextBox.Text != "")
            {
                number = Convert.ToInt32(NUDTextBox.Text);
            }
            else
            {
                number = 0;
            }
            if (number > minvalue)
            {
                NUDTextBox.Text = Convert.ToString(number - 1);
            }
        }

        private void runNet_Click(object sender, RoutedEventArgs e)
        {
            addPlace.Visibility = Visibility.Hidden;
            addTransition.Visibility = Visibility.Hidden;
            nameBox.Visibility = Visibility.Hidden;
            numberBox.Visibility = Visibility.Hidden;
            addArc.Visibility = Visibility.Hidden;
            deleteObject.Visibility = Visibility.Hidden;
            runNet.Visibility = Visibility.Hidden;
            foreach (Transition transition in AllTransitions)
            {
                //if the Transition is connected to at least one Place on input and output
                if (transition.InputCount() > 0 && transition.OutputCount() > 0)
                {
                    //check if all the input-places have enough tokens
                    hasEnoughToken = true;
                    for (int i = transition.InputCount() - 1; i >= 0; i--)
                    {
                        foreach (Arc arc in AllArcs)
                        {
                            if (arc.InputFrom == transition.getInput(i) && arc.OutputTo == transition)
                            {
                                if (Convert.ToInt16(arc.arcWeight.Text) > Convert.ToInt16(transition.getInput(i).numberToken.Text))
                                {
                                    hasEnoughToken = false;
                                }
                                break;
                            }
                        }
                        if (!hasEnoughToken)
                        {
                            break;
                        }
                    }
                    //if all the input-places have enough tokens, transition fires and token-numbers change
                    if (hasEnoughToken)
                    {
                        for (int i = transition.InputCount() - 1; i >= 0; i--)
                        {
                            foreach (Arc arc in AllArcs)
                            {
                                if (arc.InputFrom as Place == transition.getInput(i) && arc.OutputTo as Transition == transition)
                                {
                                    transition.getInput(i).numberToken.Text = Convert.ToString(Convert.ToInt16(transition.getInput(i).numberToken.Text) - Convert.ToInt16(arc.arcWeight.Text));
                                    break;
                                }
                            }
                        }

                        for (int i = transition.OutputCount() - 1; i >= 0; i--)
                        {
                            foreach (Arc arc in AllArcs)
                            {
                                if (arc.InputFrom as Transition == transition && arc.OutputTo as Place == transition.getOutput(i))
                                {
                                    transition.getOutput(i).numberToken.Text = Convert.ToString(Convert.ToInt16(transition.getOutput(i).numberToken.Text) + Convert.ToInt16(arc.arcWeight.Text));
                                    break;
                                }
                            }
                        }        
                    }
                }
            }
            addPlace.Visibility = Visibility.Visible;
            addTransition.Visibility = Visibility.Visible;
            if (transitionActive || placeActive)
            {
                nameBox.Visibility = Visibility.Visible;
                addArc.Visibility = Visibility.Visible;
            }            
            if (arcActive || placeActive)
            {
                numberBox.Visibility = Visibility.Visible;
            }
            if (arcActive || placeActive || transitionActive)
            {
                deleteObject.Visibility = Visibility.Visible;
            }
            runNet.Visibility = Visibility.Visible;
        }

        private void control_MouseDown(object sender, MouseEventArgs e)
        {
            //if an Arc is currently looking for a target
            if (!foundTransitionTarget || !foundPlaceTarget)
            {
                //if an Arc is looking for a Transition as target and a Transition was clicked
                if (!foundTransitionTarget && sender is Transition)
                {
                    //check if there already is an Arc with the same input and output
                    arcExists = false;
                    for (int i = placeSelected.OutputCount() - 1; i >= 0; i--)
                    {
                        if (placeSelected.getOutput(i) == sender as Transition)
                        {
                            arcExists = true;
                            break;
                        }
                    }
                    //if there is no Arc with the same input and output
                    if (!arcExists)
                    {
                        arcSelected.setOutput(sender as Transition);
                        transitionSelected = sender as Transition;
                        transitionSelected.InputAdd(arcSelected.InputFrom as Place);
                        placeSelected.OutputAdd(sender as Transition);
                        changingSelection = true;
                        namingBox.Text = transitionSelected.transitionName.Text;
                        addPlace.Visibility = Visibility.Visible;
                        addTransition.Visibility = Visibility.Visible;
                        nameBox.Visibility = Visibility.Visible;
                        numberBox.Visibility = Visibility.Hidden;
                        addArc.Visibility = Visibility.Visible;
                        deleteObject.Visibility = Visibility.Visible;
                        runNet.Visibility = Visibility.Visible;
                        transitionActive = true;
                        placeActive = false;
                        arcActive = false;
                    }
                }
                //if an Arc is looking for a Place as target and a Place was clicked
                if (!foundPlaceTarget && sender is Place)
                {
                    //check if there already is an Arc with the same input and output
                    arcExists = false;
                    for (int i = transitionSelected.OutputCount() - 1; i >= 0; i--)
                    {
                        if (transitionSelected.getOutput(i) == sender as Place)
                        {
                            arcExists = true;
                            break;
                        }
                    }
                    //if there is no Arc with the same input and output
                    if (!arcExists)
                    {
                        arcSelected.setOutput(sender as Place);
                        placeSelected = sender as Place;
                        placeSelected.InputAdd(arcSelected.InputFrom as Transition);
                        transitionSelected.OutputAdd(sender as Place);
                        changingSelection = true;
                        namingBox.Text = placeSelected.placeName.Text;
                        changingSelection = true;
                        NUDTextBox.Text = placeSelected.numberToken.Text;
                        addPlace.Visibility = Visibility.Visible;
                        addTransition.Visibility = Visibility.Visible;
                        nameBox.Visibility = Visibility.Visible;
                        numberBox.Visibility = Visibility.Visible;
                        addArc.Visibility = Visibility.Visible;
                        deleteObject.Visibility = Visibility.Visible;
                        runNet.Visibility = Visibility.Visible;
                        placeActive = true;
                        transitionActive = false;
                        arcActive = false;
                    }
                }
            }
            //if an Arc is not currently looking for a target and a Place was clicked
            else if (sender is Place)
            {                
                placeSelected = sender as Place;
                changingSelection = true;
                namingBox.Text = placeSelected.placeName.Text;
                changingSelection = true;
                NUDTextBox.Text = placeSelected.numberToken.Text;
                nameBox.Visibility = Visibility.Visible;
                numberBox.Visibility = Visibility.Visible;
                addArc.Visibility = Visibility.Visible;
                deleteObject.Visibility = Visibility.Visible;
                placeActive = true;
                transitionActive = false;
                arcActive = false;
            }
            //if an Arc is not currently looking for a target and a Transition was clicked
            else if (sender is Transition)
            {
                transitionSelected = sender as Transition;
                changingSelection = true;
                namingBox.Text = transitionSelected.transitionName.Text;
                nameBox.Visibility = Visibility.Visible;
                numberBox.Visibility = Visibility.Hidden;
                addArc.Visibility = Visibility.Visible;
                deleteObject.Visibility = Visibility.Visible;
                transitionActive = true;
                placeActive = false;
                arcActive = false;
            }
            //if an Arc is not currently looking for a target and an Arc was clicked
            else if (sender is Line)
            {
                //find the Arc that was clicked
                foreach (Arc arc in AllArcs)
                {
                    if (sender as Line == arc.connection)
                    {
                        arcSelected = arc;
                        break;
                    }
                }
                changingSelection = true;
                NUDTextBox.Text = arcSelected.arcWeight.Text;
                nameBox.Visibility = Visibility.Hidden;
                numberBox.Visibility = Visibility.Visible;
                addArc.Visibility = Visibility.Hidden;
                deleteObject.Visibility = Visibility.Visible;
                transitionActive = false;
                placeActive = false;
                arcActive = true;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //save coordinates of the click
                Point position = Mouse.GetPosition(MainArea);
                MouseOriginX = (int)Convert.ChangeType(position.X, typeof(int));
                MouseOriginY = (int)Convert.ChangeType(position.Y, typeof(int));
                //bring the clicked object into the foreground and save its original coordinates
                if (placeActive)
                {
                    Canvas.SetZIndex(placeSelected, 1);
                    ControlOriginX = (int)Convert.ChangeType(Canvas.GetLeft(placeSelected), typeof(int));
                    ControlOriginY = (int)Convert.ChangeType(Canvas.GetTop(placeSelected), typeof(int));
                }
                else if (transitionActive)
                {
                    Canvas.SetZIndex(transitionSelected, 1);
                    ControlOriginX = (int)Convert.ChangeType(Canvas.GetLeft(transitionSelected), typeof(int));
                    ControlOriginY = (int)Convert.ChangeType(Canvas.GetTop(transitionSelected), typeof(int));
                }
            }
        }

        //used when moving the mouse while an output for an Arc is being searched for
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = Mouse.GetPosition(MainArea);
            if (arcSelected != null)
            {
                //if an Arc is searching for a Place as Output
                if (!foundPlaceTarget)
                {
                    //update its coordinates
                    arcSelected.connection.X2 = position.X;
                    arcSelected.connection.Y2 = position.Y;
                    Canvas.SetLeft(arcSelected.arcWeight, (arcSelected.connection.X2 - arcSelected.connection.X1) / 2 + arcSelected.connection.X1);
                    Canvas.SetTop(arcSelected.arcWeight, (arcSelected.connection.Y2 - arcSelected.connection.Y1) / 2 + arcSelected.connection.Y1 - 30);
                    //if a target was found
                    if (placeActive)
                    {
                        //set it as output and update the Arcs' end coordinates to its input
                        arcSelected.setOutput(placeSelected);
                        foundPlaceTarget = true;
                        arcSelected.connection.X2 = Canvas.GetLeft(placeSelected);
                        arcSelected.connection.Y2 = Canvas.GetTop(placeSelected) + 60;
                        Canvas.SetLeft(arcSelected.arcWeight, (arcSelected.connection.X2 - arcSelected.connection.X1) / 2 + arcSelected.connection.X1);
                        Canvas.SetTop(arcSelected.arcWeight, (arcSelected.connection.Y2 - arcSelected.connection.Y1) / 2 + arcSelected.connection.Y1 - 30);
                    }
                }
                //if an Arc is searching for a Transition as Output
                else if (!foundTransitionTarget)
                {
                    //update its coordinates
                    arcSelected.connection.X2 = position.X;
                    arcSelected.connection.Y2 = position.Y;
                    Canvas.SetLeft(arcSelected.arcWeight, (arcSelected.connection.X2 - arcSelected.connection.X1) / 2 + arcSelected.connection.X1);
                    Canvas.SetTop(arcSelected.arcWeight, (arcSelected.connection.Y2 - arcSelected.connection.Y1) / 2 + arcSelected.connection.Y1 - 30);
                    //if a target was found
                    if (transitionActive)
                    {
                        //set it as output and update the Arcs' end coordinates to its input
                        arcSelected.setOutput(transitionSelected);
                        foundTransitionTarget = true;
                        arcSelected.connection.X2 = Canvas.GetLeft(transitionSelected);
                        arcSelected.connection.Y2 = Canvas.GetTop(transitionSelected) + 60;
                        Canvas.SetLeft(arcSelected.arcWeight, (arcSelected.connection.X2 - arcSelected.connection.X1) / 2 + arcSelected.connection.X1);
                        Canvas.SetTop(arcSelected.arcWeight, (arcSelected.connection.Y2 - arcSelected.connection.Y1) / 2 + arcSelected.connection.Y1 - 30);
                    }
                }
            }            
        }

        //used when moving around Places and Transitions
        private void control_MouseMove(object sender, MouseEventArgs e)
        {
            UserControl p = sender as UserControl;
            Point position = Mouse.GetPosition(MainArea);
            if (p != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    //update the coordinates of the control itself
                    Canvas.SetLeft(p, (int)Convert.ChangeType(position.X - MouseOriginX + ControlOriginX, typeof(int)));
                    Canvas.SetTop(p, (int)Convert.ChangeType(position.Y - MouseOriginY + ControlOriginY, typeof(int)));
                    //update the coordinates of the connected Arcs
                    if (transitionActive)
                    {
                        foreach (Arc arc in AllArcs)
                        {
                            if (arc.InputFrom as Transition == transitionSelected)
                            {
                                arc.connection.X1 = Canvas.GetLeft(transitionSelected) + 45;
                                arc.connection.Y1 = Canvas.GetTop(transitionSelected) + 60;
                                Canvas.SetLeft(arc.arcWeight, (arc.connection.X2 - arc.connection.X1) / 2 + arc.connection.X1);
                                Canvas.SetTop(arc.arcWeight, (arc.connection.Y2 - arc.connection.Y1) / 2 + arc.connection.Y1 - 30);
                            }
                            if (arc.OutputTo as Transition == transitionSelected)
                            {
                                arc.connection.X2 = Canvas.GetLeft(transitionSelected);
                                arc.connection.Y2 = Canvas.GetTop(transitionSelected) + 60;
                                Canvas.SetLeft(arc.arcWeight, (arc.connection.X2 - arc.connection.X1) / 2 + arc.connection.X1);
                                Canvas.SetTop(arc.arcWeight, (arc.connection.Y2 - arc.connection.Y1) / 2 + arc.connection.Y1 - 30);
                            }
                        }
                    }
                    if (placeActive)
                    {
                        foreach (Arc arc in AllArcs)
                        {
                            if (arc.InputFrom as Place == placeSelected)
                            {
                                arc.connection.X1 = Canvas.GetLeft(placeSelected) + 85;
                                arc.connection.Y1 = Canvas.GetTop(placeSelected) + 60;
                                Canvas.SetLeft(arc.arcWeight, (arc.connection.X2 - arc.connection.X1) / 2 + arc.connection.X1);
                                Canvas.SetTop(arc.arcWeight, (arc.connection.Y2 - arc.connection.Y1) / 2 + arc.connection.Y1 - 30);
                            }
                            if (arc.OutputTo as Place == placeSelected)
                            {
                                arc.connection.X2 = Canvas.GetLeft(placeSelected);
                                arc.connection.Y2 = Canvas.GetTop(placeSelected) + 60;
                                Canvas.SetLeft(arc.arcWeight, (arc.connection.X2 - arc.connection.X1) / 2 + arc.connection.X1);
                                Canvas.SetTop(arc.arcWeight, (arc.connection.Y2 - arc.connection.Y1) / 2 + arc.connection.Y1 - 30);
                            }
                        }
                    }
                }
            }
        }

        private void control_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                UserControl p = sender as UserControl;
                Canvas.SetZIndex(p, 0);
            }
        }      
    }
}