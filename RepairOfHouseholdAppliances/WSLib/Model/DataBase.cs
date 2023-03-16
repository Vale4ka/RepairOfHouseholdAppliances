using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSLib.Core;
using WSLib.Tables;

namespace WSLib.Model
{
    public class DataBase
    {
        private readonly SqlConnection _con;
        private SqlDataReader _reader;
        private User _actualUser;
        private MyException _exception;
        private SqlDataAdapter _adapter = new SqlDataAdapter();
        SqlCommandBuilder _builder = new SqlCommandBuilder();
        public MyException GetException() { return _exception; }
        public User GetActualUser() { return _actualUser; }
        public DataBase()
        {
            _con = new SqlConnection("Server = .\\SQLEXPRESS; Database = NazarovSingle;Trusted_Connection=True;");
        }
        private bool ExecuteCom(string com, Action action)
        {
            bool result;
            SqlCommand cmd = new SqlCommand(com, _con);
            try
            {
                _con.Open();
            }
            catch(Exception e)
            {
                _exception = new MyException("Нет подключения к БД", 102);
            }
            
            try
            {
                _reader = cmd.ExecuteReader();
                
                
                int count = 0;
                while (_reader.Read())
                {
                    count++;
                    action();
                }
                if (count == 0)
                {
                    _exception = new MyException("Ни одной строки не найдено", 201);
                    result = false;

                }
                else
                {
                    result = true;
                }
                _reader.Close();

            }
            catch (Exception ex)
            {
                _exception = new MyException(ex.Message, 101);
                result = false;
            }
            _con.Close();
            return result;
        }
        private void ExecuteCom(string com)
        {
            SqlCommand cmd = new SqlCommand(com, _con);
            _con.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _exception = new MyException(ex.Message, 102);
            }
            _con.Close();
        }
        public void SingIn(string login, string password)
        {
            string command = string.Format("Select Users.Id, Users.Login, Users.FullName, Roles.Name as RoleName from Users " +
                             "join Roles on Users.RoleId = Roles.Id " +
                             "Where Login = '{0}' and Password = '{1}'", login, password);

            bool res = ExecuteCom(command, () => {
                _actualUser = new User(
                    int.Parse(_reader["Id"].ToString()),
                    _reader["Login"].ToString(),

                    _reader["RoleName"].ToString()
                    )
                {
                    FullName = _reader["FullName"].ToString()
                };  
            });
            if (!res)
            {
                //_exception = new MyException("Ошибка авторизации", 202);
                return;
            }
            _exception = new MyException("Успешная авторизация", 301);

        }
        public void AddUser(string login, string password, string RoleName)
        {
            string command = string.Format("Select Id from Roles " +
                             "Where Name = '{0}'", RoleName);
            int roleId = -1;
            bool res = ExecuteCom(command, () =>
            {
                roleId = int.Parse(_reader["Id"].ToString());
            });
            if (!res)
            {
                _exception = new MyException("Роли не существует", 203);
                return;
            }
            command = string.Format("Insert Into Users Values " +
                             "('{0]','{1}','{2}')", login, password, roleId);
            ExecuteCom(command);
            _exception = new MyException("Успешное добавление", 302);
        }
        public List<string> GetTablesName()
        {
            List<string> tables = new List<string>();
            string command = string.Format("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES");
            bool res = ExecuteCom(command, () => {

                tables.Add(_reader["TABLE_NAME"].ToString());

            });
            return tables;
        }
        public DataView SelectTable(string tableName, string filterName)
        {
            DataTable dt = new DataTable();
            if(filterName == "")
            {
                _adapter.SelectCommand = new SqlCommand($"Select * from {tableName}", _con);
            }
            else
            {
                List<string> columns = new List<string>();
                string command = string.Format($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS "+
                    "WHERE TABLE_NAME = '" + tableName + "'");
                bool res = ExecuteCom(command, () => {

                    columns.Add(_reader["COLUMN_NAME"].ToString());

                });
                string selCommand = "Select * from "+tableName+" where ";
                for(int i = 0; i < columns.Count; i++ )
                {
                    selCommand += columns[i] + " like N'" + filterName + "%'";
                    if(!(i == columns.Count - 1))
                    {
                        selCommand += " or ";
                    }

                }
                _adapter.SelectCommand = new SqlCommand(selCommand, _con);
            }
            
            _adapter.Fill(dt);
            _builder = new SqlCommandBuilder(_adapter);
            return dt.DefaultView;
        }
        public void SaveChanges(DataTable table)
        {

            _adapter.InsertCommand = _builder.GetInsertCommand();
            _adapter.UpdateCommand = _builder.GetUpdateCommand();
            _adapter.DeleteCommand = _builder.GetDeleteCommand();
            _adapter.Update(table);
           
        }

    }
}
