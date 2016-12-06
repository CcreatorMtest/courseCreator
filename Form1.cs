using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseCreater
{
    public partial class CourseCreater : Form
    {
        bool isTasksHaveChanges = false;

        public CourseCreater()
        {
            InitializeComponent();
            TaskListBox.DataSource = thisCourseTasks;

            answerBoxForOneAnswer[0] = new AnswerBox(AnswerTextBox1, AnswerCheckBox1);
            answerBoxForOneAnswer[1] = new AnswerBox(AnswerTextBox2, AnswerCheckBox2);
            answerBoxForOneAnswer[2] = new AnswerBox(AnswerTextBox3, AnswerCheckBox3);
            answerBoxForOneAnswer[3] = new AnswerBox(AnswerTextBox4, AnswerCheckBox4);

            answerBoxForAllAnswers[0] = new AnswerBox(AnswerTextBox_1, AnswerCheckBox_1);
            answerBoxForAllAnswers[1] = new AnswerBox(AnswerTextBox_2, AnswerCheckBox_2);
            answerBoxForAllAnswers[2] = new AnswerBox(AnswerTextBox_3, AnswerCheckBox_3);
            answerBoxForAllAnswers[3] = new AnswerBox(AnswerTextBox_4, AnswerCheckBox_4);

            Init();
            
        }
        
        private void TaskListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSavingNow)
                return;
            if (!isTaskSaved)
            {
                DialogResult result = MessageBox.Show("Задача не была сохранена. Уверены, что хотите перейти к другой", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.No)
                    return;
            }

            nCurrentTask = TaskListBox.SelectedIndex;
            Task tmpTask = new Task((Task)course.Tasks.GetValue(TaskListBox.SelectedIndex));
            TaskNameBox.Text = tmpTask.Name;
            TaskRedaktorBox.Text = tmpTask.Text;
            num = tmpTask.AnswerType;
            if (num != 2)
            {
                if (tmpTask.AnswerType == 0)
                {
                    AnswerTypeBox.Text = "Выбрать единственный верный";
                    ChoiseOneAnswerPanel.Visible = true;
                    ChoiseAllAnswersPanel.Visible = false;
                    InputAnswerPanel.Visible = false;
                }
                else
                {
                    AnswerTypeBox.Text = "Выбрать все верные";
                    ChoiseOneAnswerPanel.Visible = false;
                    ChoiseAllAnswersPanel.Visible = true;
                    InputAnswerPanel.Visible = false;
                }

                for (int i = 0; i < 4; i++)
                {
                    if (i < tmpTask.TrueAnswers.Length)
                    {
                        answerBoxForOneAnswer[i].SelectTask_true(tmpTask.TrueAnswers[i]);
                        answerBoxForAllAnswers[i].SelectTask_true(tmpTask.TrueAnswers[i]);
                    }
                    else if (i - tmpTask.TrueAnswers.Length < tmpTask.FalseAnswers.Length)
                    {
                        int j = i - tmpTask.TrueAnswers.Length;
                        answerBoxForOneAnswer[i].SelectTask_false(tmpTask.FalseAnswers[j]);
                        answerBoxForAllAnswers[i].SelectTask_false(tmpTask.FalseAnswers[j]);
                    }
                    else
                    {
                        answerBoxForOneAnswer[i].SelectTask_None("Пусто");
                        answerBoxForAllAnswers[i].SelectTask_None("Пусто");
                    }
                }
            }
            else
            {
                AnswerTypeBox.Text = "Ввести ответ";
                ChoiseOneAnswerPanel.Visible = false;
                ChoiseAllAnswersPanel.Visible = false;
                InputAnswerPanel.Visible = true;
                InputTextAnswerBox.Text = "";
                InputTextAnswerBox.ForeColor = Color.MidnightBlue;
                for (int i = 0; i < tmpTask.TrueAnswers.Length; i++)
                {
                    InputTextAnswerBox.Text += tmpTask.TrueAnswers[i];
                    if(i != tmpTask.TrueAnswers.Length - 1)
                        InputTextAnswerBox.Text += Environment.NewLine;
                }
            }

            if (TaskListBox.SelectedIndex == 0)
                RpevTaskButton.Enabled = false;
            else
                RpevTaskButton.Enabled = true;
            if (TaskListBox.SelectedIndex == course.Tasks.Length - 1)
                NextTaskButton.Enabled = false;
            else
                NextTaskButton.Enabled = true;

            TaskRedaktorPanel.Visible = true;
            AnswerPanel.Visible = true;
            TaskNamePanel.Visible = true;

            isTaskSaved = true;
            TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = false;
        }

        private void RpevTaskButton_Click(object sender, EventArgs e)
        {
            PrevTask();
        }

        private void NextTaskButton_Click(object sender, EventArgs e)
        {
            NextTask();
        }

        private void TaskBackButton_Click(object sender, EventArgs e)
        {
            TaskBack();
        }

        private void TaskCloseButton_Click(object sender, EventArgs e)
        {
            if (!isTaskSaved)
            {
                DialogResult result =  MessageBox.Show("Задача не была сохранена. Уверены, что хотите её закрыть", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if(result == System.Windows.Forms.DialogResult.No)
                    return;
            }
            TaskRedaktorPanel.Visible = false;
            AnswerPanel.Visible = false;
            TaskNamePanel.Visible = false;
        }

        private void AnswerTypeBox_TextUpdate(object sender, EventArgs e)
        {
            if (num == 0)
                AnswerTypeBox.Text = "Выбрать единственный верный";
            else if (num == 1)
                AnswerTypeBox.Text = "Выбрать все верные";
            else if (num == 2)
                AnswerTypeBox.Text = "Ввести ответ";
            else
                AnswerTypeBox.Text = "";
        }

        private void AnswerTypeButton_Click(object sender, EventArgs e)
        {
            isTaskSaved = false;
            TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true;

            num = AnswerTypeBox.SelectedIndex;
            if (num != 2)
            {
                if (num == 0)
                {
                    AnswerTypeBox.Text = "Выбрать единственный верный";
                    ChoiseOneAnswerPanel.Visible = true;
                    ChoiseAllAnswersPanel.Visible = false;
                    InputAnswerPanel.Visible = false;
                }
                else
                {
                    AnswerTypeBox.Text = "Выбрать все верные";
                    ChoiseOneAnswerPanel.Visible = false;
                    ChoiseAllAnswersPanel.Visible = true;
                    InputAnswerPanel.Visible = false;
                }
                AnswerTextBox_1.Text = AnswerTextBox1.Text = "Вариант ответа 1";
                AnswerCheckBox_1.Checked = AnswerCheckBox1.Checked = true;
                AnswerTextBox_1.ForeColor = AnswerTextBox1.ForeColor = Color.LightSlateGray;
                AnswerCheckBox_1.Enabled = AnswerCheckBox1.Enabled = true;
                AnswerTextBox_2.Text = AnswerTextBox2.Text = "Вариант ответа 2";
                AnswerCheckBox_2.Checked = AnswerCheckBox2.Checked = false;
                AnswerTextBox_2.ForeColor = AnswerTextBox2.ForeColor = Color.LightSlateGray;
                AnswerCheckBox_2.Enabled = AnswerCheckBox2.Enabled = true;
                AnswerTextBox_3.Text = AnswerTextBox3.Text = "Пусто";
                AnswerCheckBox_3.Checked = AnswerCheckBox3.Checked = false;
                AnswerTextBox_3.ForeColor = AnswerTextBox3.ForeColor = Color.LightSlateGray;
                AnswerCheckBox_3.Enabled = AnswerCheckBox3.Enabled = false;
                AnswerTextBox_4.Text = AnswerTextBox4.Text = "Пусто";
                AnswerCheckBox_4.Checked = AnswerCheckBox4.Checked = false;
                AnswerTextBox_4.ForeColor = AnswerTextBox4.ForeColor = Color.LightSlateGray;
                AnswerCheckBox_4.Enabled = AnswerCheckBox4.Enabled = false;
            }
            else
            {
                AnswerTypeBox.Text = "Ввести ответ";
                ChoiseOneAnswerPanel.Visible = false;
                ChoiseAllAnswersPanel.Visible = false;
                InputAnswerPanel.Visible = true;
                InputTextAnswerBox.ForeColor = Color.LightSlateGray;
                InputTextAnswerBox.Text = "Допустимый ответ 1" + Environment.NewLine + "Допустимый ответ 2";
            }
        }

        private void AnswerTextBox1_Leave(object sender, EventArgs e) { answerBoxForOneAnswer[0].AnswerBoxLeave(); }
        private void AnswerTextBox2_Leave(object sender, EventArgs e) { answerBoxForOneAnswer[1].AnswerBoxLeave(); }
        private void AnswerTextBox3_Leave(object sender, EventArgs e) { answerBoxForOneAnswer[2].AnswerBoxLeave(); }
        private void AnswerTextBox4_Leave(object sender, EventArgs e) { answerBoxForOneAnswer[3].AnswerBoxLeave(); }
        private void AnswerTextBox1_Click(object sender, EventArgs e) {answerBoxForOneAnswer[0].AnswerTextBoxClick();}
        private void AnswerTextBox2_Click(object sender, EventArgs e) {answerBoxForOneAnswer[1].AnswerTextBoxClick();}
        private void AnswerTextBox3_Click(object sender, EventArgs e) {answerBoxForOneAnswer[2].AnswerTextBoxClick();}
        private void AnswerTextBox4_Click(object sender, EventArgs e) {answerBoxForOneAnswer[3].AnswerTextBoxClick();}
        private void AnswerCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            isTaskSaved = false;
            TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true;

            if (AnswerCheckBox1.Checked == true)
            {
                AnswerCheckBox2.Checked = false;
                AnswerCheckBox3.Checked = false;
                AnswerCheckBox4.Checked = false;
            }
        }
        private void AnswerCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            isTaskSaved = false;
            TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true;

            if (AnswerCheckBox2.Checked == true)
            {
                AnswerCheckBox1.Checked = false;
                AnswerCheckBox3.Checked = false;
                AnswerCheckBox4.Checked = false;
            }
        }
        private void AnswerCheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            isTaskSaved = false;
            TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true;

            if (AnswerCheckBox3.Checked == true)
            {
                AnswerCheckBox1.Checked = false;
                AnswerCheckBox2.Checked = false;
                AnswerCheckBox4.Checked = false;
            }
        }
        private void AnswerCheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            isTaskSaved = false;
            TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true;

            if (AnswerCheckBox4.Checked == true)
            {
                AnswerCheckBox1.Checked = false;
                AnswerCheckBox2.Checked = false;
                AnswerCheckBox3.Checked = false;
            }
        }
        private void AnswerTextBox1_TextChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; answerBoxForOneAnswer[0].AnswerTextBoxTextChanged();}
        private void AnswerTextBox2_TextChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; answerBoxForOneAnswer[1].AnswerTextBoxTextChanged();}
        private void AnswerTextBox3_TextChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; answerBoxForOneAnswer[2].AnswerTextBoxTextChanged();}
        private void AnswerTextBox4_TextChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; answerBoxForOneAnswer[3].AnswerTextBoxTextChanged();}

        private void AnswerTextBox_1_Leave(object sender, EventArgs e) {answerBoxForAllAnswers[0].AnswerBoxLeave();}
        private void AnswerTextBox_2_Leave(object sender, EventArgs e) {answerBoxForAllAnswers[1].AnswerBoxLeave();}
        private void AnswerTextBox_3_Leave(object sender, EventArgs e) {answerBoxForAllAnswers[2].AnswerBoxLeave();}
        private void AnswerTextBox_4_Leave(object sender, EventArgs e) {answerBoxForAllAnswers[3].AnswerBoxLeave();}
        private void AnswerTextBox_1_Click(object sender, EventArgs e) {answerBoxForAllAnswers[0].AnswerTextBoxClick();}
        private void AnswerTextBox_2_Click(object sender, EventArgs e) {answerBoxForAllAnswers[1].AnswerTextBoxClick();}
        private void AnswerTextBox_3_Click(object sender, EventArgs e) {answerBoxForAllAnswers[2].AnswerTextBoxClick();}
        private void AnswerTextBox_4_Click(object sender, EventArgs e) {answerBoxForAllAnswers[3].AnswerTextBoxClick();}
        private void AnswerTextBox_1_TextChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; answerBoxForAllAnswers[0].AnswerTextBoxTextChanged(); }
        private void AnswerTextBox_2_TextChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; answerBoxForAllAnswers[1].AnswerTextBoxTextChanged(); }
        private void AnswerTextBox_3_TextChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; answerBoxForAllAnswers[2].AnswerTextBoxTextChanged(); }
        private void AnswerTextBox_4_TextChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; answerBoxForAllAnswers[3].AnswerTextBoxTextChanged(); }

        private void InputTextAnswerBox_Leave(object sender, EventArgs e)
        {
            if (InputTextAnswerBox.Text == "")
            {
                InputTextAnswerBox.ForeColor = Color.LightSlateGray;
                InputTextAnswerBox.Text = "Допустимый ответ 1" + Environment.NewLine + "Допустимый ответ 2";
            }
            else
            {
                InputTextAnswerBox.ForeColor = System.Drawing.Color.MidnightBlue;
            }
        }
        private void InputTextAnswerBox_Click(object sender, EventArgs e)
        {
            if (InputTextAnswerBox.ForeColor == Color.LightSlateGray)
            {
                InputTextAnswerBox.Text = "";
                InputTextAnswerBox.ForeColor = System.Drawing.Color.MidnightBlue;
            }
        }
        private void InputTextAnswerBox_TextChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; }

        public class AnswerBox
        {
            public TextBox textBox;
            public CheckBox checkBox;

            public AnswerBox(TextBox textBox, CheckBox checkBox)
            {
                this.textBox = textBox;
                this.checkBox = checkBox;
            }

            public void AnswerBoxLeave()
            {
                if (textBox.Text == "")
                {
                    textBox.ForeColor = Color.LightSlateGray;
                    textBox.Text = "Пусто";
                    checkBox.Checked = false;
                    checkBox.Enabled = false;
                }
                else
                {
                    textBox.ForeColor = System.Drawing.Color.MidnightBlue;
                    checkBox.Enabled = true;
                }
            }

            public void AnswerTextBoxClick()
            {
                if (textBox.ForeColor == Color.LightSlateGray)
                    textBox.Text = "";
            }

            public void AnswerTextBoxTextChanged()
            {

                if (textBox.Text == "")
                    checkBox.Enabled = false;
                else if (textBox.Text != "Пусто" || textBox.ForeColor != Color.LightSlateGray)
                {
                    checkBox.Enabled = true;
                    textBox.ForeColor = System.Drawing.Color.MidnightBlue;
                }
            }

            public void SelectTask_true(string Answ)
            {
                textBox.Text = Answ;
                checkBox.Checked = true;
                textBox.ForeColor = System.Drawing.Color.MidnightBlue;
                checkBox.Enabled = true;
            }

            public void SelectTask_false(string Answ)
            {
                textBox.Text = Answ;
                checkBox.Checked = false;
                textBox.ForeColor = System.Drawing.Color.MidnightBlue;
                checkBox.Enabled = true;
            }

            public void SelectTask_None(string Answ)
            {
                textBox.Text = Answ;
                checkBox.Checked = false;
                textBox.ForeColor = Color.LightSlateGray;
                checkBox.Enabled = false;
            }
    }

        private void TaskSaveButton_Click(object sender, EventArgs e)
        {
            SaveTask();
        }

        private void TaskNameBox_TextChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; }
        private void TaskRedaktorBox_TextChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; }
        private void AnswerCheckBox_1_CheckedChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; }
        private void AnswerCheckBox_2_CheckedChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; }
        private void AnswerCheckBox_3_CheckedChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; }
        private void AnswerCheckBox_4_CheckedChanged(object sender, EventArgs e) {isTaskSaved = false; TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true; }

        private void TaskCreateButton_Click(object sender, EventArgs e)
        {
            if (!isTaskSaved)
            {
                DialogResult result = MessageBox.Show("Задача не была сохранена. Уверены, что хотите перейти к другой", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return;
            }

            if (course.Tasks != null)
                nCurrentTask = course.Tasks.Length;
            else
                nCurrentTask = 0;

            TaskNameBox.Text = "";
            TaskRedaktorBox.Text = "";
            num = 2;
            AnswerTypeBox.Text = "Ввести ответ";
            ChoiseOneAnswerPanel.Visible = false;
            ChoiseAllAnswersPanel.Visible = false;
            InputAnswerPanel.Visible = true;
            InputTextAnswerBox.Text = "Допустимый ответ 1" + Environment.NewLine + "Допустимый ответ 2";
            InputTextAnswerBox.ForeColor = Color.LightSlateGray;

            isTaskSaved = false;
            TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true;

            isSavingNow = true;
            TaskListBox.SelectedIndex = -1;
            isSavingNow = false;
            RpevTaskButton.Enabled = false;
            NextTaskButton.Enabled = false;
            
            TaskRedaktorPanel.Visible = true;
            AnswerPanel.Visible = true;
            TaskNamePanel.Visible = true;
        }

        private void SaveCourseButton_Click(object sender, EventArgs e)
        {

            if (!isTaskSaved)
            {
                DialogResult result = MessageBox.Show("Не все задачи были сохранены. Вы уверены, что хотите сохранить курс не учитывая последние изменения задачи", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return;
            }
            if (course.fileName != null)
            {
                DialogResult res = MessageBox.Show("Вы хотите сохранить курс в старом файле? (при отрицательном ответе вам будет предоставлена возможность создания нового файла, для сохранения курса в нём)", "СОХРАНЕНИЕ КУРСА", MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Cancel)
                    return;
                if (res == DialogResult.Yes)
                {
                    SaveCourse();
                    return;
                }
            }
            SaveAsCourse();
        }
        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(!isTaskSaved)
            {
                DialogResult res =  MessageBox.Show("Не все задачи были сохранены. Вы уверены, что хотите сохранить курс не учитывая последние изменения задачи", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if (res == DialogResult.No)
                    return;
            }
            SaveAsCourse();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCourse();
        }
        
        private void курсToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (!isTaskSaved)
            {
                DialogResult result = MessageBox.Show("Не все задачи были сохранены. Вы уверены, что хотите сохранить курс не учитывая последние изменения задачи", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return;
            }
            if (course.fileName != null)
            {
                SaveCourse();
                return;
            }
            SaveAsCourse();
        }

        private void задачуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTask();
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isTaskSaved)
            {
                DialogResult res = MessageBox.Show("Вы не сохранили задачу. Если вы создадите новый курс, её последние измененения будут утеряны. Открыть новый курс?", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if (res == DialogResult.No)
                    return;
            }
            if(isTasksHaveChanges == true)
            {
                DialogResult result = MessageBox.Show("В старом курсе были изменены и сохранены задачи. Возможно, вам стоило сохранить последнюю версию того курса. Создать новый курс не сохранив старый?", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel)
                    return;
            }
            //TaskPanels.Visible = true;

            course.fileName = null;
            course.Name = null;
            course.Tasks = null;

            isSavingNow = true;
            nCurrentTask = -1;
            thisCourseTasks.Clear();
            TaskListBox.SelectedIndex = -1;
            isSavingNow = false;
            isTasksHaveChanges = false;
            picturePanel.Visible = false;

            TaskRedaktorPanel.Visible = false;
            AnswerPanel.Visible = false;
            TaskNamePanel.Visible = false;
        }

        private void курсToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CourseBack();
        }

        private void задачиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskBack();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isTaskSaved)
            {
                DialogResult res = MessageBox.Show("Вы не сохранили задачу. Если вы создадите новый курс, её последние измененения будут утеряны. Открыть новый курс?", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if (res == DialogResult.No)
                    return;
            }
            if (isTasksHaveChanges == true)
            {
                DialogResult result = MessageBox.Show("В старом курсе были изменены и сохранены задачи. Возможно, вам стоило сохранить последнюю версию того курса. Создать новый курс не сохранив старый?", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel)
                    return;
            }
            //TaskPanels.Visible = true;

            course.fileName = null;
            course.Name = null;
            course.Tasks = null;

            isSavingNow = true;
            nCurrentTask = -1;
            thisCourseTasks.Clear();
            TaskListBox.SelectedIndex = -1;
            isSavingNow = false;
            isTasksHaveChanges = false;
            picturePanel.Visible = false;

            TaskRedaktorPanel.Visible = false;
            AnswerPanel.Visible = false;
            TaskNamePanel.Visible = false;
            panel2.Visible = false;
        }

        private void OpenCoursebtn_Click(object sender, EventArgs e)
        {
            OpenCourse();
            panel2.Visible = false;
        }

        private void добавитьЗадачуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isTaskSaved)
            {
                DialogResult result = MessageBox.Show("Задача не была сохранена. Уверены, что хотите перейти к другой", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return;
            }

            if (course.Tasks != null)
                nCurrentTask = course.Tasks.Length;
            else
                nCurrentTask = 0;

            TaskNameBox.Text = "";
            TaskRedaktorBox.Text = "";
            num = 2;
            AnswerTypeBox.Text = "Ввести ответ";
            ChoiseOneAnswerPanel.Visible = false;
            ChoiseAllAnswersPanel.Visible = false;
            InputAnswerPanel.Visible = true;
            InputTextAnswerBox.Text = "Допустимый ответ 1" + Environment.NewLine + "Допустимый ответ 2";
            InputTextAnswerBox.ForeColor = Color.LightSlateGray;

            isTaskSaved = false;
            TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = true;

            isSavingNow = true;
            TaskListBox.SelectedIndex = -1;
            isSavingNow = false;
            RpevTaskButton.Enabled = false;
            NextTaskButton.Enabled = false;

            TaskRedaktorPanel.Visible = true;
            AnswerPanel.Visible = true;
            TaskNamePanel.Visible = true;
        }
    }
}
