using RepairOfHouseholdAppliances.Core;
using RepairOfHouseholdAppliances.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairOfHouseholdAppliances.ViewModel
{
    public class AdminVM : BindableBase
    {
        public AdminVM()
        {
            Tables = MainModel.GetDataBase().GetTablesName();
        }
        private List<string> _tables;
        public List<string> Tables
        {
            get => _tables;
            set => SetProperty(ref _tables, value);
        }
        private string _selectedTable;
        public string SelectedTable
        {
            get => _selectedTable;
            set
            {
                SetProperty(ref _selectedTable, value);
                _search = "";
                if (SelectedTable != null)
                    Table = MainModel.GetDataBase().SelectTable(SelectedTable, Search);
                else
                    Table = null;
            }
        }
        private string _search;
        public string Search
        {
            get => _search ?? "";
            set
            {
                _search = value;
                Table = MainModel.GetDataBase().SelectTable(SelectedTable, Search);
            }
        }
        private DataView _table;
        public DataView Table
        {
            get => _table;
            set => SetProperty(ref _table, value);
        }
        public DelegateCommand SaveCommand { 
            get {
                return new DelegateCommand(() =>
                {
                    MainModel.GetDataBase().SaveChanges(_table.Table);
                });
            }
        }
    }
}
