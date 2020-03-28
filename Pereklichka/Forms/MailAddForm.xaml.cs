using Pereklichka.Database;
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

namespace Pereklichka.Forms
{
    /// <summary>
    /// Логика взаимодействия для MailAddForm.xaml
    /// </summary>
    public partial class MailAddForm : Window
    {
        DeferredMailing deferredMailing = new DeferredMailing();
        public MailAddForm()
        {
            InitializeComponent();

            GroupComboBox.ItemsSource = DataHelper.GetContext().Group.ToList();
            DataContext = deferredMailing;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(deferredMailing.SendEmail))
                errors.AppendLine("Введите корректную почту.");
            if (DateBox.SelectedDate == null || HourBox.Text == "" || MinuteBox.Text == "")
                errors.AppendLine("Введите корректную дату.");
            if (deferredMailing.Group == null)
                errors.AppendLine("Выберите группу!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime dateTime = new DateTime();
            dateTime = (DateTime)DateBox.SelectedDate;
            deferredMailing.DeferredTime = dateTime + new TimeSpan(int.Parse(HourBox.Text), int.Parse(MinuteBox.Text), 0);
            deferredMailing.StatusId = 1;
            if (deferredMailing.DeferredId == 0)
                DataHelper.GetContext().DeferredMailing.Add(deferredMailing);

            try
            {
                DataHelper.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
