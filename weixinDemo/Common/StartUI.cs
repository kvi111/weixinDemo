using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace weixinDemo
{
    public class StartUI : WechatApi
    {
        //private static final ExecutorService executorService = Executors.newFixedThreadPool(3);
        public List<GroupMessage> listGroupMsg = new List<GroupMessage>();
        public List<UserMessage> listUserMsg = new List<UserMessage>();

        private MessageHandle messageHandle;

        public StartUI() : base(Environment.empty())
        {
            //setMsgHandle(new TulingRobot(environment));
            //start();
        }

        public void setMsgHandle(MessageHandle messageHandle)
        {
            this.messageHandle = messageHandle;
        }

        private void waitForLogin()
        {
            while (true)
            {
                Console.WriteLine(Const.LOG_MSG_SCAN_QRCODE);
                if (!waitforlogin(1))
                {
                    continue;
                }
                break;
            }
            while (true)
            {
                Console.WriteLine(Const.LOG_MSG_CONFIRM_LOGIN);
                if (!waitforlogin(0))
                {
                    continue;
                }
                break;
            }
        }

        /**
         * 启动机器人
         */
        public void start()
        {
            Console.WriteLine(Const.LOG_MSG_START);
            Console.WriteLine(Const.LOG_MSG_TRY_INIT);

            if (getUUID()==false) //获取uuid
            {
                Console.WriteLine(Const.LOG_MSG_FAIL);
                return;
            }
            
            Byte[] byteData = genqrcode();//获取二维码
            if (byteData.Length > 1000)
            {
                FormLogin.instance.SetQRImage(byteData);
            }
            else
            {
                Console.WriteLine(Const.LOG_MSG_FAIL);
                return;
            }

            waitForLogin();//等待用户扫描、确定
            
            if (login()==false) //登录
            {
                Console.WriteLine(Const.LOG_MSG_FAIL);
                return;
            }
            
            if (webwxinit()==false) //初始化
            {
                Console.WriteLine(Const.LOG_MSG_FAIL);
                return;
            }

            if (openStatusNotify()==false) //开启微信状态通知
            {
                Console.WriteLine(Const.LOG_MSG_FAIL);
                return;
            }
            
            if (getContact()==false) //获取联系人
            {
                Console.WriteLine(Const.LOG_MSG_FAIL);
                return;
            }

            //填充联系人列表
            //if (groupList.size() > 0)
            //{
            //    executorService.execute(new Runnable() {
            //        @Override
            //        public void run()
            //    {
            //        Console.WriteLine(Const.LOG_MSG_GET_GROUP_MEMBER);
            //        StartUI.super.fetchGroupContacts();
            //    }
            //});
            //}

            FormLogin.instance.SetVisable(false);
            FormLogin.instance.ShowMain();
            FormLogin.formMain.SetContact(this.contactList);

            snapshot(); //保存配置

            this.listen();
        }

        /// <summary>
        /// retcode: 0 正常，1100 失败/退出微信
        /// selector:0 正常，2 新的消息，7 进入/离开聊天界面
        /// </summary>
        private void listen()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("listen synccheck ...");
                    //retcode, selector
                    int[] checkResponse = synccheck();
                    int retcode = checkResponse[0];
                    int selector = checkResponse[1];
                    Console.WriteLine("retcode: {0}, selector: {1}", retcode, selector);
                    switch (retcode)
                    {
                        case 1100:
                            Console.WriteLine(Const.LOG_MSG_LOGOUT);
                            break;
                        case 1101:
                            Console.WriteLine(Const.LOG_MSG_LOGIN_OTHERWHERE);
                            break;
                        case 1102:
                            Console.WriteLine(Const.LOG_MSG_QUIT_ON_PHONE);
                            break;
                        case 0:
                            this.handle(selector);
                            break;
                        default:
                            Console.WriteLine("wxSync: {0}\n", wxSync().ToString());
                            break;
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("listen synccheck 异常：{0}",ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 和微信保持同步，获取最新消息
        /// </summary>
        /// <returns></returns>
        public JObject wxSync()
        {
            String url = conf["API_webwxsync"] + "?sid={0}&skey={1}&pass_ticket={2}";
            url = String.Format(url, session.getSid(), session.getSkey(), session.getPassTicket());

            Dictionary<String, Object> params1 = new Dictionary<String, Object>();
            params1.Add("BaseRequest", this.baseRequest);
            params1.Add("SyncKey", this.synckeyDic);
            params1.Add("rr", Utils.currentTimeMillis());

            JObject response = doPost(url, params1);
            if (null == response)
            {
                return null;
            }

            //JsonObject dic = response.getAsJsonObject();
            if (null != response)
            {
                JToken baseResponse = response["BaseResponse"];
                if (null != baseResponse && ((JValue)baseResponse["Ret"]).Value.ToString() == "0")
                {
                    this.makeSynckey(response);
                }
            }
            return response;
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="selector"></param>
        private void handle(int selector)
        {
            switch (selector)
            {
                case 2:
                    JObject dic = wxSync();
                    if (null != dic)
                    {
                        handle_msg(dic);
                    }
                    break;
                case 7:
                    wxSync();
                    break;
                case 0:
                    Thread.Sleep(1000);
                    break;
                case 4:
                    // 保存群聊到通讯录
                    // 修改群名称
                    // 新增或删除联系人
                    // 群聊成员数目变化
                    dic = wxSync();
                    if (null != dic)
                    {
                        handle_mod(dic);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 处理发来的消息
        /// </summary>
        /// <param name="dic"></param>
        public void handle_msg(JObject dic)
        {
            Console.WriteLine("handle message");
            if (null != messageHandle)
            {
                messageHandle.WxSync(dic);
            }

            int intMegCount = dic["AddMsgList"].Count();
            if (intMegCount == 0)
            {
                return;
            }

            Console.WriteLine(Const.LOG_MSG_NEW_MSG, intMegCount);

            JArray msgs = (JArray)dic["AddMsgList"];
            foreach (JObject element in msgs)
            {
                //JsonObject msg = element.getAsJsonObject();
                String msgType = ((JValue)element["MsgType"]).Value.ToString();
                String msgId = ((JValue)element["MsgId"]).Value.ToString();
                String content = ((JValue)element["Content"]).Value.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                UserMessage userMessage = new UserMessage(this);
                userMessage.setRawMsg(element);

                // 文本groupMessage
                if (conf["MSGTYPE_TEXT"]== msgType)
                {
                    // 地理位置消息
                    if (content.Contains("pictype=location"))
                    {
                        String location = content.Split(new string[] { "<br/>" },StringSplitOptions.None)[1];
                        userMessage.setLocation(location);
                        userMessage.setLog(String.Format(Const.LOG_MSG_LOCATION, location));
                    }
                    else
                    {
                        // 普通文本
                        String text = null;
                        if (content.Contains(":<br/>"))
                        {
                            text = content.Split(new string[] { ":<br/>" }, StringSplitOptions.None)[1];
                        }
                        else
                        {
                            text = content;
                        }
                        userMessage.setText(text);
                        userMessage.setLog(text.Replace("<br/>", "\n"));
                    }
                }
                else if (conf["MSGTYPE_STATUSNOTIFY"]==msgType)
                {
                    Console.WriteLine(Const.LOG_MSG_NOTIFY_PHONE);
                    return;
                }

                this.show_msg(userMessage);

                Boolean isGroupMsg = (((JValue)element["FromUserName"]).Value.ToString() + ((JValue)element["ToUserName"]).Value.ToString()).Contains("@@");
                if (isGroupMsg)
                {
                    GroupMessage groupMessage = make_group_msg(userMessage);
                    if (null != messageHandle)
                    {
                        messageHandle.groupMessage(groupMessage);
                    }
                }
                else
                {
                    if (null != messageHandle)
                    {
                        messageHandle.userMessage(userMessage);
                    }
                }
            }
        }
        private void show_msg(UserMessage userMessage)
        {
            Dictionary<String, String> src = new Dictionary<string, string>() { { "","" }};
            Dictionary<String, String> dst = null;
            Dictionary<String, String> group = null;
            JObject msg = userMessage.getRawMsg();

            String content = ((JValue)msg["Content"]).Value.ToString();
            content = content.Replace("&lt;", "<").Replace("&gt;", ">");

            String msg_id = ((JValue)msg["MsgId"]).Value.ToString();

            // 接收到来自群的消息
            if (((JValue)msg["FromUserName"]).Value.ToString().Substring(0,2)=="@@")
            {
                String groupId = ((JValue)msg["FromUserName"]).Value.ToString();
                group = this.getGroupById(groupId);
                if (content.Contains(":<br/>"))
                {
                    String u_id = content.Split(new string[] { ":<br/>" }, StringSplitOptions.None)[0];
                    src = this.getGroupUserById(u_id, groupId);
                    dst = new Dictionary<string, string>() { { "ShowName", "GROUP" }};
                }
                else
                {
                    String u_id = ((JValue)msg["ToUserName"]).Value.ToString();
                    src = new Dictionary<string, string>() { { "ShowName", "SYSTEM" } };
                    dst = getGroupUserById(u_id, groupId);
                }
            }
            else
            {
                // 非群聊消息
                src = this.getUserById(((JValue)msg["FromUserName"]).Value.ToString());
                dst = this.getUserById(((JValue)msg["ToUserName"]).Value.ToString());
            }


            if (null != group)
            {
                Console.WriteLine("{0} |{1}| {2} -> {3}: {4}\n", msg_id, group["ShowName"], src["ShowName"],
                        dst["ShowName"], userMessage.getLog());
            }
            else
            {
                string newMsg = string.Format(DateTime.Now.ToString(" hh:mm:ss ") + "{0} {1} -> {2}: {3}\n", msg_id, src["ShowName"],
                        dst["ShowName"], userMessage.getLog());
                
                Console.WriteLine(newMsg);
            }
            //显示到界面上
            string showMsg = DateTime.Now.ToString(" hh:mm:ss ") + src["ShowName"] + "：" + userMessage.getLog() + "\r\n";
            if (FormLogin.formMain.isCurr(src["UserName"])) //发来消息是否来自于当前聊天窗口用户
            {
                FormLogin.formMain.AddtextBoxHistory(showMsg);
            }
            else
            {
                FormLogin.formMain.SetTitle(showMsg);
            }
            listUserMsg.Add(userMessage);
        }

        private void handle_mod(JObject dic)
        {
            Console.WriteLine("handle modify");
            handle_msg(dic);

            JArray modContactList = (JArray)dic["ModContactList"];
            foreach (JObject contact in modContactList)
            {
                //JsonObject m = element.getAsJsonObject();
                string userName = ((JValue)contact["UserName"]).Value.ToString();
                if (userName.StartsWith("@@"))
                {
                    Boolean in_list = false;
                    String g_id = userName;
                    foreach (JObject group in groupList)
                    {
                        //JsonObject group = ge.getAsJsonObject();
                        if (g_id == ((JValue)group["UserName"]).Value.ToString())
                        {
                            in_list = true;
                            group.Add("MemberCount", contact["MemberCount"]);
                            group.Add("NickName", contact["NickName"]);
                            this.groupMemeberList.Add(g_id, (JArray)contact["MemberList"]);
                            if (null != messageHandle)
                            {
                                messageHandle.groupMemberChange(g_id, (JArray)contact["MemberList"]);
                            }
                            break;
                        }
                    }
                    if (!in_list)
                    {
                        this.groupList.Add(contact);
                        this.groupMemeberList.Add(g_id, (JArray)contact["MemberList"]);
                        if (null != messageHandle)
                        {
                            messageHandle.groupListChange(g_id, (JArray)contact["MemberList"]);
                            messageHandle.groupMemberChange(g_id, (JArray)contact["MemberList"]);
                        }
                    }
                }
                else if (userName == "@")
                {
                    Boolean in_list = false;
                    foreach (JObject ue in memberList)
                    {
                        JObject u = ue;
                        String u_id = userName;
                        if (u_id == ((JValue)u["UserName"]).Value.ToString())
                        {
                            u = contact;
                            in_list = true;
                            break;
                        }
                    }
                    if (!in_list)
                    {
                        this.memberList.Add(contact);
                    }
                }
            }
        }

        private GroupMessage make_group_msg(UserMessage userMessage)
        {
            Console.WriteLine("make group message");
            GroupMessage groupMessage = new GroupMessage(this);
            groupMessage.setRawMsg(userMessage.getRawMsg());
            groupMessage.setMsgId(((JValue)userMessage.getRawMsg()["MsgId"]).Value.ToString());
            groupMessage.setFromUserName(((JValue)userMessage.getRawMsg()["FromUserName"]).Value.ToString());
            groupMessage.setToUserName(((JValue)userMessage.getRawMsg()["ToUserName"]).Value.ToString());
            groupMessage.setMsgType(((JValue)userMessage.getRawMsg()["MsgType"]).Value.ToString());
            groupMessage.setText(userMessage.getText());

            String content = ((JValue)userMessage.getRawMsg()["Content"]).Value.ToString().Replace("&lt;", "<").Replace("&gt;", ">");

            Dictionary<String, String> group = null, src = null;

            if (groupMessage.getFromUserName().StartsWith("@@"))
            {
                //接收到来自群的消息
                String g_id = groupMessage.getFromUserName();
                groupMessage.setGroupId(g_id);
                group = this.getGroupById(g_id);
                if (content.Contains(":<br/>"))
                {
                    String u_id = content.Split(new string[] { ":<br/>" },StringSplitOptions.None)[0];
                    src = getGroupUserById(u_id, g_id);
                }
            }
            else if (groupMessage.getToUserName().StartsWith("@@"))
            {
                // 自己发给群的消息
                String g_id = groupMessage.getToUserName();
                groupMessage.setGroupId(g_id);
                String u_id = groupMessage.getFromUserName();
                src = this.getGroupUserById(u_id, g_id);
                group = this.getGroupById(g_id);
            }

            if (null != src)
            {
                groupMessage.setUser_attrstatus(src["AttrStatus"]);
                groupMessage.setUser_display_name(src["DisplayName"]);
                groupMessage.setUser_nickname(src["NickName"]);
            }
            if (null != group)
            {
                groupMessage.setGroup_count(group["MemberCount"]);
                groupMessage.setGroup_owner_uin(group["OwnerUin"]);
                groupMessage.setGroup_name(group["ShowName"]);
            }
            groupMessage.setTimestamp(((JValue)userMessage.getRawMsg()["CreateTime"]).Value.ToString());

            return groupMessage;
        }
    }
}
