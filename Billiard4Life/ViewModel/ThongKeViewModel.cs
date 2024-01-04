using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using RestaurantManagement.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Billiard4Life.DataProvider;
using System.Drawing.Drawing2D;
using System.Data.SqlTypes;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using LiveCharts.Defaults;

namespace Billiard4Life.ViewModel
{
    public class ThongKeViewModel : BaseViewModel
    {
        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;
        private string _SumOfPaid;
        public string SumOfPaid { get => _SumOfPaid; set { _SumOfPaid = value; OnPropertyChanged(); } }
        private string _SumOfProfit;
        public string SumOfProfit { get => _SumOfProfit; set { _SumOfProfit = value; OnPropertyChanged(); } }
        private string _DateBegin;
        public string DateBegin
        {
            get => _DateBegin;
            set
            {
                _DateBegin = value;
                if (DateBegin == null || DateEnd == null) return;
                GetRevenue("Ngày");
                OnPropertyChanged();
            }
        }
        private string _DateEnd;
        public string DateEnd
        {
            get => _DateEnd;
            set
            {
                _DateEnd = value;
                if (DateBegin == null || DateEnd == null) return;
                GetRevenue("Ngày");
                OnPropertyChanged();
            }
        }
        private SeriesCollection _SeriesCollectionRevenue;
        public SeriesCollection SeriesCollectionRevenue
        {
            get { return _SeriesCollectionRevenue; }
            set { _SeriesCollectionRevenue = value; }
        }
        private SeriesCollection _SeriesCollectionCrowd;
        public SeriesCollection SeriesCollectionCrowd
        {
            get { return _SeriesCollectionCrowd; }
            set { _SeriesCollectionCrowd = value; }
        }
        private SeriesCollection _SeriesCollectionTypeTable;
        public SeriesCollection SeriesCollectionTypeTable
        {
            get { return _SeriesCollectionTypeTable; }
            set { _SeriesCollectionTypeTable = value; }
        }
        private SeriesCollection _SeriesCollectionStaff;
        public SeriesCollection SeriesCollectionStaff
        {
            get { return _SeriesCollectionStaff; }
            set { _SeriesCollectionStaff = value; }
        }
        public Func<double, string> Formatter { get; set; }
        public Func<double, string> CrowdFormatter { get; set; }
        private string _CrowdMonth;
        public string CrowdMonth
        {
            get => _CrowdMonth;
            set
            {
                _CrowdMonth = value;
                GetCrowd();
                OnPropertyChanged();
            }
        }
        private string _StaffMonth;
        public string StaffMonth
        {
            get => _StaffMonth;
            set
            {
                _StaffMonth = value;
                GetStaffRevenue();
                OnPropertyChanged();
            }
        }
        private string _TypeTableMonth;
        public string TypeTableMonth
        {
            get => _TypeTableMonth;
            set
            {
                _TypeTableMonth = value;
                GetPercentTypeTable();
                OnPropertyChanged();
            }
        }
        private ObservableCollection<string> _ListMonths;
        public ObservableCollection<string> ListMonths { get => _ListMonths; set { _ListMonths = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _ListCrowdMonths;
        public ObservableCollection<string> ListCrowdMonths { get => _ListCrowdMonths; set { _ListCrowdMonths = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _LabelsCrowd;
        public ObservableCollection<string> LabelsCrowd { get => _LabelsCrowd; set { _LabelsCrowd = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _LabelsRevenue;
        public ObservableCollection<string> LabelsRevenue { get => _LabelsRevenue; set { _LabelsRevenue = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _LabelsStaff;
        public ObservableCollection<string> LabelsStaff { get => _LabelsStaff; set { _LabelsStaff = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _Types;
        public ObservableCollection<string> Types { get => _Types; set { _Types = value; OnPropertyChanged(); } }
        private string _TypeSelected;
        public string TypeSelected
        {
            get => _TypeSelected;
            set
            {
                _TypeSelected = value;
                if (TypeSelected == "Theo ngày")
                {
                    DateBeginVisible = "Visible";
                    DateEndVisible = "Visible";
                    ListTimeVisible = "Hidden";
                    DateBegin = DateTime.Now.Month + "/1/" + DateTime.Now.Year;
                    DateEnd = DateTime.Now.ToShortDateString();
                }
                if (TypeSelected == "Theo tháng")
                {
                    DateBeginVisible = "Hidden";
                    DateEndVisible = "Hidden";
                    ListTimeVisible = "Visible";
                    GetListTime("Tháng");
                    TimeSelected = ListTime[ListTime.Count - 1];
                }
                if (TypeSelected == "Theo năm")
                {
                    DateBeginVisible = "Hidden";
                    DateEndVisible = "Hidden";
                    ListTimeVisible = "Visible";
                    GetListTime("Năm");
                    TimeSelected = ListTime[ListTime.Count - 1];
                }
                OnPropertyChanged();
            }
        }
        private string _PercentProOnRevenue;
        public string PercentProOnRevenue { get => _PercentProOnRevenue; set { _PercentProOnRevenue = value; OnPropertyChanged(); } }
        private string _DateBeginVisible;
        public string DateBeginVisible { get => _DateBeginVisible; set { _DateBeginVisible = value; OnPropertyChanged(); } }
        private string _DateEndVisible;
        public string DateEndVisible { get => _DateEndVisible; set { _DateEndVisible = value; OnPropertyChanged(); } }
        private string _ListTimeVisible;
        public string ListTimeVisible { get => _ListTimeVisible; set { _ListTimeVisible = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _ListTime;
        public ObservableCollection<string> ListTime { get => _ListTime; set { _ListTime = value; OnPropertyChanged(); } }
        private string _TimeSelected;
        public string TimeSelected
        {
            get => _TimeSelected;
            set
            {
                _TimeSelected = value;
                if (TypeSelected == "Theo tháng") GetRevenue("Tháng");
                if (TypeSelected == "Theo năm") GetRevenue("Năm");
                OnPropertyChanged();
            }
        }

        public ThongKeViewModel()
        {
            OpenConnect();

            ListMonths = new ObservableCollection<string>();
            ListCrowdMonths = new ObservableCollection<string>();
            LabelsCrowd = new ObservableCollection<string>();
            LabelsRevenue = new ObservableCollection<string>();
            LabelsStaff = new ObservableCollection<string>();
            Types = new ObservableCollection<string>();
            ListTime = new ObservableCollection<string>();
            SeriesCollectionRevenue = new SeriesCollection();
            SeriesCollectionCrowd = new SeriesCollection();
            SeriesCollectionStaff = new SeriesCollection();
            SeriesCollectionTypeTable = new SeriesCollection();

            Formatter = value => String.Format("{0:0,0 VND}", Math.Round(value));
            CrowdFormatter = value => Math.Round(value).ToString("G");

            Types.Add("Theo ngày");
            Types.Add("Theo tháng");
            Types.Add("Theo năm");

            TypeSelected = "Theo ngày";
            DateBeginVisible = "Visible";
            DateEndVisible = "Visible";
            ListTimeVisible = "Hidden";
            CrowdMonth = StaffMonth = TypeTableMonth = DateTime.Now.Month + "/" + DateTime.Now.Year;

            GetListMonths();

            CloseConnect();
        }
        
        public void GetListTime(string month)
        {
            ListTime.Clear();

            if (month == "Tháng")
            {
                for (int i = 1; i <= DateTime.Now.Month; i++)
                {
                    ListTime.Add(i.ToString() + "/" + DateTime.Now.Year);
                }
            }
            else
            {
                for (int i = DateTime.Now.Year - 3; i <= DateTime.Now.Year; i++)
                {
                    ListTime.Add(i.ToString());
                }
            }
        }
        public void GetListMonths()
        {
            ListCrowdMonths.Clear();
            ListMonths.Clear();
            int currentMonth = DateTime.Now.Month;
            for (int i = 1; i <= currentMonth; i++)
            {
                ListMonths.Add(i.ToString() + "/" + DateTime.Now.Year);
                ListCrowdMonths.Add(i.ToString() + "/" + DateTime.Now.Year);
            }
            ListMonths.Add("Tất cả");
        }
        private string GetMonth(string dt)
        {
            int i = 0;
            string month = "";
            while (i < dt.Length && dt[i] != '/')
            {
                month += dt[i];
                i++;
            }
            return month;
        }

        private void OpenConnect()
        {
            sqlCon = new SqlConnection(strCon);
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
        }
        private void CloseConnect()
        {
            if (sqlCon.State == ConnectionState.Open)
            {
                sqlCon.Close();
            }
        }
    }
}