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
    /// Логика взаимодействия для UserAddForm.xaml
    /// </summary>
    public partial class UserAddForm : Window
    {
        Users Users = new Users();
        public UserAddForm(Users users)
        {
            InitializeComponent();
            if (users != null)
                Users = users;
            GroupComboBox.ItemsSource = DataHelper.GetContext().Group.ToList();
            DataContext = Users;
            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(Users.Name))
                errors.AppendLine("Введите корректное имя.");
            if (string.IsNullOrWhiteSpace(Users.Lastname))
                errors.AppendLine("Введите корректную фамилию.");
            if (string.IsNullOrWhiteSpace(Users.Email))
                errors.AppendLine("Введите корректную почту.");
            if (string.IsNullOrWhiteSpace(Users.Password))
                errors.AppendLine("Введите корректную фамилию.");
            if (Users.Group == null)
                errors.AppendLine("Выберите группу!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Users.UserId == 0)
                DataHelper.GetContext().Users.Add(Users);

            try
            {
                DataHelper.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена!");
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
