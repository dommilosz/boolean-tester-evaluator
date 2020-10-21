using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoolTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            eval();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            createVars(textBox1, checkedListBox1);
            if (auto_eval_box.Checked)
            {
                eval();
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            createVars(textBox2, checkedListBox2);
            if (auto_eval_box.Checked)
            {
                eval();
            }
        }
        public void eval()
        {
            richTextBox1.Text = "X:" + "\n" + textBox1.Text + "\n";
            List<inputObj> inputs = new List<inputObj>();
            foreach (var item in checkedListBox1.Items)
            {
                var str = item.ToString();
                if (checkedListBox1.CheckedItems.Contains(item))
                {
                    inputs.Add(new inputObj(1, str[0]));
                }
                else
                {
                    inputs.Add(new inputObj(0, str[0]));
                }
            }
            var result = BoolLogicBase.evaluateValue(textBox1.Text, inputs.ToArray());
            foreach (var step in result.steps_str)
            {
                richTextBox1.AppendText(step + "\n");
            }
            x_result.Text = $"x = {result.result_str()}";
            if (result.result_str() == null || result.result_str().Length < 1)
            {
                x_result.Text = $"x = ?";
            }

            richTextBox1.AppendText("\n"+"Y:" + "\n" + textBox2.Text + "\n");
            List<inputObj> inputs_y = new List<inputObj>();
            foreach (var item in checkedListBox2.Items)
            {
                var str = item.ToString();
                if (checkedListBox2.CheckedItems.Contains(item))
                {
                    inputs_y.Add(new inputObj(1, str[0]));
                }
                else
                {
                    inputs_y.Add(new inputObj(0, str[0]));
                }
            }

            var result_y = BoolLogicBase.evaluateValue(textBox2.Text, inputs_y.ToArray());
            foreach (var step in result_y.steps_str)
            {
                richTextBox1.AppendText(step + "\n");
            }
            y_result.Text = $"y = {result_y.result_str()}";
            if (result_y.result_str() == null || result_y.result_str().Length < 1)
            {
                y_result.Text = $"y = 0";
            }
            x_eq_y.Text = $"xy = {((result.result_str()==result_y.result_str())?'1':'0')}";

            if(result_y.result_str() == null || result_y.result_str().Length < 1|| result.result_str() == null || result.result_str().Length < 1)
            {
                x_eq_y.Text = $"xy = 0";
            }
            x_eq_y_h.Text = "x<=>y = 0";
            var eq_h_res = BoolLogicBase.isFunctionsEqualsHard(textBox1.Text, textBox2.Text);
            if (eq_h_res.result_str()=="1")
            {
                x_eq_y_h.Text = "x<=>y = 1";
            }
            richTextBox2.Text = "";
            foreach (var item in eq_h_res.steps_str)
            {
                richTextBox2.AppendText(item + "\n");
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (auto_eval_box.Checked)
                nextTickEval = true;
        }
        bool nextTickEval = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (nextTickEval) { eval(); nextTickEval = false; }
        }
        public void createVars(TextBox exp_box,CheckedListBox checkedListBox)
        {
            var chars = exp_box.Text.ToCharArray();
            var oldItems = new List<string>();
            foreach (var item in checkedListBox.CheckedItems)
            {
                oldItems.Add(item.ToString());
            }
            checkedListBox.Items.Clear();
            foreach (var item in chars)
            {
                if (Regex.IsMatch(item.ToString(), "[a-w]"))
                {
                    if(!checkedListBox.Items.Contains(item))
                    checkedListBox.Items.Add(item);
                }
            }
            foreach (var item in oldItems)
            {
                if (checkedListBox.Items.Contains(item[0]))
                {
                    checkedListBox.SetItemChecked((checkedListBox.Items.IndexOf(item[0])), true);
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Preview p = new Preview(textBox1.Text, textBox2.Text);
            //p.ShowDialog();
            label5.Text = boolUtils.toFunctions(textBox1.Text);
        }
    }
}
