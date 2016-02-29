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
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;

namespace KeyhanControls.Chart
{
    [System.ComponentModel.DefaultProperty("Legends")]
    public partial class BarChart : UserControl
    {
        public BarChart()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #region Dependency properties
        public static readonly DependencyProperty VerticalPropertyNameProperty = DependencyProperty.Register("VerticalPropertyName", typeof(string), typeof(BarChart));
        public string VerticalPropertyName
        {
            get { return GetValue(VerticalPropertyNameProperty) == null ? string.Empty : GetValue(VerticalPropertyNameProperty).ToString(); }
            set
            {
                SetValue(VerticalPropertyNameProperty, value);
            }
        }

        public static readonly DependencyProperty HorizontalPropertyNameProperty = DependencyProperty.Register("HorizontalPropertyName", typeof(string), typeof(BarChart));
        public string HorizontalPropertyName
        {
            get { return GetValue(HorizontalPropertyNameProperty) == null ? string.Empty : GetValue(HorizontalPropertyNameProperty).ToString(); }
            set
            {
                SetValue(HorizontalPropertyNameProperty, value);
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(BarChart),
            new FrameworkPropertyMetadata() { PropertyChangedCallback = ItemsSourceChangedCallBack });
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
                IsDataSourceBinded = value != null;
                Refresh();
            }
        }

