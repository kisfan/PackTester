using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.IO.Ports;
using System.Globalization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace PackTesterInterface
{
    public partial class MainWindow : Window
    {
        static SerialPort serialPort;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            Data = new ObservableCollection<MyCellData>();
            Steps = new ObservableCollection<Step>();

            //temporarily fill 24 cells with data
            for (int k = 1; k <= 24; k++)
            {
                Data.Add(new MyCellData(k, 3.2));
            }

            //create some fake steps
            Steps.Add(new Step() { Name = "Step 1: Discharge", Status = stepStatus.Complete });
            Steps.Add(new Step() { Name = "Step 2: Wait", Status = stepStatus.Selected });
            Steps.Add(new Step() { Name = "Step 3: Charge", Status = stepStatus.Incomplete });

            //Bind Steps StackPanel to steps list
            FieldsListBox.ItemsSource = Steps;

            //Timer & stopwatch
            Stopwatch stopWatch;
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;

            //TODO: Move this to whatever button starts a test
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            //Theading
            System.Threading.Thread processThread;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Random r = new Random();
            for (int k = 0; k < 24; k++)
            {
                Data[k] = new MyCellData(k, r.Next(280, 365) / 100.0);
            }
            BarChart1.Refresh();
        }

        private ObservableCollection<MyCellData> _data = null;
        public ObservableCollection<MyCellData> Data
        {
            get 
            { 
                return _data; 
            }
            set
            {
                _data = value;
                Notify("Data");
            }
        }

        private ObservableCollection<Step> _steps = null;
        public ObservableCollection<Step> Steps
        {
            get
            {
                return _steps;
            }
            set
            {
                _steps = value;
                Notify("Steps");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void ConnectToSerial(string comName)
        {
            serialPort = new SerialPort(comName, 9600);

            serialPort.Open();
            if (serialPort.IsOpen)
            {
                //We have successfully connected. 
                //TODO: Change menu to say "Disconnect", and have no sub-menu items.

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetCOMPorts();
        }

        #region Helper Funcions

        void GetCOMPorts()
        {
            //TODO: clear existing menu items?

            //Add to a sub item
            MenuItem comMenuItem = new MenuItem();
            MenuItem File = (MenuItem)this.MainMenu.Items[0];
            MenuItem Connect = (MenuItem)File.Items[0];

            string[] comPortArray = SerialPort.GetPortNames();
            foreach (var comPort in comPortArray)
            {
                comMenuItem.Header = comPort;
                comMenuItem.Click += ComMenuItem_Click;
                Connect.Items.Add(comMenuItem);
            }

            if (comPortArray.Count() == 0)
            {
                //TODO: need to state that no options are available.
            }
        }

        private void ComMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string comName = ((MenuItem)sender).Header.ToString();
            ConnectToSerial(comName);
        }
        #endregion
    }

    public enum stepStatus
    {
        Complete,
        Incomplete,
        Selected,
    };

    public class Step
    {
        public string Name { get; set; }
        public stepStatus Status { get; set; }
    }

    public class MyCellData
    {
        private int cell;
        private double voltage;

        public MyCellData(int cell, double voltage)
        {
            this.cell = cell;
            this.voltage = voltage;
        }

        public int Cell {
            get { return cell; }
            set { cell = value; }
        }

        public double Voltage
        {
            get { return voltage; }
            set { voltage = value; }
        }
    }

    #region Converters
    public class StepStatusToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((stepStatus)value == stepStatus.Incomplete)
            {
                return "#F2F2F2";
            }
            else if ((stepStatus)value == stepStatus.Selected)
            {
                return "#C2F0D9";
            }

            return "#F6EEEE";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion Converter
}
