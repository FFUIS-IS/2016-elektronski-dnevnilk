using ElectronicSchoolDiary.Models;
using ElectronicSchoolDiary.Repos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Diagnostics;
using System.Data.SqlServerCe;

namespace ElectronicSchoolDiary
{
    public partial class DirectorForm : Form
    {
        private User CurrentUser;
        private Director CurrentDirector;
        private static SqlCeConnection Connection = DataBaseConnection.Instance.Connection;

        public void warning()
        {
            MessageBox.Show("Polja ne mogu biti prazna !");
        }

        public DirectorForm(User user, Director director)
        {
            InitializeComponent();
            this.Text = "Direktor : " + director.Name + " " + director.Surname;
            CurrentUser = user;
            CurrentDirector = director;
        }
        
        private void DirectorForm_Load(object sender, EventArgs e)
        {
            CenterToParent();
            ControlBox = false;
            ChoseComboBox.SelectedIndex = 0;
        }

        private void LogOutUserButton_Click(object sender, EventArgs e)
        {
                Form form = new LoginForm();
                form.Show();
                this.Close();
        }
        private string selectedUser()
        {
            return ChoseComboBox.Text;
        }

        private void ChoseComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string current = selectedUser();
                if (current == "Administracija")
                {
                    AdminPanel.Show();
                    label2.Show();
                    ChoseComboBox.Show();
                    TeacherPanel.Hide();
                    StudentAndParentPanel.Hide();
                    DepartmentsPanel.Hide();
                    PasswordPanel.Hide();
                }
                if (current == "Nastavnici")
                {
                    TeacherPanel.Show();
                    label2.Show();
                    ChoseComboBox.Show();
                    StudentAndParentPanel.Hide();
                    AdminPanel.Hide();
                    DepartmentsPanel.Hide();
                    PasswordPanel.Hide();

                }
                if (current == "Učenici i roditelji")
                {
                    StudentAndParentPanel.Show();
                    label2.Show();
                    ChoseComboBox.Show();
                    AdminPanel.Hide();
                    TeacherPanel.Hide();
                    DepartmentsPanel.Hide();
                    PasswordPanel.Hide();
                }

                if (current == "Odjeljenja")
                {
                    DepartmentsPanel.Show();
                    label2.Show();
                    ChoseComboBox.Show();
                    AdminPanel.Hide();
                    TeacherPanel.Hide();
                    StudentAndParentPanel.Hide();
                    PasswordPanel.Hide();
                }
                if (current == "Izvještaji")
                {
                    AdminPanel.Hide();
                    label2.Show();
                    ChoseComboBox.Show();
                    TeacherPanel.Hide();
                    StudentAndParentPanel.Hide();
                    DepartmentsPanel.Hide();
                    PasswordPanel.Hide();
                }


            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void UserSettingsButton_Click(object sender, EventArgs e)
        {
            PasswordPanel.Show();
            AdminPanel.Hide();
            label2.Hide();
            ChoseComboBox.Hide();
            TeacherPanel.Hide();
            StudentAndParentPanel.Hide();
            DepartmentsPanel.Hide();
        }

        public string CalculateAverageGrade(string[] parts)
        {
            float sum = 0;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                sum += float.Parse(parts[i]);
            }
            float average = sum / (parts.Length - 1);

            return average.ToString("0.00");
        }