        protected static void ItemsSourceChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as BarChart).ItemsSource = (IEnumerable)e.NewValue;
        }

        #endregion Dependency properties

        #region Properties
        private bool _isDataSourceBinded = false;
        public bool IsDataSourceBinded
        {
            get { return _isDataSourceBinded; }
            private set { _isDataSourceBinded = value; }
        }

        private IEnumerable _items = null;
        public IEnumerable Items
        {
            get
            {
                if (IsDataSourceBinded)
                    return ItemsSource;
                else
                    return _items;
            }
            set
            {
                if (_items != value)
                {
                    if (IsDataSourceBinded)
                        throw new Exception("Control is in DataSource mode.");

                    _items = value;
                    Notify("Items");
                    Draw();
                }
            }
        }
        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Methods
        private void Notify(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void Draw()
        {
            Draw(true);
        }

        private void Draw(bool UpdateLegends)
        {
            cnvMain.Children.Clear();

            double VertLineHorMargin = 40;
            double HorLineVertMargin = 25;
            double LegendsHorMargin = 1;

            // Drawing Main lines
            Line vLine = new Line();
            vLine.X1 = VertLineHorMargin;
            vLine.X2 = vLine.X1;
            vLine.Y1 = 10;
            vLine.Y2 = cnvMain.ActualHeight - 10;

            Line hLine = new Line();
            hLine.X1 = 10;
            hLine.X2 = cnvMain.ActualWidth - 10;
            hLine.Y1 = cnvMain.ActualHeight - HorLineVertMargin;
            hLine.Y2 = hLine.Y1;

            //cnvMain.Children.Add(vLine);
            //cnvMain.Children.Add(hLine);

            /////////////////////
            ArrayList tmpItems = new ArrayList();
            if (Items != null)
                foreach (object item in Items)
                    tmpItems.Add(item);

            if (tmpItems.Count == 0 || String.IsNullOrEmpty(VerticalPropertyName) || String.IsNullOrEmpty(HorizontalPropertyName))
                return;

            if (tmpItems[0].GetType().GetProperty(VerticalPropertyName) == null)
                throw new ArgumentException("VerticalPropertyName is not correct.");

            if (tmpItems[0].GetType().GetProperty(HorizontalPropertyName) == null)
                throw new ArgumentException("HorizontalPropertyName is not correct.");

            tmpItems.Sort(new ItemsComparer(HorizontalPropertyName));

            List<double> HorValues = new List<double>();
            double MaxValue = 0;

            foreach (var item in tmpItems)
            {
                var VertValue = item.GetType().GetProperty(VerticalPropertyName).GetValue(item, null);
                var HorValue = item.GetType().GetProperty(HorizontalPropertyName).GetValue(item, null);

                if (!HorValues.Exists(i => i == Convert.ToDouble(HorValue)))
                    HorValues.Add(Convert.ToDouble(HorValue));

                if (Convert.ToDouble(VertValue) > MaxValue)
                    MaxValue = Convert.ToDouble(VertValue);
            }

            if (MaxValue < 3.65)
                MaxValue = 3.65;

            if (cnvMain.ActualWidth == 0)
                return;

            double DrawingAreaWidth = (cnvMain.ActualWidth - VertLineHorMargin);
            double MaxValueTopMargin = 10 + 20;

            Line lMax = new Line();
            lMax.StrokeDashArray = new DoubleCollection() { 2 };
            lMax.X1 = VertLineHorMargin - 5;
            lMax.X2 = hLine.X2;
            lMax.Y1 = MaxValueTopMargin;
            lMax.Y2 = lMax.Y1;
            //cnvMain.Children.Add(lMax);

            Line lHigh = new Line();
            lHigh.StrokeDashArray = new DoubleCollection() { 2 };
            lHigh.X1 = lMax.X1;
            lHigh.X2 = lMax.X2;
            lHigh.Y1 = hLine.Y1 - ((hLine.Y1 - lMax.Y1) * 3.65 / MaxValue);
            lHigh.Y2 = lHigh.Y1;
            cnvMain.Children.Add(lHigh);

            Line lLow = new Line();
            lLow.StrokeDashArray = new DoubleCollection() { 2 };
            lLow.X1 = lMax.X1;
            lLow.X2 = lMax.X2;
            lLow.Y1 = hLine.Y1 - ((hLine.Y1 - lMax.Y1) * 2.8 / MaxValue); 
            lLow.Y2 = lLow.Y1;
            cnvMain.Children.Add(lLow);

            TextBlock tbMax = new TextBlock();
            tbMax.Text = (3.65).ToString();
            var formattedMaxText = new FormattedText(tbMax.Text,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface(tbMax.FontFamily, tbMax.FontStyle, tbMax.FontWeight, tbMax.FontStretch),
                    tbMax.FontSize,
                    Brushes.Black);
            Canvas.SetLeft(tbMax, VertLineHorMargin - formattedMaxText.Width - 10);
            Canvas.SetTop(tbMax, lMax.Y1 - formattedMaxText.Height / 2.0);
            cnvMain.Children.Add(tbMax);

            TextBlock tbLow = new TextBlock();
            tbLow.Text = (2.8).ToString();
            var formattedLowText = new FormattedText(tbLow.Text,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface(tbLow.FontFamily, tbLow.FontStyle, tbLow.FontWeight, tbLow.FontStretch),
                    tbLow.FontSize,
                    Brushes.Black);
            Canvas.SetLeft(tbLow, VertLineHorMargin - formattedLowText.Width - 10);
            Canvas.SetTop(tbLow, lLow.Y1 - formattedLowText.Height / 2.0);
            cnvMain.Children.Add(tbLow);

            double BarsWidth = (DrawingAreaWidth - (HorValues.Count * LegendsHorMargin)) / HorValues.Count;
            if (Double.IsInfinity(BarsWidth) || Double.IsNaN(BarsWidth))
                BarsWidth = 0;

            double HorItemWidth = Math.Ceiling((DrawingAreaWidth - (HorValues.Count * LegendsHorMargin)) / HorValues.Count);

            for (int i = 0; i < HorValues.Count; i++)
            {
                Line l = new Line();
                l.X1 = (HorItemWidth * i) + VertLineHorMargin + BarsWidth/ 2.0;
                l.X2 = l.X1;
                l.Y1 = hLine.Y1;
                l.Y2 = l.Y1 + 5;
                cnvMain.Children.Add(l);

                TextBlock tb = new TextBlock();
                tb.Text = HorValues[i].ToString();
                var formattedText = new FormattedText(tb.Text,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
                    tb.FontSize,
                    Brushes.Black);
                Canvas.SetLeft(tb, l.X1 - (formattedText.Width / 2));
                Canvas.SetTop(tb, l.Y2 + 5);
                cnvMain.Children.Add(tb);
            }

            foreach (double HorizontalIndex in HorValues)
            {
                var items = from object item in tmpItems
                            where Convert.ToDouble(item.GetType().GetProperty(HorizontalPropertyName).GetValue(item, null)) == HorizontalIndex
                            select item;

                int LegendValueIndex = 0;
                foreach (object item in items)
                {
                    var VerValue = item.GetType().GetProperty(VerticalPropertyName).GetValue(item, null);
                    var HorValue = item.GetType().GetProperty(HorizontalPropertyName).GetValue(item, null);

                    int HorizontalValueIndex = HorValues.IndexOf(Convert.ToDouble(HorValue));

                    double BarLeft = (HorItemWidth * HorizontalValueIndex) + LegendsHorMargin +
                        VertLineHorMargin +
                        (LegendValueIndex * BarsWidth);

                    Border b = new Border();
                    b.Style = (Style)cnvMain.FindResource("BarStyle");
                    b.Width = BarsWidth;
                    b.Height = Convert.ToDouble(VerValue) * (hLine.Y1 - lMax.Y1) / MaxValue;

                    b.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#eeffcc"));
                    Canvas.SetLeft(b, BarLeft);
                    var top = hLine.Y1 - b.Height;
                    Canvas.SetTop(b, top);
                    cnvMain.Children.Add(b);

                    TextBlock tbValue = new TextBlock();
                    Panel.SetZIndex(tbValue, 100);
                    tbValue.Text = VerValue.ToString();
                    Binding binding = new Binding("ValueVisibility");
                    binding.Source = this;
                    tbValue.SetBinding(TextBlock.VisibilityProperty, binding);
                    var formattedText = new FormattedText(tbValue.Text,
                        CultureInfo.CurrentUICulture,
                        FlowDirection.LeftToRight,
                        new Typeface(tbValue.FontFamily, tbValue.FontStyle, tbValue.FontWeight, tbValue.FontStretch),
                        tbValue.FontSize,
                        Brushes.Black);
                    Canvas.SetLeft(tbValue, BarLeft + (((BarLeft + BarsWidth) - BarLeft) / 2 - formattedText.Width / 2));
                    Canvas.SetTop(tbValue, hLine.Y1 - b.Height - formattedText.Height - 5);
                    cnvMain.Children.Add(tbValue);

                    LegendValueIndex++;
                }
            }
        }

        public void Refresh()
        {
            Draw();
        }

        private void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Refresh();
        }
        #endregion Methods

        #region Comparer for sorting items
        class ItemsComparer : IComparer
        {
            #region Constructors
            public ItemsComparer(string HorPropertyName)
                : base()
            {
                HorizontalPropertyName = HorPropertyName;
            }
            #endregion Constructors

            #region Properties
            private string HorizontalPropertyName = String.Empty;
            #endregion Properties

            #region Methods
            public int Compare(object x, object y)
            {
                if (String.IsNullOrEmpty(HorizontalPropertyName))
                    throw new ArgumentException();

                var xHorValue = Convert.ToDouble(x.GetType().GetProperty(HorizontalPropertyName).GetValue(x, null));
                var yHorValue = Convert.ToDouble(y.GetType().GetProperty(HorizontalPropertyName).GetValue(y, null));

                return xHorValue.CompareTo(yHorValue);
            }
            #endregion Methods
        }
        #endregion Comparer for sorting items
    }
}
