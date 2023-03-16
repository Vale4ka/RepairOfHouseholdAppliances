using RepairOfHouseholdAppliances.Core;
using RepairOfHouseholdAppliances.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace RepairOfHouseholdAppliances.ViewModel
{
    public class SingInVM : BindableBase
    {
        public SingInVM()
        {
            SystemBlocked = true;
            Exception = "";
        }
        private bool _systemBlocked;
        public bool SystemBlocked
        {
            get => _systemBlocked;
            set => SetProperty(ref _systemBlocked, value);
        }
        private string _login;
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }
        private string _exception;
        public string Exception
        {
            get => _exception;
            set => SetProperty(ref _exception, value);
        }
        public ICommand SingInCommand { get => new ParCommand(SingIn); }
        private void SingIn(object parameter)
        {
            string password = (parameter as PasswordBox).Password;
            if (Login == "" || password == "")
            {
                Exception = "Поля не должны быть пустые";
                return;
            }
            MainModel.GetDataBase().SingIn(Login, password);
            Exception = MainModel.GetDataBase().GetException().Message;
            if (MainModel.GetDataBase().GetException().Code == 202)
            {
                SystemBlocked = false;
                return;
            }
            if (MainModel.GetDataBase().GetException().Code == 301)
            {
                switch (MainModel.GetDataBase().GetActualUser().RoleName)
                {
                    case "Admin":
                        MainModel.GetViews().OpenAdmin();
                        MainModel.GetViews().CloseSingIn();
                        break;
                    case "Worker":
                        MainModel.GetViews().OpenWorker();
                        MainModel.GetViews().CloseSingIn();
                        break;
                }
            }
        }
    }
}
