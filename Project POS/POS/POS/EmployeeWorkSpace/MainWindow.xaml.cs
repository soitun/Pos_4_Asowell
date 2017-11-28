﻿using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using POS.BusinessModel;
using POS.Context;
using POS.Entities;
using POS.Repository;
using POS.Repository.DAL;
using POS.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows.Controls;
using POS.Helper.PrintHelper;
using Chair = POS.BusinessModel.Chair;

namespace POS.EmployeeWorkSpace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// the object that store all repository you want to get data DBAsowell
        /// in Employee WorkSpace
        /// </summary>
        internal EmployeewsOfAsowell _unitofwork;
        internal EmployeewsOfCloud _cloudUnitofwork;

        Employee emp;
        SalaryNote empSln;
        public ProgressBar proTable;
        public ProgressBar proChair;
        public Entities.Table currentTable { get; set; }
        public Entities.Chair currentChair { get; set; }
        internal Table b;
        internal Dash d;
        internal Entry en;
        internal Info info;
        internal Login login;
        internal SettingFoodPage st;
        internal SettingTableSize stts;
        internal ChangeThemePage chtm;
        internal AllEmployeeLogin ael;

        public MainWindow()
        {
            InitializeComponent();
            currentTable = null;
            emp = App.Current.Properties["EmpLogin"] as Employee;
            proTable = pgbReservedTable as ProgressBar;
            proChair = pgbReservedChair as ProgressBar;

            cUser.Content = EmpLoginListData.emploglist.Count() + " employee(s) available";


            //string[] config = ReadWriteData.ReadDBConfig();
            //if (config != null)
            //{
            //    _unitofwork = new EmployeewsOfAsowell(config[0], config[1], config[2], config[3]);
            //}
            //else
            //{
            //    _unitofwork = new EmployeewsOfAsowell();
            //}

            _unitofwork = new EmployeewsOfAsowell();
            _cloudUnitofwork = new EmployeewsOfCloud();
            try
            {
                b = new Table(_unitofwork, _cloudUnitofwork);
                d = new Dash();
                en = new Entry();
                info = new Info();
                st = new SettingFoodPage(_cloudUnitofwork);
                stts = new SettingTableSize();

                DispatcherTimer workTimer = new DispatcherTimer();
                workTimer.Tick += WorkTime_Tick;
                workTimer.Interval = new TimeSpan(0, 0, 1);
                workTimer.Start();

                DispatcherTimer RefreshTimer = new DispatcherTimer();
                RefreshTimer.Tick += Refresh_Tick;
                RefreshTimer.Interval = new TimeSpan(0, 5, 0);
                RefreshTimer.Start();

                initProgressTableChair();


                this.Loaded += (sender, args) =>
                {
                    bntTable.IsEnabled = true;
                    bntDash.IsEnabled = false;
                    bntEntry.IsEnabled = true;
                    bntInfo.IsEnabled = true;
                    myFrame.Navigate(d);
                };

                this.Closing += (sender, args) =>
                {
                    workTimer.Stop();
                    _unitofwork.Dispose();
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        

        public void initProgressTableChair()
        {
            proTable.Maximum = 0;
            proTable.Value = 0;
            proChair.Maximum = 0;
            proChair.Value = 0;

            foreach (Entities.Table t in _unitofwork.TableRepository.Get().ToList())
            {
                if (t.ChairAmount != 0)
                {
                    var chairoftable = _unitofwork.ChairRepository.Get(x => x.TableOwned.Equals(t.TableId)).ToList();
                    if (chairoftable.Count != 0)
                    {
                        foreach (var ch in chairoftable)
                        {
                            var chairorderdetailstemp = _unitofwork.OrderDetailsTempRepository.Get(x => x.ChairId.Equals(ch.ChairId)).ToList();
                            if (chairorderdetailstemp.Count != 0)
                            {
                                proChair.Value += 1;
                                proChair.Maximum += 1;
                            }
                            else
                            {
                                proChair.Maximum += 1;
                            }
                        }
                    }
                }

                if (t.IsOrdered == 1)
                {
                    proTable.Value += 1;
                    proTable.Maximum += 1;
                }
                else
                {
                    proTable.Maximum += 1;
                }

                setToolTip(proChair, proTable);
            }
        }

        private void setToolTip(ProgressBar proChair, ProgressBar proTable)
        {
            proTable.ToolTip = "Reserved table(" + proTable.Value + "/" + proTable.Maximum + ")";
            proChair.ToolTip = "Reserved chair(" + proChair.Value + "/" + proChair.Maximum + ")";
        }

        private void Refresh_Tick(object sender, EventArgs e)
        {
            en.ucMenu.UcMenu_Loaded(en.ucMenu, null);
        }

        private void WorkTime_Tick(object sender, EventArgs e)
        {
            DateTime nowWH = DateTime.Now;
            DateTime startWH = (App.Current.Properties["EmpWH"] as WorkingHistory).StartTime;
            var timer = nowWH - startWH;
            string fH = "", fm = "", fs = "";
            fH = timer.Hours.ToString();
            fm = timer.Minutes.ToString();
            fs = timer.Seconds.ToString();

            if (timer.Hours < 10)
            {
                fH = "0" + timer.Hours;
            }
            if (timer.Minutes < 10)
            {
                fm = "0" + timer.Minutes;
            }
            if (timer.Seconds < 10)
            {
                fs = "0" + timer.Seconds;
            }

            txtTimeWk.Text = fH + ":" + fm + ":" + fs;
        }

        private void bntDash_Click(object sender, RoutedEventArgs e)
        {
            bntTable.IsEnabled = true;
            bntDash.IsEnabled = false;
            bntEntry.IsEnabled = true;
            bntInfo.IsEnabled = true;
            myFrame.Navigate(d);

        }

        private void bntTable_Click(object sender, RoutedEventArgs e)
        {
            myFrame.Navigate(b);
            bntTable.IsEnabled = false;
            bntDash.IsEnabled = true;
            bntEntry.IsEnabled = true;
            bntInfo.IsEnabled = true;
        }

        private void bntEntry_Click(object sender, RoutedEventArgs e)
        {
            myFrame.Navigate(en);
            bntTable.IsEnabled = true;
            bntDash.IsEnabled = true;
            bntEntry.IsEnabled = false;
            bntInfo.IsEnabled = true;
        }

        private void bntInfo_Click(object sender, RoutedEventArgs e)
        {
            myFrame.Navigate(info);
            bntTable.IsEnabled = true;
            bntDash.IsEnabled = true;
            bntEntry.IsEnabled = true;
            bntInfo.IsEnabled = false;
        }


        private void DemoItemsListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void lbiFoodList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myFrame.Navigate(st);
            bntTable.IsEnabled = true;
            bntDash.IsEnabled = true;
            bntEntry.IsEnabled = true;
            bntInfo.IsEnabled = true;
        }

        private void lbiTableSize_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myFrame.Navigate(stts);
            bntTable.IsEnabled = true;
            bntDash.IsEnabled = true;
            bntEntry.IsEnabled = true;
            bntInfo.IsEnabled = true;
        }

        private void btnStartWorking_Click(object sender, RoutedEventArgs e)
        {
            if(App.Current.Properties["AdLogin"] != null)
            {
                return;
            }

            if(App.Current.Properties["CurrentEmpWorking"] != null)
            {
                MessageBox.Show("It's have some employee on working! Please wait!");
                return;
            }

            AllEmployeeLogin ael = new AllEmployeeLogin((MainWindow)Window.GetWindow(this), _unitofwork, _cloudUnitofwork, cUser, 4);
            ael.ShowDialog();
        }

        private void btnEndWorking_Click(object sender, RoutedEventArgs e)
        {
            if(App.Current.Properties["AdLogin"] != null)
            {
                App.Current.Properties["AdLogin"] = null;

                if(App.Current.Properties["CurrentEmpWorking"] != null)
                {
                    cUser.Content = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList).Emp.Username;
                }

                if (App.Current.Properties["CurrentEmpWorking"] == null)
                {
                    cUser.Content = EmpLoginListData.emploglist.Count() + " employee(s) available";
                }

                return;
            }

            if (App.Current.Properties["CurrentEmpWorking"] == null)
            {
                cUser.Content = EmpLoginListData.emploglist.Count() + " employee(s) available";
                return;
            }

            if(App.Current.Properties["CurrentEmpWorking"] != null)
            {
                cUser.Content = EmpLoginListData.emploglist.Count() + " employee(s) available";
                App.Current.Properties["CurrentEmpWorking"] = null;
            }
        }

        private void btnOtherEmp_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["AdLogin"] != null)
            {
                return;
            }

            AllEmployeeLogin ael = new AllEmployeeLogin((MainWindow)Window.GetWindow(this), _unitofwork, _cloudUnitofwork, cUser, 1);
            ael.ShowDialog();
        }

        private void btnEmpDetail_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["AdLogin"] != null)
            {
                return;
            }

            AllEmployeeLogin ael = new AllEmployeeLogin((MainWindow)Window.GetWindow(this), _unitofwork, _cloudUnitofwork, cUser, 2);
            ael.ShowDialog();
        }

        private void bntLogout_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["AdLogin"] != null)
            {
                return;
            }

            AllEmployeeLogin ael = new AllEmployeeLogin((MainWindow)Window.GetWindow(this), _unitofwork, _cloudUnitofwork, cUser, 3);
            ael.ShowDialog();

            if(App.Current.Properties["CurrentEmpWorking"] == null)
            {
                if(en.IsEnabled == true)
                {
                    myFrame.Navigate(b);
                    bntTable.IsEnabled = false;
                    bntDash.IsEnabled = true;
                    bntEntry.IsEnabled = true;
                    bntInfo.IsEnabled = true;
                }
            }

            //WorkingHistory wh = App.Current.Properties["EmpWH"] as WorkingHistory;

            //wh.EndTime = DateTime.Now;
            //_unitofwork.WorkingHistoryRepository.Insert(wh);
            //_unitofwork.Save();

            //empSln = _unitofwork.SalaryNoteRepository.Get(sle => sle.EmpId.Equals(emp.EmpId) && sle.ForMonth.Equals(DateTime.Now.Month) && sle.ForYear.Equals(DateTime.Now.Year)).First();
            //var workH = wh.EndTime - wh.StartTime;
            //empSln.WorkHour += workH.Hours + workH.Minutes/60 + workH.Seconds/3600;
            //_unitofwork.SalaryNoteRepository.Update(empSln);
            //_unitofwork.Save();

            //foreach (var table in _unitofwork.TableRepository.Get())
            //{
            //    var orderTemp = _unitofwork.OrderTempRepository.Get(x => x.TableOwned.Equals(table.TableId)).First();
            //    orderTemp.CusId = "CUS0000001";
            //    orderTemp.Ordertime = DateTime.Now;
            //    orderTemp.TotalPrice = 0;
            //    orderTemp.CustomerPay = 0;
            //    orderTemp.PayBack = 0;
            //    table.IsOrdered = 0;
            //    var orderDetails = _unitofwork.OrderDetailsTempRepository.Get(x => x.OrdertempId.Equals(orderTemp.OrdertempId));
            //    if(orderDetails.Count() != 0)
            //    {
            //        foreach(var od in orderDetails)
            //        {
            //            _unitofwork.OrderDetailsTempRepository.Delete(od);
            //            _unitofwork.Save();
            //        }
            //    }
            //}

            //login = new Login();
            //this.Close();
            //login.Show();
        }

        private void lbiChangeTheme_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            chtm = new ChangeThemePage();
            chtm.Show();
        }

        private void LbiEODReport_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var printer = new DoPrintHelper(_unitofwork, _cloudUnitofwork, DoPrintHelper.Eod_Printing);
            printer.DoPrint();
        }
        
    }
}
