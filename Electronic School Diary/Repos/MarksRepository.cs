using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;
using System.Windows.Forms;


namespace ElectronicSchoolDiary.Repos
{
    class MarksRepository
    {
        private static SqlCeConnection Connection = DataBaseConnection.Instance.Connection;

        public static int getMarkQuery()
        {
            string markString = @"SELECT Mark FROM Marks";
            int mark = Int16.Parse(markString);
            return mark;
        }

        public static int GetIdByMark(int mark)
        {
            SqlCeCommand command = new SqlCeCommand(@"SELECT Id FROM Marks WHERE Mark = @mark", Connection);
            command.Parameters.AddWithValue("@mark", mark);
            SqlCeDataReader reader = command.ExecuteReader();

            reader.Read();

            int result = (int)reader["Id"];
            reader.Close();

            return result;
        }

        public static DateTime GetDateQuery()
        {
            string dateString = @"SELECT Date FROM Marks";
            DateTime date = DateTime.Parse(dateString);
            return date;
        }

        public static int GetIdByStudentId(int studentId)
        {
            SqlCeCommand command = new SqlCeCommand(@"SELECT Id FROM Marks WHERE StudentsId = @studentid", Connection);
            command.Parameters.AddWithValue("@studentid", studentId);
            SqlCeDataReader reader = command.ExecuteReader();

            reader.Read();

            int result = (int)reader["Id"];
            reader.Close();

            return result;
        }

        public static int GetIdByCoursesId(int courseId)
        {
            SqlCeCommand command = new SqlCeCommand(@"SELECT Id FROM Marks WHERE CoursesId = @courseid", Connection);
            command.Parameters.AddWithValue("@courseid", courseId);
            SqlCeDataReader reader = command.ExecuteReader();

            reader.Read();

            int result = (int)reader["Id"];
            reader.Close();

            return result;
        }

        public static bool AddMark(int Mark, DateTime Date, int StudentId, int CourseId)
        {
            bool flag = false;

            try
            {
                if(Mark < 1 || Mark > 5)
                {
                    MessageBox.Show("Ocjena mora biti izmedju 1 i 5!");
                }
                int studentId = StudentRepository.GetIdByNumber(StudentId);
                int courseId = CoursesRepository.GetIdByClassesId(CourseId);
                SqlCeCommand command1 = new SqlCeCommand(@"INSERT INTO Marks(Mark, Date, StudentId, CourseId)
                    VALUES (@mark, @date, @studentId, @courseId)", Connection);
                command1.Parameters.AddWithValue("@mark", Mark);
                command1.Parameters.AddWithValue("@date", Date);
                command1.Parameters.AddWithValue("@studentId", studentId);
                command1.Parameters.AddWithValue("@courseId", courseId);
                int result = command1.ExecuteNonQuery();
                if(result > 0)
                {
                    flag = true;
                    MessageBox.Show("Ocjena je uspjesno dodata!");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return flag;
        }




    }
}
