using Pereklichka.Database;
using Pereklichka.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
using System.Windows.Threading;

namespace Pereklichka.Pages
{
    /// <summary>
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        DispatcherTimer Timer = new DispatcherTimer();
        public StartPage()
        {
            InitializeComponent();
            Timer.Interval = new TimeSpan(0, 1, 0);
            Timer.Tick += TimerTick;
            Timer.Start();
            ReloadPage();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            foreach (var mail in DataHelper.GetContext().DeferredMailing.Where(n => n.StatusId == 1))
            {
                if (mail.DeferredTime <= DateTime.Now)
                {
                    StartMailing(mail);
                    DataHelper.GetContext().DeferredMailing.Remove(mail);
                    DataHelper.GetContext().SaveChanges();
                    ReloadPage();
                }
            }
        }

        private async void StartMailing(DeferredMailing mail)
        {
            await Task.Run(() =>
            {
                foreach (var user in DataHelper.GetContext().Users)
                {
                    MailAddress from = new MailAddress(user.Email, user.Name + " " + user.Lastname);
                    MailAddress to = new MailAddress(mail.SendEmail, "test");

                    using (MailMessage message = new MailMessage(from, to))
                    {
                        string domen = user.Email.Split(new char[] { '@' })[1];
                        using (SmtpClient client = new SmtpClient($"smtp."+ domen == "bk.ru" ? "mail.ru" : domen, 587))
                        {
                            message.Subject = "";
                            message.Body = user.Name + " " + user.Lastname;

                            client.Credentials = new NetworkCredential(user.Email, user.Password);
                            client.EnableSsl = true;
                            try
                            {
                                client.Send(message);
                            }
                            catch
                            {
                                Console.WriteLine($"Сообщения с почты помечаются как спам");
                            }
                        }
                    }
                }
            });
        }

        private void ReloadPage()
        {
            DataHelper.GetContext().ChangeTracker.Entries().ToList().ForEach(n => n.Reload());
            IS31Data.ItemsSource = DataHelper.GetContext().Users.Where(n => n.GroupId == 1).ToList();
            DeferredListBox.ItemsSource = DataHelper.GetContext().DeferredMailing.Where(n => n.DeferredTime > DateTime.Now).ToList();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            UserAddForm userAddForm = new UserAddForm(null);
            userAddForm.ShowDialog();
            ReloadPage();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (IS31Data.SelectedItem is Users user)
            {
                UserAddForm userAddForm = new UserAddForm(user);
                userAddForm.ShowDialog();
                ReloadPage();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<Users> users = IS31Data.SelectedItems.Cast<Users>().ToList();
                if (MessageBox.Show("Вы действительно хотите удалить эти элементы?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    DataHelper.GetContext().Users.RemoveRange(users);

                try
                {
                    DataHelper.GetContext().SaveChanges();
                    ReloadPage();
                }
                catch
                {

                }
            }
            catch
            {
                MessageBox.Show("Выберите элементы удаления!");
            }
        }

        private void SendMail_Click(object sender, RoutedEventArgs e)
        {
            if (DeferredListBox.SelectedItem is DeferredMailing mail)
                StartMailing(mail);
            else
                MessageBox.Show("Выберите рассылку!");


        }

        private void AddMail_Click(object sender, RoutedEventArgs e)
        {
            MailAddForm mailAddForm = new MailAddForm();
            mailAddForm.ShowDialog();
            ReloadPage();
        }

        private void DeleteMail_Click(object sender, RoutedEventArgs e)
        {
            if (DeferredListBox.SelectedItem is DeferredMailing mail)
                DataHelper.GetContext().DeferredMailing.Remove(mail);
            else
            {
                MessageBox.Show("Выберите рассылку!");
                return;
            }
            MessageBox.Show("Успешно удалено!");
            DataHelper.GetContext().SaveChanges();
            ReloadPage();
        }
    }
}
