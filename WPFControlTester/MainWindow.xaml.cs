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

namespace PackTesterInterface
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            Data = new ObservableCollection<MyCellData>();
            Steps = new ObservableCollection<Step>();

            Data.Add(new MyCellData(1, 3.75));
            Data.Add(new MyCellData(2, 3.41));
            Data.Add(new MyCellData(3, 3.2));
            Data.Add(new MyCellData(4, 3.14));
            Data.Add(new MyCellData(5, 3.54));
            Data.Add(new MyCellData(6, 3.28));
            Data.Add(new MyCellData(7, 3.4));
            Data.Add(new MyCellData(8, 3.4));
            Data.Add(new MyCellData(9, 3.4));
            Data.Add(new MyCellData(10, 3.4));
            Data.Add(new MyCellData(11, 3.4));
            Data.Add(new MyCellData(12, 3.4));
            Data.Add(new MyCellData(13, 3.4));
            Data.Add(new MyCellData(14, 3.4));
            Data.Add(new MyCellData(15, 2.8));
            Data.Add(new MyCellData(16, 3.65));
            Data.Add(new MyCellData(17, 3.4));
            Data.Add(new MyCellData(18, 3.4));
            Data.Add(new MyCellData(19, 3.4));
            Data.Add(new MyCellData(20, 3.4));
            Data.Add(new MyCellData(21, 3.4));
            Data.Add(new MyCellData(22, 3.4));
            Data.Add(new MyCellData(23, 3.4));
            Data.Add(new MyCellData(24, 2.8));

            Steps.Add(new Step() { Name = "Step 1: Discharge" });
            Steps.Add(new Step() { Name = "Step 2: Wait" });
            Steps.Add(new Step() { Name = "Step 3: Charge" });

            FieldsListBox.ItemsSource = Steps;

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
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

        private void MenuItemConnect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetCOMPorts();
        }

        #region Helper Funcions

        void GetCOMPorts()
        {
            //TODO: clear existing menu items?

            string[] comPortArray = SerialPort.GetPortNames();
            foreach (var comPort in comPortArray)
            {
                //TODO: add menu items
            }

            if (comPortArray.Count() == 0)
            {
                //TODO: need to state that no options are available.
            }
        }
        #endregion
    }

    public class Step
    {
        public string Name { get; set; }
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

    #region Converter
    public class Bool2Visibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
                return (Visibility)value == Visibility.Visible ? true : false;
            else
                throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            else
                throw new NotImplementedException();
        }
    }
    #endregion Converter
}
