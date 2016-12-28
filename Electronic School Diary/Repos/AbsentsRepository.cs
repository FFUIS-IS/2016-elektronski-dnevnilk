using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;
using System.Windows.Forms;

namespace ElectronicSchoolDiary.Repos
{
    class AbsentsRepository
    {
        private static SqlCeConnection Connection = DataBaseConnection.Instance.Connection;

        public static int GetHourQuery()
        {
            string hourString = @"SELECT Hour FROM Abstents";
            int hour = Int16.Parse(hourString);
            return hour;
        }

        public static bool IsJustifiedQuery()
        {
            string isJustifiedString = @"SELECT Justified FROM Abstents";
            bool isJustified = Boolean.Parse(isJustifiedString);
            return isJustified;
        }

        public static DateTime GetDateQuery()
        {
            string dateString = @"SELECT Date FROM Absents";
            DateTime date = DateTime.Parse(dateString);
            return date;
        }

        public static int GetIdByStudentId(int studentId)
        {
            SqlCeCommand command = new SqlCeCommand(@"SELECT Id FROM Absents WHERE StudentsId = @studentid", Connection);
            command.Parameters.AddWithValue("@studentid", studentId);
            SqlCeDataReader reader = command.ExecuteReader();

            reader.Read();

            int result = (int)reader["Id"];
            reader.Close();

            return result;
        }

        public static bool AddAbsent(int Hour, DateTime Date, int StudentId, bool IsJustified)
        {
            bool flag = false;

            try
            {
                if (Hour < 1 || Hour > 7)
                {
                    MessageBox.Show("Broj casova mora biti izmedju 1 i 7!");
                }
                int studentId = StudentRepository.GetIdByNumber(StudentId);
                SqlCeCommand command1 = new SqlCeCommand(@"INSERT INTO Absents(Hour, Date, StudentId, IsJustified)
                    VALUES (@hour, @date, @studentId, @isJustified)", Connection);
                command1.Parameters.AddWithValue("@hour", Hour);
                command1.Parameters.AddWithValue("@date", Date);
                command1.Parameters.AddWithValue("@studentId", studentId);
                command1.Parameters.AddWithValue("@isJustified", IsJustified);
                int result = command1.ExecuteNonQuery();
                if (result > 0)
                {
                    flag = true;
                    MessageBox.Show("Izostanak je uspjesno dodat!");
                    //IsJustified = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return flag;
        }
    }
}
