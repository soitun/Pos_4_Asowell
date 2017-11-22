﻿using POS.Entities;
using POS.Repository.DAL;
using System;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace POS.EmployeeWorkSpace
{
    /// <summary>
    /// Interaction logic for AllEmployeeLogin.xaml
    /// </summary>
    public partial class AllEmployeeLogin : Window
    {
        private EmployeewsOfAsowell _unitofwork;
        private List<Employee> _employee;
        private EmpLoginList _emplog;
        MaterialDesignThemes.Wpf.Chip _cUser;
        private DispatcherTimer LoadForm;
        private bool IsShow = false;
        private int _typeshow = 0;

        public AllEmployeeLogin(EmployeewsOfAsowell unitofwork, MaterialDesignThemes.Wpf.Chip cUser, int typeshow)
        {
            _unitofwork = unitofwork;
            _employee = _unitofwork.EmployeeRepository.Get(x => x.Deleted == 0).ToList();
            _cUser = cUser;
            _typeshow = typeshow;
            InitializeComponent();

            initData();

            this.Loaded += AllEmployeeLogin_Loaded;

            LoadForm = new DispatcherTimer();
            LoadForm.Tick += LoadForm_Tick;
            LoadForm.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }

        private void AllEmployeeLogin_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = 500;
            spLoginAnother.Visibility = Visibility.Collapsed;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void LoadForm_Tick(object sender, EventArgs e)
        {
            if(IsShow)
            {
                this.Width -= 10;
                if(this.Width == 500)
                {
                    LoadForm.Stop();
                }
            }
            else
            {
                this.Width += 10;
                if (this.Width == 870)
                {
                    LoadForm.Stop();
                }
            }
        }

        private void initData()
        {
            foreach (var e in EmpLoginListData.emploglist)
            {
                e.EmpWH.EndTime = DateTime.Now;
                int h = (e.EmpWH.EndTime - e.EmpWH.StartTime).Hours;
                int m = (e.EmpWH.EndTime - e.EmpWH.StartTime).Minutes;
                int s = (e.EmpWH.EndTime - e.EmpWH.StartTime).Seconds;

                e.TimePercent = (int)((((double)h) + (double)m / 60.0 + (double)s / 3600.0) / 24.0 * 100);
            }

            lvLoginList.ItemsSource = EmpLoginListData.emploglist;

            if(_typeshow == 1)
            {
                btnLoginNew.Visibility = Visibility.Collapsed;
                btnLogout.Visibility = Visibility.Collapsed;
                btnView.Visibility = Visibility.Visible;
            }
            if(_typeshow == 2)
            {
                btnLoginNew.Visibility = Visibility.Visible;
                btnLogout.Visibility = Visibility.Collapsed;
                btnView.Visibility = Visibility.Collapsed;
            }
            if(_typeshow == 3)
            {
                btnLoginNew.Visibility = Visibility.Collapsed;
                btnLogout.Visibility = Visibility.Visible;
                btnView.Visibility = Visibility.Collapsed;
            }
        }
        
        private void btnLoginNew_Click(object sender, RoutedEventArgs e)
        {
            if(this.Width == 500)
            {
                IsShow = false;
                LoadForm.Start();
            }

            spLoginAnother.Visibility = Visibility.Visible;
            lvLoginList.UnselectAll();
            txbLabel.Text = "Login Another";
            txtUsername.IsEnabled = true;
            txtPass.IsEnabled = true;
            txtUsername.Text = "";
            txtPass.Password = "";

            btnAcceptLogin.Visibility = Visibility.Visible;
            btnAcceptLogout.Visibility = Visibility.Collapsed;
            btnAcceptView.Visibility = Visibility.Collapsed;
            btnAcceptCancel.Visibility = Visibility.Visible;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (this.Width == 500)
            {
                IsShow = false;
                LoadForm.Start();
            }

            _emplog = lvLoginList.SelectedItem as EmpLoginList;
            if (_emplog == null)
            {
                MessageBox.Show("Please choose one employee want to logout!");
                return;
            }

            spLoginAnother.Visibility = Visibility.Visible;
            lvLoginList.UnselectAll();
            txbLabel.Text = "Logout";
            txtUsername.IsEnabled = false;
            txtPass.IsEnabled = true;
            txtUsername.Text = _emplog.Emp.Username;
            txtPass.Password = "";

            btnAcceptLogin.Visibility = Visibility.Collapsed;
            btnAcceptLogout.Visibility = Visibility.Visible;
            btnAcceptView.Visibility = Visibility.Collapsed;
            btnAcceptCancel.Visibility = Visibility.Visible;
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (this.Width == 500)
            {
                IsShow = false;
                LoadForm.Start();
            }

            _emplog = lvLoginList.SelectedItem as EmpLoginList;
            if (_emplog == null)
            {
                MessageBox.Show("Please choose one employee want to view details!");
                return;
            }

            spLoginAnother.Visibility = Visibility.Visible;
            lvLoginList.UnselectAll();
            txbLabel.Text = "View Details";
            txtUsername.IsEnabled = false;
            txtPass.IsEnabled = true;
            txtUsername.Text = _emplog.Emp.Username;
            txtPass.Password = "";

            btnAcceptLogin.Visibility = Visibility.Collapsed;
            btnAcceptLogout.Visibility = Visibility.Collapsed;
            btnAcceptView.Visibility = Visibility.Visible;
            btnAcceptCancel.Visibility = Visibility.Visible;
        }

        private async void btnAcceptLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string pass = txtPass.Password.Trim();
            try
            {
                btnLoginNew.IsEnabled = false;
                PgbLoginProcess.Visibility = Visibility.Visible;
                await Async(username, pass, null);

                btnLoginNew.IsEnabled = true;
                PgbLoginProcess.Visibility = Visibility.Collapsed;
                
                lvLoginList.ItemsSource = EmpLoginListData.emploglist;
                lvLoginList.Items.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void btnAcceptLogout_Click(object sender, RoutedEventArgs e)
        {
            if (!_emplog.Emp.Pass.Equals(txtPass.Password.Trim()))
            {
                MessageBox.Show("Fail! Please try again!");
                return;
            }

            try
            {
                btnAcceptLogout.IsEnabled = false;
                PgbLoginProcess.Visibility = Visibility.Visible;
                await Async("", "", _emplog);

                btnAcceptLogout.IsEnabled = true;
                PgbLoginProcess.Visibility = Visibility.Collapsed;
                
                lvLoginList.ItemsSource = EmpLoginListData.emploglist;
                lvLoginList.Items.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnAcceptView_Click(object sender, RoutedEventArgs e)
        {
            if (!_emplog.Emp.Pass.Equals(txtPass.Password.Trim()))
            {
                MessageBox.Show("Login fail! Please try again!");
                return;
            }

            EmployeeDetail ed = new EmployeeDetail(_emplog.Emp.Username, _unitofwork);
            ed.ShowDialog();
        }

        private void btnAcceptCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.Width == 870)
            {
                IsShow = true;
                LoadForm.Start();
            }

            spLoginAnother.Visibility = Visibility.Collapsed;
            lvLoginList.UnselectAll();
            txbLabel.Text = "Login Another";
            txtUsername.IsEnabled = true;
            txtPass.IsEnabled = true;
            txtUsername.Text = "";
            txtPass.Password = "";

            btnAcceptLogin.Visibility = Visibility.Collapsed;
            btnAcceptLogout.Visibility = Visibility.Collapsed;
            btnAcceptView.Visibility = Visibility.Collapsed;
            btnAcceptCancel.Visibility = Visibility.Collapsed;
        }

        private async Task Async(string username, string pass, EmpLoginList empout)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (empout != null)
                    {
                        empout.EmpWH.EndTime = DateTime.Now;
                        _unitofwork.WorkingHistoryRepository.Insert(empout.EmpWH);
                        _unitofwork.Save();

                        var workH = empout.EmpWH.EndTime - empout.EmpWH.StartTime;
                        empout.EmpSal.WorkHour += workH.Hours + workH.Minutes / 60 + workH.Seconds / 3600;
                        _unitofwork.SalaryNoteRepository.Update(empout.EmpSal);
                        _unitofwork.Save();

                        EmpLoginListData.emploglist.Remove(empout);

                        Dispatcher.Invoke(() =>
                        {
                            checkEmployeeCount();
                        });

                        return;
                    }

                    bool isFound = false;
                    foreach (Employee emp in _employee)
                    {
                        if (emp.Username.Equals(username) && emp.Pass.Equals(pass))
                        {
                            var chemp = EmpLoginListData.emploglist.Where(x => x.Emp.EmpId.Equals(emp.EmpId)).ToList();
                            if(chemp.Count != 0)
                            {
                                MessageBox.Show("This employee is already login!");
                                return;
                            }

                            App.Current.Properties["EmpLogin"] = emp;

                            try
                            {
                                SalaryNote empSalaryNote = _unitofwork.SalaryNoteRepository.Get(sle => sle.EmpId.Equals(emp.EmpId) && sle.ForMonth.Equals(DateTime.Now.Month) && sle.ForYear.Equals(DateTime.Now.Year)).First();

                                App.Current.Properties["EmpSN"] = empSalaryNote;
                                WorkingHistory empWorkHistory = new WorkingHistory { ResultSalary = empSalaryNote.SnId, EmpId = empSalaryNote.EmpId };
                                App.Current.Properties["EmpWH"] = empWorkHistory;
                            }
                            catch (Exception ex)
                            {
                                SalaryNote empSalary = new SalaryNote { EmpId = emp.EmpId, SalaryValue = 0, WorkHour = 0, ForMonth = DateTime.Now.Month, ForYear = DateTime.Now.Year, IsPaid = 0 };
                                _unitofwork.SalaryNoteRepository.Insert(empSalary);
                                _unitofwork.Save();
                                WorkingHistory empWorkHistory = new WorkingHistory { ResultSalary = empSalary.SnId, EmpId = empSalary.EmpId };
                                App.Current.Properties["EmpWH"] = empWorkHistory;
                                App.Current.Properties["EmpSN"] = empSalary;
                            }

                            Dispatcher.Invoke(() =>
                            {
                                EmpLoginListData.emploglist.Add(new EmpLoginList { Emp = emp, EmpSal = App.Current.Properties["EmpSN"] as SalaryNote, EmpWH = App.Current.Properties["EmpWH"] as WorkingHistory, TimePercent = 0 });
                                checkEmployeeCount();
                                txtUsername.IsEnabled = true;
                                txtPass.IsEnabled = true;
                                txtUsername.Text = "";
                                txtPass.Password = "";
                            });
                            isFound = true;

                            //end create

                            break;
                        }

                    }

                    if (!isFound)
                    {
                        MessageBox.Show("incorrect username or password");
                        return;
                    }
                });
            }
            catch (Exception ex)
            {

            }
        }

        private void checkEmployeeCount()
        {
            if(EmpLoginListData.emploglist.Count == 1)
            {
                _cUser.Content = EmpLoginListData.emploglist.ElementAt(0).Emp.Username;
            }
            else if(EmpLoginListData.emploglist.Count == 0)
            {
                foreach (var table in _unitofwork.TableRepository.Get())
                {
                    var orderTemp = _unitofwork.OrderTempRepository.Get(x => x.TableOwned.Equals(table.TableId)).First();
                    orderTemp.CusId = "CUS0000001";
                    orderTemp.Ordertime = DateTime.Now;
                    orderTemp.TotalPrice = 0;
                    orderTemp.CustomerPay = 0;
                    orderTemp.PayBack = 0;
                    table.IsOrdered = 0;
                    var orderDetails = _unitofwork.OrderDetailsTempRepository.Get(x => x.OrdertempId.Equals(orderTemp.OrdertempId));
                    if (orderDetails.Count() != 0)
                    {
                        foreach (var od in orderDetails)
                        {
                            _unitofwork.OrderDetailsTempRepository.Delete(od);
                            _unitofwork.Save();
                        }
                    }

                    Login login = new Login();
                    this.Close();
                    login.Show();
                }
            }
            else
            {
                _cUser.Content = EmpLoginListData.emploglist.Count + " employee(s) available";
            }
        }
    }

    public class EmpLoginList
    {
        private Employee _emp;
        private SalaryNote _empsal;
        private WorkingHistory _empwh;
        private int _timepercent;

        public Employee Emp
        {
            get { return _emp; }
            set { _emp = value; }
        }

        public SalaryNote EmpSal
        {
            get { return _empsal; }
            set { _empsal = value; }
        }

        public WorkingHistory EmpWH
        {
            get { return _empwh; }
            set { _empwh = value; }
        }

        public int TimePercent
        {
            get { return _timepercent; }
            set { _timepercent = value; }
        }
    }

    public class EmpLoginListData
    {
        public static List<EmpLoginList> emploglist = new List<EmpLoginList>();
    }
}