        private void CertificateRoundedButton_Click(object sender, EventArgs e)
        {
            int columnNumber = 6;
            PdfPTable table = new PdfPTable(columnNumber)
            {
                //actual width of table in points
                TotalWidth = 400f,
                //fix the absolute width of the table
                LockedWidth = true
            };

            //relative col widths in proportions - 1/3 and 2/3
            float[] widths = new float[] { 1f, 1f, 1f, 1f, 1f, 1f };
            table.SetWidths(widths);
            table.HorizontalAlignment = 0;
            //leave a gap before and after the table
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;
            PdfPCell cell = new PdfPCell(new Phrase("Ucenici"))
            {
                Colspan = columnNumber,
                Border = 0,
                HorizontalAlignment = 1
            };
            table.AddCell(cell);

            table.AddCell("Ime");
            table.AddCell("Prezime");
            table.AddCell("Odsustva(U)");
            table.AddCell("Odsustva(O)");
            table.AddCell("Odsustva(N)");
            table.AddCell("Prosjek");



            int TeacherId = TeacherRepository.GetIdQuery();
            int DepartmentId = DepartmentsRepository.GetIdByTeacherId(TeacherId);
            string StudentsQuery = StudentRepository.GetQuery(DepartmentId);

            SqlCeCommand cmd = new SqlCeCommand(StudentsQuery, Connection);
            SqlCeDataReader reader = cmd.ExecuteReader();


            try
            {
                while (reader.Read())
                {
                    int justifiedAbsents = AbsentsRepository.GetAbsents((int)reader["Id"], 1);
                    int unJustifiedAbsents = AbsentsRepository.GetAbsents((int)reader["Id"], 0);
                    int sum = justifiedAbsents + unJustifiedAbsents;

                    string CoursesId = CoursesRepository.GetCoursesId();
                    string[] parts = CoursesId.Split(',');
                    int suum = 0;
                    for (int i = 0; i < parts.Length; i++)
                    {
                        string marks = MarksRepository.GetMarks((int)reader["Id"]);
                        string[] particles = marks.Split(',');
                        double mark = Math.Round(float.Parse(CalculateAverageGrade(particles)), MidpointRounding.AwayFromZero);

                        suum += (int)mark;
                    }

                    float average = suum / (parts.Length);

                    string averageMark = average.ToString("0.00") + "(" + Math.Round(average, MidpointRounding.AwayFromZero) + ")";

                    table.AddCell(reader["Name"].ToString());
                    table.AddCell(reader["Surname"].ToString());
                    table.AddCell(sum.ToString());
                    table.AddCell(justifiedAbsents.ToString());
                    table.AddCell(unJustifiedAbsents.ToString());
                    table.AddCell(averageMark);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            LoginForm logf = new LoginForm();
            string Dir = logf.GetHomeDirectory();

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

        private void roundedButton1_Click(object sender, EventArgs e)
        {

        }

        private void ControlTableButton_Click(object sender, EventArgs e)
        {
            ChoseComboBox.SelectedIndex = 0;
            AdminPanel.Show();
            label2.Show();
            ChoseComboBox.Show();
            TeacherPanel.Hide();
            StudentAndParentPanel.Hide();
            DepartmentsPanel.Hide();
            PasswordPanel.Hide();
        }

        private void ChangeTeacherInfoButton_Click(object sender, EventArgs e)
        {
            if(
                TeacherNameTextBox.Text.Length == 0 ||
                TeacherSurnameTextBox.Text.Length == 0 )
            {
                warning();
            }
        }

        private void ChangeAdminInfoRoundedButton_Click(object sender, EventArgs e)
        {
            if(AdminNameTextBox.Text.Length == 0 || AdminSurnameTextBox.Text.Length ==0)
            {
                warning();
            }
        }

        private void AddDirectorPasswordButton_Click(object sender, EventArgs e)
        {
            if (OldPassTextBox.Text.Length == 0 ||
                  NewPassTextBox.Text.Length == 0 ||
                  ConfirmedNewPassTextBox.Text.Length == 0)
            {
                warning();
            }
            else
            {
                if (OldPassTextBox.Text == NewPassTextBox.Text)
                {
                    MessageBox.Show("Unesite novu lozinku koja se razlikuje od stare !");
                }
                else if (OldPassTextBox.Text == CurrentUser.Password && NewPassTextBox.Text == ConfirmedNewPassTextBox.Text)
                {
                   bool isChanged =  UsersRepository.ChangePassword(CurrentUser.Id, OldPassTextBox.Text, NewPassTextBox.Text, ConfirmedNewPassTextBox.Text);
                   if(isChanged == true)
                    {
                        ChoseComboBox.SelectedIndex = 0;
                        AdminPanel.Show();
                        label2.Show();
                        ChoseComboBox.Show();
                        TeacherPanel.Hide();
                        StudentAndParentPanel.Hide();
                        DepartmentsPanel.Hide();
                        PasswordPanel.Hide();
                        UserSettingsButton.Hide();
                    }
                }
                else if (NewPassTextBox.Text != ConfirmedNewPassTextBox.Text)
                    MessageBox.Show("Nove lozinke se ne poklapaju !");
                else MessageBox.Show("Pogrešna lozinka !");

            }
        }
    }
}
