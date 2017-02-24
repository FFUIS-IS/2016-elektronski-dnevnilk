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
using System.Data.SqlServerCe;

namespace ElectronicSchoolDiary
{
    public partial class DirectorForm : Form
    {
        private User CurrentUser;
        private Director CurrentDirector;
        private SqlCeConnection connection = DataBaseConnection.Instance.Connection;
        private SqlCeCommand command;
        private SqlCeDataReader reader;
        private int selectedItem;

        public void warning()
        {
            MessageBox.Show("Polja ne mogu biti prazna !");
        }

        public DirectorForm(User user, Director director)
        {
            command = connection.CreateCommand();
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
            fillTeachers();
            fillAdmins();
            fillStudents();
            fillDepartments();
        }

        private void fillTeachers()
        {
            TeachersListBox.Items.Clear();
            command.CommandText = "SELECT name, surname FROM Teachers;";
            connection.Close();
            connection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                TeachersListBox.Items.Add(reader.GetString(0) + " " + reader.GetString(1));
            }
            connection.Close();
        }

        private void fillAdmins()
        {
            AdministratorsListBox.Items.Clear();
            command.CommandText = "SELECT name, surname FROM Administration;";
            connection.Close();
            connection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                AdministratorsListBox.Items.Add(reader.GetString(0) + " " + reader.GetString(1));
            }
            connection.Close();
        }

        private void fillStudents()
        {
            StudentsListBox.Items.Clear();
            command.CommandText = "SELECT name, surname FROM Students;";
            connection.Close();
            connection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                StudentsListBox.Items.Add(reader.GetString(0) + " " + reader.GetString(1));
            }
            connection.Close();
        }

        private void fillDepartments()
        {
            DepartmentsListBox.Items.Clear();
            command.CommandText = "SELECT ClassesId, Title FROM Departments;";
            connection.Close();
            connection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                DepartmentsListBox.Items.Add(reader.GetInt32(0) + "-" + reader.GetInt32(1));
            }
            connection.Close();
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

        

        private void CertificateRoundedButton_Click(object sender, EventArgs e)
        {
           
        }

        private void roundedButton1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Da li ste sigurni da zelite da izbrisete?", "Upozorenje!", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                command.CommandText = "DELETE FROM Students WHERE Id = " + selectedItem + ";";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Ucenik je izbrisan iz baze");
                fillStudents();
            }
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
            if (
                TeacherNameTextBox.Text.Length == 0 ||
                TeacherSurnameTextBox.Text.Length == 0)
            {
                warning();
            }
            else
            {
                string adress = (string.IsNullOrEmpty(TeacherAddressTextBox.Text) ? ("N/a") : (TeacherAddressTextBox.Text));
                string phone = (string.IsNullOrEmpty(TeacherPhoneNumberTextBox.Text) ? ("N/a") : (TeacherPhoneNumberTextBox.Text));
                command.CommandText = "UPDATE teachers SET name = '" + TeacherNameTextBox.Text + "'"
                    + ", surname = '" + TeacherSurnameTextBox.Text + "', address = '" + adress + "', "
                    + " phone_number = '" + phone + "' WHERE usersID = " + int.Parse(usernameTextBox.Text) + ";";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Nastavnik je prepravljen u bazi");
                fillTeachers();
            }
        }

        private void ChangeAdminInfoRoundedButton_Click(object sender, EventArgs e)
        {
            if (AdminNameTextBox.Text.Length == 0 || AdminSurnameTextBox.Text.Length == 0)
            {
                warning();
            }
            else
            {
                command.CommandText = "UPDATE Administration SET name = '" + AdminNameTextBox.Text + "'"
                    + ", surname = '" + AdminSurnameTextBox.Text + ";";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Administrator je prepravljen u bazi");
                fillAdmins();
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
                    bool isChanged = UsersRepository.ChangePassword(CurrentUser.Id, OldPassTextBox.Text, NewPassTextBox.Text, ConfirmedNewPassTextBox.Text);
                    if (isChanged == true)
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

        private void TeachersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] name = TeachersListBox.SelectedItem.ToString().Split(' ');
            command.CommandText = "SELECT Address, Phone_number, UsersId FROM Teachers WHERE Name = '"
                + name[0] + "' AND Surname = '" + name[1] + "';";
            connection.Open();
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                TeacherAddressTextBox.Text = reader.GetString(0);
                TeacherNameTextBox.Text = name[0];
                TeacherSurnameTextBox.Text = name[1];
                TeacherPhoneNumberTextBox.Text = reader.GetString(1);
                usernameTextBox.Text = "" + reader.GetInt32(2);
            }
            connection.Close();
        }

        private void RemoveTeacherButton_Click(object sender, EventArgs e)
        {
            if (
               TeacherNameTextBox.Text.Length == 0 ||
               TeacherSurnameTextBox.Text.Length == 0)
            {
                warning();
            }
            else
            {
                DialogResult result = MessageBox.Show("Da li ste sigurni da zelite da izbrisete " + TeacherNameTextBox.Text + " " + TeacherSurnameTextBox.Text + "?", "Upozorenje!", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    command.CommandText = "DELETE FROM Teachers WHERE UsersID = " + int.Parse(usernameTextBox.Text) + ";";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    MessageBox.Show("Nastavnik je izbrisna iz baze");
                    fillTeachers();
                }
            }
        }

        private void AdministratorsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] name = AdministratorsListBox.SelectedItem.ToString().Split(' ');
            command.CommandText = "SELECT Name, Surname, id FROM Administration WHERE Name = '"
                + name[0] + "' AND Surname = '" + name[1] + "';";
            connection.Open();
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                AdminNameTextBox.Text = name[0];
                AdminSurnameTextBox.Text = name[1];
                selectedItem = reader.GetInt32(2);
            }
            connection.Close();
        }

        private void RemoveAdministratorButton_Click(object sender, EventArgs e)
        {
            if (
               AdminNameTextBox.Text.Length == 0 ||
               AdminSurnameTextBox.Text.Length == 0)
            {
                warning();
            }
            else
            {
                DialogResult result = MessageBox.Show("Da li ste sigurni da zelite da izbrisete " + AdminNameTextBox.Text + " " + AdminSurnameTextBox.Text + "?", "Upozorenje!", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    command.CommandText = "DELETE FROM Administration WHERE UsersID = " + selectedItem + ";";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    MessageBox.Show("Administrator je izbrisan iz baze");
                    fillAdmins();
                }
            }
        }

        private void StudentsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillAbsents(StudentsListBox.SelectedItem.ToString(), StudentJustifiedAbsentLabel1, StudentUnjustifiedAbsentLabel1);
            fillCourses(CoursesComboBox, StudentsListBox.SelectedItem.ToString());
            fillParents();
        }
        private void fillParents()
        {
            command.CommandText = "SELECT * FROM Parents WHERE studentsID = " + selectedItem + ";";
            connection.Open();
            reader = command.ExecuteReader();
            while(reader.Read())
            {
                ParentNameLabel.Text = reader.GetString(1);
                ParentSurnameLabel.Text = reader.GetString(2);
                ParentAddressLabel.Text = reader.GetString(3);
                ParentEmailLabel.Text = reader.GetString(4);
                ParentPhoneLabel.Text = reader.GetString(5);
            }

            connection.Close();
        }
        private void fillCourses(ComboBox box, string studentName)
        {
            box.Items.Clear();

            string[] name = studentName.Split(' ');
            command.CommandText = "SELECT departmentsId FROM students where name = '" + name[0] + "' AND surname = '" + name[1] + "';";
            connection.Open();
            reader = command.ExecuteReader();
            reader.Read();
            int department = reader.GetInt32(0);
            reader.Close();
            command.CommandText = "SELECT CoursesID FROM Teachers_departments_Courses WHERE departmentsID = " + department + ";";
            reader = command.ExecuteReader();
            SqlCeCommand reserveCommand = connection.CreateCommand();
            SqlCeDataReader reserveReader;
            while (reader.Read())
            {
                reserveCommand.CommandText = "SELECT Title FROM Courses WHERE id = " + reader.GetInt32(0) + ";";
                reserveReader = reserveCommand.ExecuteReader();
                reserveReader.Read();
                box.Items.Add(reserveReader.GetString(0));
            }
            connection.Close();
        }

        private void fillAbsents(string studentName, Label justifiedLabel, Label unjustifiedLabel)
        {
            string[] name = studentName.Split(' ');
            command.CommandText = "SELECT id from students where name = '" + name[0] + "' AND surname = '" + name[1] + "';";
            connection.Open();
             reader = command.ExecuteReader();
            
                reader.Read();

            
            selectedItem = reader.GetInt32(0);
            command.CommandText = "SELECT hour, justified FROM Absents WHERE studentsid = " + reader.GetInt32(0) + ";";
            reader.Close();
            reader = command.ExecuteReader();
            int justified = 0;
            int unjustified = 0;
            while (reader.Read())
            {
                if (reader.GetByte(1) == 1)
                    justified += reader.GetInt32(0);
                else
                    unjustified += reader.GetInt32(0);
            }
            justifiedLabel.Text = justified.ToString();
            unjustifiedLabel.Text = unjustified.ToString();
            connection.Close();
        }
        private void RemoveDepartmentButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Da li ste sigurni da zelite da izbrisete?", "Upozorenje!", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                command.CommandText = "DELETE FROM Departments WHERE Id = " + selectedItem + ";";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Odjeljenje je izbrisano iz baze");
                fillDepartments();
            }
        }

        private void DepartmentsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            StudentsComboBox.Items.Clear();
            comboBox1.Items.Clear();
            int justifiedApsent = 0;
            int unjustifiedApsent = 0;
            int numberOfMarks = 0;
            int sumOfMarks = 0;

            string[] type = DepartmentsListBox.SelectedItem.ToString().Split('-');
            command.CommandText = "SELECT id FROM departments WHERE title = '" + type[1] + "' AND classesID = " + int.Parse(type[0]) +";";
            connection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                SqlCeDataReader studentReader = new SqlCeCommand("SELECT * FROM students WHERE departmentsID = " + reader.GetInt32(0) + ";", connection).ExecuteReader();
                SqlCeDataReader coursesReader = new SqlCeCommand("SELECT id FROM courses WHERE classesID = " + int.Parse(type[0]) + ";", connection).ExecuteReader();
                while (coursesReader.Read())
                {
                    while (studentReader.Read())
                    {
                        StudentsComboBox.Items.Add(studentReader.GetString(1) + " " + studentReader.GetString(2));
                        SqlCeDataReader justifiedReader = 
                            new SqlCeCommand("SELECT hour, justified FROM Absents WHERE studentsID = " 
                            + studentReader.GetInt32(0) + ";", connection).ExecuteReader();
                        while(justifiedReader.Read())
                        {
                            if (justifiedReader.GetByte(1) == 1)
                                justifiedApsent += justifiedReader.GetInt32(0);
                            else
                                unjustifiedApsent += justifiedReader.GetInt32(0);
                        }
                    
                            SqlCeDataReader markReader = new SqlCeCommand("SELECT Mark FROM Marks WHERE studentsID = " + studentReader.GetInt32(0) + " AND coursesID = " + coursesReader.GetInt32(0) + ";", connection).ExecuteReader();
                            while(markReader.Read())
                            {
                                numberOfMarks++;
                                sumOfMarks += markReader.GetInt32(0);
                            }
                    }
                    SqlCeDataReader teacherReader = new SqlCeCommand("SELECT teachersID from Teachers_Departments_Courses WHERE coursesID = " 
                        + coursesReader.GetInt32(0) + " AND DepartmentsId = " + reader.GetInt32(0) + ";" , connection).ExecuteReader();
                    if (teacherReader.Read())
                    {
                        int teacherID = teacherReader.GetInt32(0);
                        teacherReader.Close();
                        teacherReader = new SqlCeCommand("SELECT * FROM Teachers WHERE id = " + teacherID + ";", connection).ExecuteReader();
                        teacherReader.Read();
                        TeacherNameLabel.Text = teacherReader.GetString(1);
                        TeacherSurnameLabel.Text = teacherReader.GetString(2);
                        TeacherAddressLabel.Text = teacherReader.GetString(3);
                        TeacherPhoneLabel.Text = teacherReader.GetString(5);
                    }
                }
            }
            if (numberOfMarks == 0)
                DepartmentAverageMarkLabel.Text = "0";
            else
                DepartmentAverageMarkLabel.Text = (sumOfMarks / numberOfMarks).ToString();
            DepartmentJustifiedAbsentsLabel.Text = justifiedApsent.ToString();
            DepartmentUnjustifiedAbsentsLabel.Text = unjustifiedApsent.ToString();
  
            connection.Close();
        }
        

        private void CoursesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            StudentMarkLabel1.Text = "";
            command.CommandText = "SELECT id FROM Courses WHERE title = '" + CoursesComboBox.SelectedItem.ToString() + "';";
            connection.Open();
            reader = command.ExecuteReader();
            reader.Read();
            int courseID = reader.GetInt32(0);
            reader.Close();
            command.CommandText = "SELECT mark FROM Marks WHERE studentsID = " + selectedItem
                + " AND coursesID = " + courseID + ";";
            reader = command.ExecuteReader();
            while(reader.Read())
                StudentMarkLabel1.Text += reader.GetInt32(0).ToString() + ", ";
            connection.Close();
        }

        private void StudentsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillCourses(comboBox1, StudentsComboBox.SelectedItem.ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillAbsents(StudentsComboBox.SelectedItem.ToString(), StudentJustifiedAbsentsLabel, StudentUnjustifiedAbsentsLabel);
            fillMarks(StudentsComboBox.SelectedItem.ToString(), comboBox1.SelectedItem.ToString(), StudentMarkLabel);
        }

        private void fillMarks(string studentName, string course, Label label)
        {
            label.Text = "";
            connection.Open();
            command.CommandText = "SELECT id from courses WHERE title = '" + course + "';";
            reader = command.ExecuteReader();
            reader.Read();
            int courseID = reader.GetInt32(0);
            reader.Close();
            string[] name = studentName.Split(' ');
            command.CommandText = "SELECT id from students WHERE name = '" + name[0] + "' AND surname = '" + name[1] + "';";
            reader = command.ExecuteReader();
            reader.Read();
            int studentID = reader.GetInt32(0);
            reader.Close();
            command.CommandText = "SELECT mark from Marks WHERE studentsID = " + studentID + " AND coursesID = " + courseID + ";";
            reader = command.ExecuteReader();
            while (reader.Read())
                label.Text += reader.GetInt32(0) + ", ";
            connection.Close();
        }
    }
}
