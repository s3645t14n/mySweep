using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mySweep
{
    public partial class Form1 : Form
    {
        //private const int size = 20;//размер клетки
        //private const int rows = 30;//строк
        //private const int cols = 50;//столбцов
        //private const int minePer = 15;//% мин
        private int[,] theSea;
        private bool[,] isChecked;
        private int recursionlvl, mines, flags;
        private int size;//размер клетки
        private int rows;//строк
        private int cols;//столбцов
        private int minePer;//% мин
        private bool debug = false;
        private bool helper = true;
        private bool zeroHit;

        public Form1()
        {
            InitializeComponent();
            checkBox2.Checked = false;
            checkBox2.Enabled = false;     
        }

        private void setup(int size, int rows, int cols, int minePer)
        {
            this.Size = new Size(size * cols + 150, size * rows);
            this.CenterToScreen();
            //Image myimage = new Bitmap(@"C:\Users\Себастиан\Documents\Visual Studio 2015\Projects\mySweep\mySweep\bin\Release\satskwp3.jpg");
            //this.BackgroundImage = myimage;
            //this.BackgroundImageLayout = ImageLayout.Stretch;

            mines = 0;
            flags = 0;
            zeroHit = false;

            int left = 150, top = 0;
            theSea = new int[rows, cols];
            Random rand = new Random();

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    if (rand.Next(0, 100) < minePer)
                    {
                        theSea[i, j] = 1;
                        mines++;
                    }
                    else
                        theSea[i, j] = 0;
                }


            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    //создание кнопок
                    Button button = new Button();
                    button.Text = " ";
                    button.Left = left + size * j;
                    button.Top = top + size * i;
                    button.BackColor = System.Drawing.Color.CornflowerBlue;
                    button.FlatAppearance.BorderColor = System.Drawing.Color.White;
                    button.FlatAppearance.BorderSize = 0;
                    button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Lavender;
                    button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
                    button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    button.Name = "b" + i.ToString().PadLeft(5, '0') + j.ToString().PadLeft(5, '0');
                    button.Size = new System.Drawing.Size(size, size);
                    button.TabIndex = 0;
                    button.ForeColor = Color.White;
                    button.Font = new Font("CenturyGothic", (float)(size / 2.5), FontStyle.Bold);
                    //debug
                    if (debug == true && theSea[i, j] == 1) button.Text = "!";
                    this.Controls.Add(button);
                    button.MouseDown += new MouseEventHandler(this.MyButtonHandler);
                }

            label5.Text = "Мин: " + mines;
            label6.Text = "Флагов: " + flags;
        }

        private void MyButtonHandler(object sender, MouseEventArgs e)
        {
            Control btn = (Control)sender;
            if (e.Button == MouseButtons.Left)
            {
                recursionlvl = 0;
                isChecked = new bool[rows, cols];
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        isChecked[i, j] = false;

                btn.BackColor = System.Drawing.Color.Lavender;
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        if (btn.Name == ("b" + i.ToString().PadLeft(5, '0') + j.ToString().PadLeft(5, '0')))
                            check(btn, i, j, true);
            }
            else
            {
                if (btn.Text == " " || btn.Text == "?" || btn.Text == "!")
                {
                    if (btn.Text == "?")
                    {
                        btn.Text = " ";
                        btn.ForeColor = Color.White;
                        //for (int i = 0; i < rows; i++)
                        //    for (int j = 0; j < cols; j++)
                        //        if (btn.Name == ("b" + i.ToString().PadLeft(5, '0') + j.ToString().PadLeft(5, '0')))
                        //            if (theSea[i, j] == 1)
                        //                mines++;
                    }
                    else
                    {
                        btn.ForeColor = Color.Red;
                        btn.Text = "?";
                        //for (int i = 0; i < rows; i++)
                        //    for (int j = 0; j < cols; j++)
                        //        if (btn.Name == ("b" + i.ToString().PadLeft(5, '0') + j.ToString().PadLeft(5, '0')))
                        //            if (theSea[i, j] == 1)
                        //                mines--;
                    }
                }
            }

            if (winCheck() == true)
                MessageBox.Show("Все чисто!", "ГГВП");
        }

        private int check(Control btn, int i, int j, bool isClick)
        {
            recursionlvl++;
            if (theSea[i, j] == 1 && isClick == true)
            {
                if (zeroHit == false)
                {
                    MessageBox.Show("Пока не стоит сюда кликать", "ОП");
                    btn.BackColor = Color.CornflowerBlue;
                    return -1;
                }
                else
                {
                    btn.BackColor = System.Drawing.Color.Red;
                    btn.ForeColor = System.Drawing.Color.White;
                    btn.Text = "X";
                    DialogResult dialogResult = MessageBox.Show("Заново?", "ПОТРАЧЕНО", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        Reset();
                    return -1;//код проигрыша
                }
            }
            else if (theSea[i, j] == 1 && isClick == false)
                return -2;//код остановки рекурсии

            //вычисление числа окружающих клетку мин
            int mCounter = 0;
            for (int m = i - 1; m < i + 2; m++)
                for (int n = j - 1; n < j + 2; n++)
                    if (m >= 0 && n >= 0 && m < rows && n < cols)
                        if (theSea[m, n] == 1)
                            mCounter++;
            if (mCounter != 0)
            {
                btn.Text = mCounter.ToString();
                switch (mCounter) {
                    case 1:
                        btn.ForeColor = Color.HotPink;
                        break;
                    case 2:
                        btn.ForeColor = Color.Green;
                        break;
                    case 3:
                        btn.ForeColor = Color.Blue;
                        break;
                    case 4:
                        btn.ForeColor = Color.Red;
                        break;
                    case 5:
                        btn.ForeColor = Color.Red;
                        break;
                    case 6:
                        btn.ForeColor = Color.Red;
                        break;
                    case 7:
                        btn.ForeColor = Color.Black;
                        break;
                    case 8:
                        btn.ForeColor = Color.Black;
                        break;
                }

                if (btn.BackColor == Color.CornflowerBlue && helper == true)
                {
                    int hCounter = 0;
                    for (int m = i - 1; m < i + 2; m++)
                        for (int n = j - 1; n < j + 2; n++)
                            if (m >= 0 && n >= 0 && m < rows && n < cols)
                                if (this.Controls["b" + m.ToString().PadLeft(5, '0') + n.ToString().PadLeft(5, '0')].Text == " " && this.Controls["b" + m.ToString().PadLeft(5, '0') + n.ToString().PadLeft(5, '0')].BackColor == Color.CornflowerBlue)
                                    hCounter++;
                    //MessageBox.Show(hCounter + " " + mCounter, "debug");
                    if (hCounter == mCounter)
                        for (int m = i - 1; m < i + 2; m++)
                            for (int n = j - 1; n < j + 2; n++)
                                if (m >= 0 && n >= 0 && m < rows && n < cols)
                                    if (this.Controls["b" + m.ToString().PadLeft(5, '0') + n.ToString().PadLeft(5, '0')].Text == " " && this.Controls["b" + m.ToString().PadLeft(5, '0') + n.ToString().PadLeft(5, '0')].BackColor == Color.CornflowerBlue)
                                        this.Controls["b" + m.ToString().PadLeft(5, '0') + n.ToString().PadLeft(5, '0')].BackColor = Color.LawnGreen;
                }
            }
            else
            {
                zeroHit = true;
                for (int m = i - 1; m < i + 2; m++)
                    for (int n = j - 1; n < j + 2; n++)
                        if (m >= 0 && n >= 0 && m < rows && n < cols)
                            if (isChecked[m, n] == false)
                            {
                                isChecked[m, n] = true;
                                check(this.Controls["b" + m.ToString().PadLeft(5, '0') + n.ToString().PadLeft(5, '0')], m, n, false);
                                this.Controls["b" + m.ToString().PadLeft(5, '0') + n.ToString().PadLeft(5, '0')].BackColor = System.Drawing.Color.Lavender;
                            }
            }

            for (int m = i - 1; m < i + 2; m++)
                for (int n = j - 1; n < j + 2; n++)
                    if (m >= 0 && n >= 0 && m < rows && n < cols)
                        if (justACheck(m, n) == 0)
                            if (isChecked[m, n] == false)
                            {
                                isChecked[m, n] = true;
                                check(this.Controls["b" + m.ToString().PadLeft(5, '0') + n.ToString().PadLeft(5, '0')], m, n, false);
                                this.Controls["b" + m.ToString().PadLeft(5, '0') + n.ToString().PadLeft(5, '0')].BackColor = System.Drawing.Color.Lavender;
                            }

            return mCounter;
        }

        private int justACheck(int i, int j)
        {
            int minesC = 0;
            for (int m = i - 1; m < i + 2; m++)
                for (int n = j - 1; n < j + 2; n++)
                    if (m >= 0 && n >= 0 && m < rows && n < cols)
                        if (theSea[m, n] == 1)
                            minesC++;
            return minesC;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            //label1.Visible = false;
            //label2.Visible = false;
            //label3.Visible = false;
            //label4.Visible = false;
            //textBox1.Visible = false;
            //textBox2.Visible = false;
            //textBox3.Visible = false;
            //textBox4.Visible = false;
            //button1.Visible = false;
            //checkBox1.Visible = false;
            try
            {
                size = Int32.Parse(textBox1.Text);
                rows = Int32.Parse(textBox2.Text);
                cols = Int32.Parse(textBox4.Text);
                minePer = Int32.Parse(textBox3.Text);
            }
            catch
            {
                MessageBox.Show("Введи нормальное число", "НЕ НОРМ");
            }
            if (checkBox1.Checked == true) debug = true;
            else debug = false;
            if (checkBox2.Checked == true) helper = true;
            else helper = false;
            this.Controls.Clear();
            this.InitializeComponent();
            setup(size, rows, cols, minePer);
            checkBox1.Checked = debug;
            checkBox2.Checked = helper;
            checkBox2.Checked = false;
            checkBox2.Enabled = false;
            textBox1.Text = size.ToString();
            textBox2.Text = rows.ToString();
            textBox3.Text = minePer.ToString();
            textBox4.Text = cols.ToString();
        }

        private bool winCheck()
        {
            int fCount = 0, fRight = 0;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (this.Controls["b" + i.ToString().PadLeft(5, '0') + j.ToString().PadLeft(5, '0')].Text == "?")
                    {
                        fCount++;
                        flags = fCount;
                        label6.Text = "Флагов: " + flags;
                        if (theSea[i, j] == 1)
                            fRight++;
                    }
            if (fCount == fRight && fRight == mines)
                return true;
            else
                return false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
