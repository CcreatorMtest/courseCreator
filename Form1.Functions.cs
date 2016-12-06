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
        IBindingList thisCourseTasks = new BindingList<string>();
        int num = -1;
        int nCurrentTask = -1;
        bool isSavingNow = false;
        bool isTaskSaved = true;
        public List<AnswerBox> oneCheckedAnswerBoxes = new List<AnswerBox>();
        public List<AnswerBox> multyCheckedAnswerBoxes = new List<AnswerBox>();
        AnswerBox[] answerBoxForOneAnswer = new AnswerBox[4];
        AnswerBox[] answerBoxForAllAnswers = new AnswerBox[4];
        Course course = new Course(null, null);

        private void Init()
        {
            course = new Course(null, null);
            course.fileName = null;

            thisCourseTasks.Clear();
            TaskListBox.SelectedIndex = -1;
            isSavingNow = true;
            nCurrentTask = -1;
            isSavingNow = false;

            TaskPanels.Visible = false;
        }

        private Task SaveInputTask()
        {
            Task tmpTask = new Task();
            if (InputTextAnswerBox.ForeColor == Color.LightSlateGray)
            {
                MessageBox.Show("Мне кажется, хотя бы один допустимый вариант ответа необходим. Если вы считаете верным ответом отсутствие какого-либо ответа нажмите Enter в поле для ввода допустимых ответов", "ОШИБКА");
                return null;
            }
            string InputTextAnswer = InputTextAnswerBox.Text.Replace(Environment.NewLine, "~");
            string[] trueAnswer = InputTextAnswer.Split('~');
            if (trueAnswer.Contains(""))
            {
                DialogResult res = MessageBox.Show("В числе допустимых ответов имеются пустые строки. Если вы не считаете отсутствие ответа допустимым ответом, то нажмите отмену и удалите лишние переходы на следующую строку", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel)
                    return null;
            }
            tmpTask.TrueAnswers = trueAnswer;
            tmpTask.FalseAnswers = new string[0];
            return tmpTask;
        }

        private Task SaveChoiseTask()
        {
            Task tmpTask = new Task();
            int nTrueAnsw = 0;
            int[] iTrueAnsw = { -1, -1, -1, -1 };
            int nFalseAnsw = 0;
            int[] iFalseAnsw = { -1, -1, -1, -1 };
            AnswerBox[] answerBoxForAnswer = new AnswerBox[4];
            if (num == 0)
                answerBoxForAnswer = answerBoxForOneAnswer;
            else
                answerBoxForAnswer = answerBoxForAllAnswers;
            for (int i = 0; i < 4; i++)
            {
                if (answerBoxForAnswer[i].textBox.ForeColor == Color.MidnightBlue && answerBoxForAnswer[i].checkBox.Enabled)
                {
                    if (answerBoxForAnswer[i].checkBox.Checked)
                    {
                        iTrueAnsw[nTrueAnsw] = i;
                        nTrueAnsw++;
                    }
                    else
                    {
                        iFalseAnsw[nFalseAnsw] = i;
                        nFalseAnsw++;
                    }
                }

            }
            string[] trueAnswer = new string[nTrueAnsw];
            string[] falseAnswer = new string[nFalseAnsw];
            int j = 0;
            foreach (int i in iTrueAnsw)
            {
                if (i == -1)
                    break;
                trueAnswer[j] = answerBoxForAnswer[i].textBox.Text;
                j++;
            }
            j = 0;
            foreach (int i in iFalseAnsw)
            {
                if (i == -1)
                    break;
                falseAnswer[j] = answerBoxForAnswer[i].textBox.Text;
                j++;
            }
            if (nFalseAnsw + nTrueAnsw == 0)
            {
                MessageBox.Show("Вы не ввели ни одного варианта ответа. Это не правильно. Исправьтесь, пожалуйста", "ОШИБКА");
                return null;
            }
            if (num == 0 && nTrueAnsw == 0)
            {
                MessageBox.Show("В задачах такого типа необходим один верный ответ. Выберете его или поменяйте тип задачи", "ОШИБКА");
                return null;
            }
            if (num == 1 && nTrueAnsw == 0)
            {
                DialogResult result = MessageBox.Show("Вы не отметили/не ввели ни одного верного ответа. Вы хотите сохранить текущий вариант задачи?", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return null;
            }
            if (num == 1 && nFalseAnsw == 0)
            {
                DialogResult result = MessageBox.Show("Вы отметили все ответы как верные. Вы хотите сохранить текущий вариант задачи?", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return null;
            }
            tmpTask.TrueAnswers = trueAnswer;
            tmpTask.FalseAnswers = falseAnswer;
            return tmpTask;
        }

        private void SaveTask()
        {
            if (TaskNameBox.Text == "")
            {
                MessageBox.Show("Введите, пожалуйста, имя задачи", "ОШИБКА");
                return;
            }
            if (TaskRedaktorBox.Text == "")
            {
                MessageBox.Show("Вы забыли ввести условие задачи. Пожалуйста, исправьте это недоразумение", "ОШИБКА");
                return;
            }
            Task tmpTask;
            if (num == 2)
            {
                tmpTask = SaveInputTask();
            }
            else
            {
                tmpTask = SaveChoiseTask();
            }
            if (tmpTask == null)
                return;
            tmpTask.Name = TaskNameBox.Text;
            tmpTask.Text = TaskRedaktorBox.Text;
            tmpTask.AnswerType = num;

            if (TaskListBox.SelectedIndex != -1)
                course.Tasks.SetValue(tmpTask, TaskListBox.SelectedIndex);
            else
                course.Add(tmpTask);

            isSavingNow = true;
            thisCourseTasks.Clear();
            int itask = 1;
            foreach (Task t in course.Tasks)
            {
                string stask = itask.ToString() + ". " + t.Name;
                thisCourseTasks.Add(stask);
                itask++;
            }
            TaskListBox.SelectedIndex = nCurrentTask;
            isSavingNow = false;

            isTaskSaved = true;
            TaskSaveButton.Enabled = false;
            задачуToolStripMenuItem.Enabled = false;
            isTasksHaveChanges = true;
        }

        private void SaveCourse()
        {
            if (File.Exists(course.fileName))
            {
                File.Delete(course.fileName);
            }
            using (FileStream fs = File.Create(course.fileName))
            {
                AddText(fs, course.ToMyString());
            }
            isTasksHaveChanges = false;
        }

        private void SaveAsCourse()
        {
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                if (course.Name == null)
                    course.Name = course.ToName(saveFileDialog1.FileName);
                FileStream fs = (FileStream)saveFileDialog1.OpenFile();
                AddText(fs, course.ToMyString());
                fs.Close();
                Text = course.Name + " - CourseCreater";
            }
            isTasksHaveChanges = false;
        }

        private void CourseBack()
        {
            isSavingNow = true;
            string FileName = course.fileName;
            StreamReader sr = new StreamReader(course.fileName);
            course = new Course(sr.ReadToEnd());
            thisCourseTasks.Clear();
            Text = course.Name + " - CourseCreater";
            int itask = 1;
            foreach (Task t in course.Tasks)
            {
                string stask = itask.ToString() + ". " + t.Name;
                thisCourseTasks.Add(stask);
                itask++;
            }
            TaskListBox.SelectedIndex = -1;
            isSavingNow = false;
            TaskRedaktorPanel.Visible = false;
            AnswerPanel.Visible = false;
            TaskNamePanel.Visible = false;

            course.fileName = FileName;
            sr.Close();
            isTasksHaveChanges = false;
            TaskPanels.Visible = true;
            picturePanel.Visible = false;
        }

        private void OpenCourse()
        {
            if (!isTaskSaved)
            {
                DialogResult res = MessageBox.Show("Вы не сохранили задачу. Если вы откроете новый курс, её последние измененения будут утеряны. Открыть новый курс?", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if (res == DialogResult.No)
                    return;
            }
            if (isTasksHaveChanges == true)
            {
                DialogResult result = MessageBox.Show("В старом курсе были изменены и сохранены задачи. Возможно, вам стоило сохранить последнюю версию того курса. Открыть другой курс не сохранив старый?", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel)
                    return;
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                isSavingNow = true;
                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                course = new Course(sr.ReadToEnd());
                thisCourseTasks.Clear();
                Text = course.Name + " - CourseCreater";
                int itask = 1;
                foreach (Task t in course.Tasks)
                {
                    string stask = itask.ToString() + ". " + t.Name;
                    thisCourseTasks.Add(stask);
                    itask++;
                }
                TaskListBox.SelectedIndex = -1;
                isSavingNow = false;
                TaskRedaktorPanel.Visible = false;
                AnswerPanel.Visible = false;
                TaskNamePanel.Visible = false;

                course.fileName = openFileDialog1.FileName;
                sr.Close();
                isTasksHaveChanges = false;
                TaskPanels.Visible = true;
                picturePanel.Visible = false;
            }
        }

        private void NextTask()
        {
            if (!isTaskSaved)
            {
                DialogResult result = MessageBox.Show("Задача не была сохранена. Уверены, что хотите перейти к другой", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.No)
                    return;
            }
            isTaskSaved = true;
            TaskSaveButton.Enabled = false;
            задачуToolStripMenuItem.Enabled = false;

            TaskListBox.SelectedIndex += 1;
        }

        private void PrevTask()
        {
            if (!isTaskSaved)
            {
                DialogResult result = MessageBox.Show("Задача не была сохранена. Уверены, что хотите перейти к другой", "ПРЕДУПРЕЖДЕНИЕ", MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.No)
                    return;
            }
            isTaskSaved = true;
            TaskSaveButton.Enabled = false;
            задачуToolStripMenuItem.Enabled = false;

            TaskListBox.SelectedIndex -= 1;
        }

        private void TaskBack()
        {
            if (TaskListBox.SelectedIndex == -1)
            {
                TaskNameBox.Text = "";
                TaskRedaktorBox.Text = "";
                num = 2;
                AnswerTypeBox.Text = "Ввести ответ";
                ChoiseOneAnswerPanel.Visible = false;
                ChoiseAllAnswersPanel.Visible = false;
                InputAnswerPanel.Visible = true;
                InputTextAnswerBox.Text = "Допустимый ответ 1" + Environment.NewLine + "Допустимый ответ 2";
                InputTextAnswerBox.ForeColor = Color.LightSlateGray;
                return;
            }

            Task tmpTask = new Task((Task)course.Tasks.GetValue(TaskListBox.SelectedIndex));
            TaskNameBox.Text = tmpTask.Name;
            TaskRedaktorBox.Text = tmpTask.Text;
            num = tmpTask.AnswerType;
            if (tmpTask.AnswerType != 2)
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
                InputTextAnswerBox.ForeColor = System.Drawing.Color.MidnightBlue;
                foreach (string str in tmpTask.TrueAnswers)
                {
                    InputTextAnswerBox.Text += str;
                    InputTextAnswerBox.Text += Environment.NewLine;
                }
            }
            isTaskSaved = true;
            TaskSaveButton.Enabled = задачуToolStripMenuItem.Enabled = false;
        }

        private Task SelectTask( int i )
        {
            Task task = new Task();
            return task;
        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
    }

    public class Task
    {
        public string Name;
        public string Text;
        public int AnswerType;
        public string[] TrueAnswers;
        public string[] FalseAnswers;

        public Task(string Name, string Text, int AnswerType, string[] TrueAnswers, string[] FalseAnswers)
        {
            this.Name = Name;
            this.Text = Text;
            this.AnswerType = AnswerType;
            this.TrueAnswers = TrueAnswers;
            this.FalseAnswers = FalseAnswers;
        }
    
        public Task()
        {
            Name = "";
            Text = "";
            AnswerType = 2;
            TrueAnswers = new string[1];
            FalseAnswers = new string[1];
        }

        public Task(int i)
        {
            Name = "Untitled" + i.ToString();
            Text = "";
            AnswerType = 2;
            TrueAnswers = new string[1];
            FalseAnswers = new string[1];
        }

        public Task(string name)
        {
            Name = name;
            Text = "";
            AnswerType = 2;
            TrueAnswers = new string[1];
            FalseAnswers = new string[1];
        }

        public Task(Task sourceTask)
        {
            Name = sourceTask.Name;
            Text = sourceTask.Text;
            AnswerType = sourceTask.AnswerType;
            TrueAnswers = sourceTask.TrueAnswers;
            FalseAnswers = sourceTask.FalseAnswers;
        }
    }
    
    public class Course
    {
        public string Name;
        public Array Tasks;
        public string fileName;
    
        public Course(string Name, Array Tasks)
        {
            this.Name = Name;
            this.Tasks = Tasks;
        }

        public Course(string fileName, string Name, Array Tasks)
        {
            this.fileName = fileName;
            this.Name = Name;
            this.Tasks = Tasks;
        }

        public Course(string courseStr)
        {
            char task_split = (char)8;
            char partTask_split = (char)7;

            string[] tmp = courseStr.Split(task_split);

            Name = tmp[0];
            Tasks = Array.CreateInstance(typeof(Task), tmp.Length - 1);
            for (int i = 1; i < tmp.Length; i++)
            {
                Task task = new Task();
                string[] taskStr = tmp[i].Split(partTask_split);
    
                task.Name = taskStr[0].Substring(1);
                task.Text = taskStr[1];
                if (taskStr[0][0] == 'a')
                {
                    task.AnswerType = 2;
                    task.TrueAnswers = new string[taskStr.Length - 2];
                    for (int j = 2; j < taskStr.Length; j++)
                    {
                        task.TrueAnswers[j - 2] += taskStr[j];
                    }
                }
                else
                {
                    if (taskStr[0][0] == 's')
                    {
                        task.AnswerType = 0;
                        string sTrueAnsw = taskStr[taskStr.Length-1];
                        int iTrueAnsw = sTrueAnsw[0] - '0';
                        task.TrueAnswers = new string[1];
                        task.TrueAnswers[0] = taskStr[iTrueAnsw + 2];
                        task.FalseAnswers = new string[taskStr.Length - 4];
                        for (int j = 2, k = 0; j < taskStr.Length - 1; j++)
                            if(j != iTrueAnsw + 2)
                            {
                                task.FalseAnswers[k] = taskStr[j];
                                k++;
                            }
                    }
                    if (taskStr[0][0] == 'd')
                    {
                        task.AnswerType = 1;
                        string sTrueAnsw = taskStr[taskStr.Length - 1];
                        int[] iTrueAnsw;
                        if (sTrueAnsw[0] == '-')
                            iTrueAnsw = new int[0];
                        else
                        {
                            iTrueAnsw = new int[sTrueAnsw.Length];
                            for (int j = 0; j < sTrueAnsw.Length; j++)
                                iTrueAnsw[j] = sTrueAnsw[j] - '0';
                        }
                        task.TrueAnswers = new string[iTrueAnsw.Length];
                        task.FalseAnswers = new string[taskStr.Length - 3 - iTrueAnsw.Length];
                        for (int j = 0, jTrue = 0, jFalse = 0; j < taskStr.Length - 3; j++)
                        {
                            if(iTrueAnsw.Contains(j))
                            {
                                task.TrueAnswers[jTrue] = taskStr[j + 2];
                                jTrue++;
                            }
                            else
                            {
                                task.FalseAnswers[jFalse] = taskStr[j + 2];
                                jFalse++;
                            }
                        }
                    }
                }
                Tasks.SetValue(task, i - 1);
            }
        }
    
        public void Add(Task newTask)
        {
            if (Tasks != null)
            {
                Array Tasks1 = Array.CreateInstance(typeof(Task), Tasks.Length + 1);
                Tasks.CopyTo(Tasks1, 0);
                Tasks1.SetValue(newTask, Tasks1.Length - 1);
                Tasks = Tasks1;
                return;
            }
            Tasks = Array.CreateInstance(typeof(Task), 1);
            Tasks.SetValue(newTask, 0);
        }
    
        public string ToMyString()
        {//96*96.png 72*72 48*48
            string res = Name;
            if (Tasks == null)
                return res;
            foreach (Task task in Tasks)
            {
                char task_split = (char)8;
                char partTask_split = (char)7;
                res += task_split;
                if (task.AnswerType == 0)
                    res += "s";
                if (task.AnswerType == 1)
                    res += "d";
                if (task.AnswerType == 2)
                    res += "a";
                res += task.Name + partTask_split + task.Text;
                if (task.AnswerType != 2)
                {
                    if (task.TrueAnswers.Length == 0)
                    {
                        foreach (string answ in task.FalseAnswers)
                            res += partTask_split + answ;
                        res += partTask_split + "-";
                    }
                    else
                    {
                        int[] iTrueAnsw = new int[task.TrueAnswers.Length];
                        SortedDictionary<int, string> map = new SortedDictionary<int, string>();
                        Random rand = new Random();
                        foreach (string answ in task.TrueAnswers)
                        {
                            map.Add(rand.Next(), answ);
                        }
                        foreach (string answ in task.FalseAnswers)
                        {
                            map.Add(rand.Next(), answ);
                        }
                        int numTrueAnsw = 0;
                        int numAnsw = 0;
                        foreach(string answer in map.Values)
                        {
                            res += partTask_split + answer;
                            if (task.TrueAnswers.Contains(answer))
                            {
                                iTrueAnsw[numTrueAnsw] = numAnsw;
                                numTrueAnsw++;
                            }
                            numAnsw++;
                        }
                        res += partTask_split;
                        foreach(int k in iTrueAnsw)
                            res += k.ToString();
                    }
                }
                else
                {
                    foreach (string answ in task.TrueAnswers)
                        res += partTask_split + answ;
                }
            }
            return res;
        }
    
        public string ToName(string fileName)
    {
        string[] answ = fileName.Split('\\');
        string[] res = answ[answ.Length - 1].Split('.');
        return res[0];
    }
    }
}
