﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace weixinDemo
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。

            listBoxContact.Sorted = true;
            listBoxContact.Font = new Font("微软雅黑", 12);
            listBoxContact.DisplayMember = "text";
            listBoxContact.ValueMember = "value";
            listBoxContact.ScrollAlwaysVisible = false;
        }

        public void SetContact(JArray contactList)
        {
            //WriteLog("SetContact开始......");
            foreach (JObject contact in contactList)
            {
                ListItem listItem = new ListItem() { text = ((JValue)contact["NickName"]).Value.ToString(), value = ((JValue)contact["UserName"]).Value.ToString() };
                //listBox1.Items.Add(listItem);
                AddListItem(listItem);
                //FormLogin.instance.startUI.GetContactHeadImg(listItem.value);
            }
        }

        public void AddListItem(ListItem listItem)
        {
            if (listBoxContact.InvokeRequired)
            {
                Action<ListItem> actionDelegate = delegate (ListItem item) { listBoxContact.Items.Add(listItem); };
                listBoxContact.Invoke(actionDelegate, listItem);
            }
            else
            {
                listBoxContact.Items.Add(listItem);
            }
        }

        public void AddtextBoxHistory(string msg)
        {
            msg += "\n";
            if (textBoxHistory.InvokeRequired)
            {
                Action<string> actionDelegate = delegate (string item) {
                    textBoxHistory.AppendText(item);
                    textBoxHistory.SelectionStart = textBoxHistory.Text.Length;
                    textBoxHistory.ScrollToCaret();
                };
                textBoxHistory.Invoke(actionDelegate, msg);
            }
            else
            {
                textBoxHistory.AppendText(msg);
                textBoxHistory.SelectionStart = textBoxHistory.Text.Length;
                textBoxHistory.ScrollToCaret();
            }
        }
        public void SetTitle(string msg)
        {
            msg += "\n";
            if (this.InvokeRequired)
            {
                Action<string> actionDelegate = delegate (string item) { this.Text = item; };
                this.Invoke(actionDelegate, msg);
            }
            else
            {
                this.Text = msg;
            }
        }
        //public void SetShow(bool isVisable)
        //{
        //    if (this.InvokeRequired)
        //    {
        //        Action<bool> actionDelegate = delegate (bool isVis) { this.Visible = isVis;  }; //this.Show();
        //        this.Invoke(actionDelegate, isVisable);
        //    }
        //    else
        //    {
        //        this.Visible = isVisable;
        //        //this.Show();
        //    }
        //}
        public bool isCurr(string userName)
        {
            if (lblNickName.Tag != null && userName!=null && ((ListItem)lblNickName.Tag).value==userName)
            {
                return true;
            }
            return false;
        }
        private void listBoxContact_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 60;
        }

        private void listBoxContact_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListItem li = (ListItem)listBoxContact.SelectedItem;
            //Dictionary<string,string> dict = FormLogin.instance.startUI.getUserById(li.value);
            //dict["NickName"]
            if (li != null)
            {
                lblNickName.Text = li.text;
                lblNickName.Tag = li;

                textBoxHistory.Clear();
                //List<UserMessage> userMsg = FormLogin.instance.startUI.listUserMsg.FindAll(m => m.getFromUserName() == li.value || m.getToUserName() == li.value);
                //foreach (UserMessage um in userMsg)
                //{
                //    textBoxHistory.AppendText(um. um.getLog();
                //}
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxInput.Text.Trim()))
            {
                MessageBox.Show("不能发送空白信息！");
                return;
            }
            if (lblNickName.Tag == null)
            {
                MessageBox.Show("请先选定一个联系人！");
                return;
            }
            FormLogin.instance.startUI.sendText(textBoxInput.Text, ((ListItem)lblNickName.Tag).value);
            textBoxHistory.AppendText(DateTime.Now.ToString(" hh:mm:ss ")+"我：" + textBoxInput.Text + "\r\n");
            //chatListBox1.Items.Add(new CCWin.SkinControl.ChatListItem() {  Text = textBoxInput.Text });
            textBoxInput.Text = "";
        }

        private void textBoxInput_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend.PerformClick();
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                FormLogin.instance.Close();
                FormLogin.instance.t.Abort();
            }
            catch
            {

            }
        }
    }
}
