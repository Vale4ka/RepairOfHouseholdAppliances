using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairOfHouseholdAppliances.View
{
    public class Views
    {
        public Views() { }
        private SingIn _singIn;
        public SingIn SingIn
        {
            get => _singIn;
            set
            {
                if (_singIn == null) _singIn = value;
            }
        }
        public void SetSingIn(SingIn singIn)
        {
            if (_singIn == null) _singIn = singIn;
        }
        public void CloseSingIn()
        {
            try
            {
                _singIn.Close();
            }
            catch { }
        }
        private Admin _admin;
        public void OpenAdmin()
        {
            _admin = new Admin();
            _admin.Show();
        }
        public void CloseAdmin()
        {
            try
            {
                _admin.Close();
            }
            catch { }
        }
        private Worker _worker;
        public void OpenWorker()
        {
            _worker = new Worker();
            _worker.Show();
        }
        public void CloseWorker()
        {
            try
            {
                _worker.Close();
            }
            catch { }
        }
    }
}
