﻿using System;
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
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using POS.Entities;
using POS.Repository.DAL;

namespace POS.AdminWorkSpace
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private static int FILL_ALL = 0;
        private static int FILL_BY_DAY = 1;
        private static int FILL_BY_MONTH = 2;


        private AdminwsOfAsowell _unitofwork;

        public Func<ChartPoint, string> PointLabel { get; set; }
        public List<decimal> PriceList;
        public SeriesCollection SeriesCollection { get; set; }
        private List<PieSeries> EmpPieSeries { get; set; }

        public SeriesCollection SeriesCollectionTime { get; set; }
        public PieSeries FirstPieSeries { get; set; }
        public PieSeries SecondPieSeries { get; set; }
        public PieSeries ThirdPieSeries { get; set; }



        public HomePage(AdminwsOfAsowell unitofwork)
        {
            InitializeComponent();
            _unitofwork = unitofwork;
            InitializeComponent();

            // init datasource for Time PieChart
            SeriesCollectionTime = new SeriesCollection();
            PriceList = new List<decimal>();
            FirstPieSeries = new PieSeries()
            {
                Title = "0h-12h"
            };
            SecondPieSeries = new PieSeries()
            {
                Title = "12h-18h"
            };
            ThirdPieSeries = new PieSeries()
            {
                Title = "18h-0h"
            };
            SeriesCollectionTime.Add(FirstPieSeries);
            SeriesCollectionTime.Add(SecondPieSeries);
            SeriesCollectionTime.Add(ThirdPieSeries);


            // init datasource for Employee PieChart
            SeriesCollection = new SeriesCollection();
            EmpPieSeries = new List<PieSeries>();
            foreach (var item in _unitofwork.EmployeeRepository.Get())
            {
                EmpPieSeries.Add(new PieSeries() { Title = item.EmpId + ": " + item.Name });
            }
            foreach (var item in EmpPieSeries)
            {
                SeriesCollection.Add(item);
            }


            // fill chart at first time
            ChartDataFilling(FILL_ALL);
            ChartDataFillingByTime(FILL_ALL);

        }

        private void ChartDataFillingByTime(int filter)
        {
            // filter data
            List<OrderNote> orderNoteWithTime = new List<OrderNote>();
            if (filter == FILL_BY_DAY)
            {
                orderNoteWithTime = _unitofwork.OrderRepository.Get(c => c.Ordertime.Day == DateTime.Now.Day).ToList();
            }
            else if (filter == FILL_BY_MONTH)
            {
                orderNoteWithTime = _unitofwork.OrderRepository.Get(c => c.Ordertime.Month == DateTime.Now.Month).ToList();
            }
            else
            {
                orderNoteWithTime = _unitofwork.OrderRepository.Get().ToList();
            }


            // calculate data
            decimal TotalPrice1 = 0;
            decimal TotalPrice2 = 0;
            decimal TotalPrice3 = 0;
            foreach (var item in orderNoteWithTime.Where(c => c.Ordertime.Hour >= 0 && c.Ordertime.Hour < 12))
            {
                TotalPrice1 += item.TotalPrice;
            }
            foreach (var item in orderNoteWithTime.Where(c => c.Ordertime.Hour >= 12 && c.Ordertime.Hour < 18))
            {
                TotalPrice2 += item.TotalPrice;
            }
            foreach (var item in orderNoteWithTime.Where(c =>
                c.Ordertime.Hour >= 18 && (c.Ordertime.Hour <= 23 && c.Ordertime.Minute <= 59)))
            {
                TotalPrice3 += item.TotalPrice;
            }


            // binding
            FirstPieSeries.Values = new ChartValues<ObservableValue> { new ObservableValue((double)TotalPrice1) };
            FirstPieSeries.DataLabels = true;

            SecondPieSeries.Values = new ChartValues<ObservableValue> { new ObservableValue((double)TotalPrice2) };
            SecondPieSeries.DataLabels = true;

            ThirdPieSeries.Values = new ChartValues<ObservableValue> { new ObservableValue((double)TotalPrice3) };
            ThirdPieSeries.DataLabels = true;

        }

        private void ChartDataFilling(int filter)
        {

            List<OrderNote> orderNoteWithTime = new List<OrderNote>();
            if (filter == FILL_BY_DAY)
            {
                orderNoteWithTime = _unitofwork.OrderRepository.Get(c => c.Ordertime.Day == DateTime.Now.Day).ToList();
            }
            else if (filter == FILL_BY_MONTH)
            {
                orderNoteWithTime = _unitofwork.OrderRepository.Get(c => c.Ordertime.Month == DateTime.Now.Month)
                    .ToList();
            }
            else
            {
                orderNoteWithTime = _unitofwork.OrderRepository.Get().ToList();
            }


            decimal count = 0;
            foreach (var itemserie in EmpPieSeries)
            {
                string[] data = itemserie.Title.Split(':');
                string empId = data[0];

                foreach (var item2 in orderNoteWithTime.Where(c => c.EmpId.Equals(empId)))

                {
                    count += item2.TotalPrice;
                }
                itemserie.Values = new ChartValues<ObservableValue> {new ObservableValue((double) count)};
                itemserie.DataLabels = true;
                count = 0;
            }

        }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }


        private void RdToday_OnClick(object sender, RoutedEventArgs e)
        {
            ChartDataFillingByTime(1);
            ChartDataFilling(1);
        }

        private void RdAll_OnClick(object sender, RoutedEventArgs e)
        {
            ChartDataFillingByTime(0);
            ChartDataFilling(0);
        }

        private void RdMonth_OnClick(object sender, RoutedEventArgs e)
        {
            ChartDataFillingByTime(2);
            ChartDataFilling(2);
        }
    }
}