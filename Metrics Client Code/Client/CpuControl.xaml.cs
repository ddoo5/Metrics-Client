﻿using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Metrics_app.Services;
using Microsoft.OpenApi.Models;

namespace Metrics_app
{
    /// <summary>
    /// Логика взаимодействия для CpuControl.xaml
    /// </summary>
    public partial class CpuControl : UserControl, INotifyPropertyChanged
    {
        private MetricsAgentClient metricsAgentClient;
        private int _lastLecture;
        private double _trend = 1;
        int Id = 1;

        public event PropertyChangedEventHandler? PropertyChanged;


        public CpuControl()
        {
            InitializeComponent();
            metricsAgentClient = new MetricsAgentClient(new HttpClient());
            LastHourSeries = new SeriesCollection
            {
                new LineSeries
                {
                    AreaLimit = -10,
                    Values = new ChartValues<ObservableValue>
                    {
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0),
                        new ObservableValue(0)
                    }
                }
            };
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(120);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LastHourSeries[0].Values.Add(new ObservableValue(_trend));
                        LastHourSeries[0].Values.RemoveAt(0);
                        Set();
                    });
                }
            });

            DataContext = this;
        }


        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public SeriesCollection LastHourSeries { get; set; }

        public int LastLecture
        {
            get { return _lastLecture; }
            set
            {
                _lastLecture = value;
                OnPropertyChanged("LastLecture");
            }
        }
        private async void Set()
        {
            CpuMetrics cpuMetrics = await metricsAgentClient.GetCpuMetrics(new GetAllCpuMetricsApiRequest()
            {
                ClientBaseAddress = "http://localhost:5000",
                id = Id
            });

            try
            {
                if (cpuMetrics != null)
                {
                    Task.Run(() =>
                    {
                        LastLecture = cpuMetrics.Value;
                    });
                    _trend = Convert.ToDouble(cpuMetrics.Value);
                    Id++;
                }
            }
            catch (Exception ex) { }
        }
    }
}
