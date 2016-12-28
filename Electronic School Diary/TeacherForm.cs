using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using ElectronicSchoolDiary.Models;
using ElectronicSchoolDiary.Repos;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlServerCe;
using System.Diagnostics;

namespace ElectronicSchoolDiary
{  
    public partial class TeacherForm : Form
    {
        private User CurrentUser;
        private Teacher CurrentTeacher;
        private static SqlCeConnection Connection = DataBaseConnection.Instance.Connection;

        public void Warning()
        {
            MessageBox.Show("Polja ne mogu biti prazna !");
        }

        public TeacherForm(User user, Teacher teacher)
        {
            InitializeComponent();
            this.Text = "Nastavnik : " + teacher.Name + " " + teacher.Surname;
            CurrentUser = user;
            CurrentTeacher = teacher;
        }
        private void TeacherForm_Load(object sender, EventArgs e)
        {
            PasswordPanel.Hide();
            ControlBox = false;
            TrueFalseAbsentComboBox.SelectedIndex = 1;
            AbsentHourComboBox.SelectedIndex = 0;
            MarkComboBox.SelectedIndex = 0;
            PopulateStudentsComboBox();
            PopulateCoursesComboBox();
            FillStudentInfoLabels();
            FillParentInfoLabels();
            FillStudentMarks();
            FillStudentAbsents();
        }
        private void PopulateStudentsComboBox()
        {
            try
            {
                int TeacherId = CurrentTeacher.Id;
                int DepartmentId = DepartmentsRepository.GetIdByTeacherId(TeacherId);
                string Name = StudentRepository.GetQuery(DepartmentId);
                Lists.FillDropDownList2(Name, "Name", Name, "Surname", StudentsBox);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }  
        private void PopulateCoursesComboBox()
        {
            try
            { 

               string CoursesId = Teachers_Departments_CoursesRepository.GetCoursesId(CurrentTeacher.Id);
               string Title = CoursesRepository.GetQuery("(" + CoursesId.TrimEnd(',')+ ")");
               Lists.FillDropDownList1(Title, "Title", CoursesBox);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private Student CurrentStudent()
        {
            Student student = null;
            try
            {
                string currentStudentName;
                string currentStudentSurname;
         
                string[] parts = StudentsBox.Text.Split('-');
                currentStudentName = parts[0];
                currentStudentSurname = parts[1];
                student = StudentRepository.GetStudentByName(currentStudentName, currentStudentSurname);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return student;
        }
        private Parent CurrentParent()
        {
            Parent parent;
            Student student = CurrentStudent();
            int studentId = StudentRepository.GetIdByJmbg(student.Jmbg);
            parent = ParentRepository.GetParentByStudentId(studentId);
            return parent;
        }
        private int GetCurrentMark()
        {
            return MarkComboBox.SelectedIndex;
        }
        private string GetCurrentCourse()
        {
            return CoursesBox.Text;
        }
      
        private void FillStudentMarks()
        {
            try
            {
                Student student = CurrentStudent();
                int studentId = StudentRepository.GetIdByJmbg(student.Jmbg);
                int courseId = CoursesRepository.GetIdByTitle(GetCurrentCourse());
                string marks = MarksRepository.GetMarks(studentId, courseId);
                MarksLabel.Text = marks;
                string[] parts = marks.Split(',');
                if (parts.Length > 0)
                {
                    AverageMarkLabel.Text = CalculateAverageGrade(parts);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void FillStudentAbsents()
        {
            try
            {
                Student student = CurrentStudent();
                int studentId = StudentRepository.GetIdByJmbg(student.Jmbg);
                int justifiedAbsents = AbsentsRepository.GetAbsents(studentId, 1);
                int unjustifiedAbsents = AbsentsRepository.GetAbsents(studentId, 0);
                JustifiedAbsentLabel.Text = justifiedAbsents.ToString();
                UnjustifiedAbsentLabel.Text = unjustifiedAbsents.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public string CalculateAverageGrade(string[] parts)
        {
            float sum = 0;
                for (int i = 0; i < parts.Length - 1 ; i++)
                {
                    sum += float.Parse(parts[i]);
                }
            float average = sum / (parts.Length -1);

            return average.ToString("0.00");
        }
        private void FillStudentInfoLabels()
        {
            try
            {
                Student student = CurrentStudent();
                int departmentId = StudentRepository.GetDepartmentIdByStudent(student);
                int departmentTitle = DepartmentsRepository.GetTitleById(departmentId);
                StudentNameLabel.Text = student.Name;
                StudentSurnameLabel.Text = student.Surname;
                StudentJmbgLabel.Text = student.Jmbg.ToString();
                StudentAddressLabel.Text = student.Address;
                StudentPhoneLabel.Text = student.Phone_number;
                DepartmentLabel.Text = departmentTitle.ToString();
            }
            catch (Exception e )
            {

                MessageBox.Show(e.Message);
            }
        }
        private void FillParentInfoLabels()
        {
            try
            { 
            Parent parent = CurrentParent();
            ParentNameLabel.Text = parent.Name;
            ParentSurnameLabel.Text = parent.Surname;
            ParentAddressLabel.Text = parent.Address;
            ParentEmailLabel.Text = parent.Email;
            ParentPhoneLabel.Text = parent.Phone_number;
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }
        }
        private void LogOutUserButton_Click(object sender, EventArgs e)
        {
                Form form = new LoginForm();
                form.Show();
                this.Close();
        }

        private void UserSettingsButton_Click(object sender, EventArgs e)
        {
            PasswordPanel.Show();
        }

        private void MarksLabel_Click(object sender, EventArgs e)
        {

        }

        private void JustifiedAbsentLabel_Click(object sender, EventArgs e)
        {

        }

        private void UnjustifiedAbsentLabel_Click(object sender, EventArgs e)
        {

        }

        private void PrintStatisticTeacherRoundedButton_Click(object sender, EventArgs e)
        {

            string Query = MarksRepository.GetQuery();

            PdfPTable table = new PdfPTable(4)
            {
                //actual width of table in points
                TotalWidth = 216f,
                //fix the absolute width of the table
                LockedWidth = true
            };

            //relative col widths in proportions - 1/3 and 2/3
            float[] widths = new float[] { 1f, 1f, 1f, 2f };
            table.SetWidths(widths);
            table.HorizontalAlignment = 0;
            //leave a gap before and after the table
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;
            PdfPCell cell = new PdfPCell(new Phrase("Students"))
            {
                Colspan = 2,
                Border = 0,
                HorizontalAlignment = 1
            };
            table.AddCell(cell);

            SqlCeCommand cmd = new SqlCeCommand(Query, Connection);
            SqlCeDataReader reader = cmd.ExecuteReader();



            try
            {
                while (reader.Read())
                {
                    table.AddCell(reader["Name"].ToString());
                    table.AddCell(reader["Surname"].ToString());
                    table.AddCell(reader["Mark"].ToString());
                    table.AddCell(reader["Mark"].ToString());
                    table.AddCell(reader["Mark"].ToString());

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            LoginForm logf = new LoginForm();
            string Dir = logf.GetHomeDirectory();
            using (MemoryStream ms = new MemoryStream())
            {
                FileStream fs = new FileStream("report.pdf", FileMode.Create);
                Document doc = new Document(PageSize.A4);
                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                doc.Add(table);
                while (reader.Read())
                {
                    doc.Add(new Paragraph(reader[0].ToString() + reader[1].ToString() + reader[2].ToString()));
                }

                pdfWriter.CloseStream = true;
                doc.Close();

                Process.Start("report.pdf");
            }



        }

        private void ControlTableButton_Click(object sender, EventArgs e)
        {
            PasswordPanel.Hide();
        }

        private void AddTeacherPasswordButton_Click(object sender, EventArgs e)
        {
            if (OldPassTextBox.Text.Length == 0 ||
                 NewPassTextBox.Text.Length == 0 ||
                 ConfirmedNewPassTextBox.Text.Length == 0)
            {
                Warning();
            }
            else
            {
                if (OldPassTextBox.Text == NewPassTextBox.Text)
                {
                    MessageBox.Show("Unesite novu lozinku koja se razlikuje od stare !");
                }
                else if (OldPassTextBox.Text == CurrentUser.Password && NewPassTextBox.Text == ConfirmedNewPassTextBox.Text)
                {
                   bool isChanged = UsersRepository.ChangePassword(CurrentUser.Id, OldPassTextBox.Text, NewPassTextBox.Text, ConfirmedNewPassTextBox.Text);
                  if(isChanged == true)
                    {
                        PasswordPanel.Hide();
                        UserSettingsButton.Hide();
                    }
                }
                else if (NewPassTextBox.Text != ConfirmedNewPassTextBox.Text)
                    MessageBox.Show("Nove lozinke se ne poklapaju !");
                else MessageBox.Show("Pogrešna lozinka !");

            }
        }

        private void AddMarkButton_Click(object sender, EventArgs e)
        {
           int mark = int.Parse(MarkComboBox.SelectedItem.ToString());
            Student student = CurrentStudent();
            int studentId = StudentRepository.GetIdByJmbg(student.Jmbg);
            int courseId = CoursesRepository.GetIdByTitle(GetCurrentCourse());
            bool isMarkAdded = MarksRepository.InsertMark(mark, studentId, courseId);
            if(isMarkAdded)
            {
                FillStudentMarks();
            }
        }

        private void StudentsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStudentInfoLabels();
            FillParentInfoLabels();
            FillStudentAbsents();
            PopulateCoursesComboBox();// ->> Nedded here because fillstudentmarks()  throws error No data exists for the row/column
            FillStudentMarks();
        }

        private void CoursesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStudentMarks();
        }

        private void AddAbsentButton_Click(object sender, EventArgs e)
        {
            Student student = CurrentStudent();
            int studentId = StudentRepository.GetIdByJmbg(student.Jmbg);
            int hours = int.Parse(AbsentHourComboBox.SelectedItem.ToString());
            bool justified = false;
            if (TrueFalseAbsentComboBox.Text == "Opravdano")
            {
                justified = true;
            }
            bool isAbsentAdded = AbsentsRepository.InsertAbsent(hours, justified, studentId);
            if (isAbsentAdded)
            {
                FillStudentAbsents();
            }
        }
    }
}