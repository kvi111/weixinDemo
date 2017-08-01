using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace weixinDemo
{
    public class WechatApi
    {
        // 配置文件环境参数
        protected Environment environment;

        protected String appid = "wx782c26e4c19acffb";
        protected String wxHost;

        // 微信配置信息
        protected Dictionary<String, String> conf = new Dictionary<String, String>();

        protected String wxFileHost;
        protected String redirectUri;

        // 登录会话
        protected Session session;

        protected Dictionary<String, Object> baseRequest;

        protected JToken synckeyDic;
        protected String synckey;

        // device_id: 登录手机设备
        // web wechat 的格式为: e123456789012345 (e+15位随机数)
        // mobile wechat 的格式为: A1234567890abcde (A+15位随机数字或字母)
        protected String deviceId = "e" + Utils.currentTimeMillis();

        protected String userAgent = Const.API_USER_AGENT[new Random().Next(2)];
        protected String cookie;

        // 登陆账号信息
        protected Dictionary<String, Object> user;

        // 好友+群聊+公众号+特殊账号
        protected JArray memberList;

        protected int memberCount;

        /// <summary>
        /// 联系人列表
        /// </summary>
        public JArray contactList;

        // 群
        protected JArray groupList;

        // 群聊成员字典 {group_id:[]}
        protected Dictionary<String, JArray> groupMemeberList = new Dictionary<String, JArray>();

        // 公众号／服务号
        protected JArray publicUsersList;

        // 特殊账号
        protected JArray specialUsersList;

        /// <summary>
        /// 最近会话列表
        /// </summary>
        protected JArray sessionList;

        // 读取、连接、发送超时时长，单位/秒
        private int readTimeout, connTimeout, writeTimeout;

        public WechatApi(Environment environment)
        {
            this.wxHost = environment.get("wxHost", "wx.qq.com");
            this.connTimeout = environment.getInt("http.conn-time-out", 10);
            this.readTimeout = environment.getInt("http.read-time-out", 10);
            this.writeTimeout = environment.getInt("http.write-time-out", 10);
            this.conf_factory();
        }

        private void conf_factory()
        {
            conf.Clear();
            // wx.qq.com
            string e = this.wxHost;
            String t = "login.weixin.qq.com";
            String o = "file.wx.qq.com";
            String n = "webpush.weixin.qq.com";

            if (e.IndexOf("wx2.qq.com") > -1)
            {
                t = "login.wx2.qq.com";
                o = "file.wx2.qq.com";
                n = "webpush.wx2.qq.com";
            }
            else if (e.IndexOf("wx8.qq.com") > -1)
            {
                t = "login.wx8.qq.com";
                o = "file.wx8.qq.com";
                n = "webpush.wx8.qq.com";
            }
            else if (e.IndexOf("qq.com") > -1)
            {
                t = "login.wx.qq.com";
                o = "file.wx.qq.com";
                n = "webpush.wx.qq.com";
            }
            else if (e.IndexOf("web2.wechat.com") > -1)
            {
                t = "login.web2.wechat.com";
                o = "file.web2.wechat.com";
                n = "webpush.web2.wechat.com";
            }
            else if (e.IndexOf("wechat.com") > -1)
            {
                t = "login.web.wechat.com";
                o = "file.web.wechat.com";
                n = "webpush.web.wechat.com";
            }
            conf.Add("LANG", "zh_CN");
            conf.Add("API_jsLogin", "https://login.weixin.qq.com/jslogin");
            conf.Add("API_qrcode", "https://login.weixin.qq.com/l/");
            conf.Add("API_qrcode_img", "https://login.weixin.qq.com/qrcode/");

            conf.Add("API_login", "https://" + e + "/cgi-bin/mmwebwx-bin/login");
            conf.Add("API_synccheck", "https://" + n + "/cgi-bin/mmwebwx-bin/synccheck");
            conf.Add("API_webwxdownloadmedia", "https://" + o + "/cgi-bin/mmwebwx-bin/webwxgetmedia");
            conf.Add("API_webwxuploadmedia", "https://" + o + "/cgi-bin/mmwebwx-bin/webwxuploadmedia");
            conf.Add("API_webwxpreview", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxpreview");
            conf.Add("API_webwxinit", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxinit");
            conf.Add("API_webwxgetcontact", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxgetcontact");
            conf.Add("API_webwxsync", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxsync");
            conf.Add("API_webwxbatchgetcontact", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxbatchgetcontact");
            conf.Add("API_webwxgeticon", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxgeticon");
            conf.Add("API_webwxsendmsg", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxsendmsg");
            conf.Add("API_webwxsendmsgimg", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxsendmsgimg");
            conf.Add("API_webwxsendmsgvedio", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxsendvideomsg");
            conf.Add("API_webwxsendemoticon", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxsendemoticon");
            conf.Add("API_webwxsendappmsg", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxsendappmsg");
            conf.Add("API_webwxgetheadimg", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxgetheadimg");
            conf.Add("API_webwxgetmsgimg", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxgetmsgimg");
            conf.Add("API_webwxgetmedia", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxgetmedia");
            conf.Add("API_webwxgetvideo", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxgetvideo");
            conf.Add("API_webwxlogout", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxlogout");
            conf.Add("API_webwxgetvoice", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxgetvoice");
            conf.Add("API_webwxupdatechatroom", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxupdatechatroom");
            conf.Add("API_webwxcreatechatroom", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxcreatechatroom");
            conf.Add("API_webwxstatusnotify", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxstatusnotify");
            conf.Add("API_webwxcheckurl", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxcheckurl");
            conf.Add("API_webwxverifyuser", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxverifyuser");
            conf.Add("API_webwxfeedback", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxsendfeedback");
            conf.Add("API_webwxreport", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxstatreport");
            conf.Add("API_webwxsearch", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxsearchcontact");
            conf.Add("API_webwxoplog", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxoplog");
            conf.Add("API_checkupload", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxcheckupload");
            conf.Add("API_webwxrevokemsg", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxrevokemsg");
            conf.Add("API_webwxpushloginurl", "https://" + e + "/cgi-bin/mmwebwx-bin/webwxpushloginurl");

            conf.Add("CONTACTFLAG_CONTACT", "1");
            conf.Add("CONTACTFLAG_CHATCONTACT", "2");
            conf.Add("CONTACTFLAG_CHATROOMCONTACT", "4");
            conf.Add("CONTACTFLAG_BLACKLISTCONTACT", "8");
            conf.Add("CONTACTFLAG_DOMAINCONTACT", "16");
            conf.Add("CONTACTFLAG_HIDECONTACT", "32");
            conf.Add("CONTACTFLAG_FAVOURCONTACT", "64");
            conf.Add("CONTACTFLAG_3RDAPPCONTACT", "128");
            conf.Add("CONTACTFLAG_SNSBLACKLISTCONTACT", "256");
            conf.Add("CONTACTFLAG_NOTIFYCLOSECONTACT", "512");
            conf.Add("CONTACTFLAG_TOPCONTACT", "2048");
            conf.Add("MSGTYPE_TEXT", "1");
            conf.Add("MSGTYPE_IMAGE", "3");
            conf.Add("MSGTYPE_VOICE", "34");
            conf.Add("MSGTYPE_VIDEO", "43");
            conf.Add("MSGTYPE_MICROVIDEO", "62");
            conf.Add("MSGTYPE_EMOTICON", "47");
            conf.Add("MSGTYPE_APP", "49");
            conf.Add("MSGTYPE_VOIPMSG", "50");
            conf.Add("MSGTYPE_VOIPNOTIFY", "52");
            conf.Add("MSGTYPE_VOIPINVITE", "53");
            conf.Add("MSGTYPE_LOCATION", "48");
            conf.Add("MSGTYPE_STATUSNOTIFY", "51");
            conf.Add("MSGTYPE_SYSNOTICE", "9999");
            conf.Add("MSGTYPE_POSSIBLEFRIEND_MSG", "40");
            conf.Add("MSGTYPE_VERIFYMSG", "37");
            conf.Add("MSGTYPE_SHARECARD", "42");
            conf.Add("MSGTYPE_SYS", "10000");
            conf.Add("MSGTYPE_RECALLED", "10002");
            conf.Add("APPMSGTYPE_TEXT", "1");
            conf.Add("APPMSGTYPE_IMG", "2");
            conf.Add("APPMSGTYPE_AUDIO", "3");
            conf.Add("APPMSGTYPE_VIDEO", "4");
            conf.Add("APPMSGTYPE_URL", "5");
            conf.Add("APPMSGTYPE_ATTACH", "6");
            conf.Add("APPMSGTYPE_OPEN", "7");
            conf.Add("APPMSGTYPE_EMOJI", "8");
            conf.Add("APPMSGTYPE_VOICE_REMIND", "9");
            conf.Add("APPMSGTYPE_SCAN_GOOD", "10");
            conf.Add("APPMSGTYPE_GOOD", "13");
            conf.Add("APPMSGTYPE_EMOTION", "15");
            conf.Add("APPMSGTYPE_CARD_TICKET", "16");
            conf.Add("APPMSGTYPE_REALTIME_SHARE_LOCATION", "17");
            conf.Add("APPMSGTYPE_TRANSFERS", "2e3");
            conf.Add("APPMSGTYPE_RED_ENVELOPES", "2001");
            conf.Add("APPMSGTYPE_READER_TYPE", "100001");
            conf.Add("UPLOAD_MEDIA_TYPE_IMAGE", "1");
            conf.Add("UPLOAD_MEDIA_TYPE_VIDEO", "2");
            conf.Add("UPLOAD_MEDIA_TYPE_AUDIO", "3");
            conf.Add("UPLOAD_MEDIA_TYPE_ATTACHMENT", "4");
        }

        /// <summary>
        /// 获取uuid
        /// </summary>
        /// <returns></returns>
        public void getUUID()
        {
            while (true)
            {
                try
                {
                    WriteLog(Const.LOG_MSG_GET_UUID);

                    String url = conf["API_jsLogin"];
                    Dictionary<String, Object> params1 = new Dictionary<String, Object>();
                    params1.Add("appid", appid);
                    params1.Add("fun", "new");
                    params1.Add("lang", conf["LANG"]);
                    params1.Add("_", Utils.currentTimeMillis() + "");

                    String response = doGet(url, new Dictionary<String, Object>[] { params1 });
                    if (string.IsNullOrEmpty(response))
                    {
                        WriteLog("获取UUID失败");
                        continue;
                        //return false;
                    }

                    String code = Utils.match(response, "window.QRLogin.code = (\\d+);");

                    if (string.IsNullOrEmpty(code))
                    {
                        WriteLog("获取UUID失败");
                        continue;
                        //return false;
                    }

                    if (code != "200")
                    {
                        WriteLog("错误的状态码: {0}", code);
                        continue;
                        //return false;
                    }
                    session = new Session();
                    session.setUuid(Utils.match(response, "window.QRLogin.uuid = \"(.*)\";"));
                    //return true;
                    WriteLog(Const.LOG_MSG_SUCCESS);
                    break;
                }
                catch (Exception ex)
                {
                    WriteLog("getUUID异常：{0}", ex.Message);
                }
            }
        }

        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <returns></returns>
        public void genqrcode()
        {
            while (true)
            {
                try
                {
                    WriteLog(Const.LOG_MSG_GET_QRCODE);

                    String url = conf["API_qrcode_img"] + session.getUuid();
                    string formBody = "t=webwx&_=" + Utils.currentTimeMillis();

                    //byte[] postData = Encoding.UTF8.GetBytes(formBody);

                    byte[] byteData = Utils.PostWebRequestBytes(url, formBody);

                    if (byteData.Length > 1000)
                    {
                        FormLogin.instance.SetQRImage(byteData);
                        WriteLog(Const.LOG_MSG_SUCCESS);
                        break;
                    }
                    else
                    {
                        WriteLog(Const.LOG_MSG_FAIL);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    WriteLog("[*] 生成二维码异常：{0}", e.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// 等待登录
        /// </summary>
        /// <param name="tip">1:等待扫描二维码 0:等待微信客户端确认</param>
        /// <returns></returns>
        public bool waitforlogin(int tip)
        {
            Thread.Sleep(2000);
            String url = conf["API_login"] + "?tip={0}&loginicon=true&uuid={1}&_{2}";
            url = String.Format(url, tip, session.getUuid(), Utils.currentTimeMillis());

            String response = doGet(url, new Dictionary<string, object>[] { });

            if (Utils.isBlank(response))
            {
                WriteLog("扫描二维码验证失败");
                return false;
            }

            String code = Utils.match(response, "window.code=(\\d+);");


            if (Utils.isBlank(code))
            {
                WriteLog("扫描二维码验证失败");
                return false;
            }

            if (code == "201") //扫码后进入
            {
                String base64HeadImg = Utils.match(response, "window.userAvatar = 'data:img/jpg;base64,(\\S+?)';"); //头像
                byte[] headImg = Convert.FromBase64String(base64HeadImg);
                FormLogin.instance.SetQRImage(headImg);
                return true;
            }

            if (code == "200") //点确定后进入
            {
                String pm = Utils.match(response, "window.redirect_uri=\"(\\S+?)\";");
                String r_uri = pm + "&fun=new";
                this.redirectUri = r_uri;
                this.wxHost = r_uri.Split(new string[] { "://" }, StringSplitOptions.RemoveEmptyEntries)[1].Split('/')[0];

                this.conf_factory();
                return true;
            }
            if (code == "408")
            {
                WriteLog(Const.LOG_MSG_WAIT_LOGIN_ERR1);
            }
            else
            {
                WriteLog(Const.LOG_MSG_WAIT_LOGIN_ERR2);
            }
            return false;
        }

        /// <summary>
        /// 登录微信
        /// </summary>
        /// <returns></returns>
        public void login()
        {
            while (true)
            {
                try
                {
                    WriteLog(Const.LOG_MSG_LOGIN);

                    HttpWebRequest webClient = (HttpWebRequest)HttpWebRequest.Create(this.redirectUri);

                    if (null != cookie)
                    {
                        webClient.Headers.Add("Cookie", this.cookie);
                        //requestBuilder.addHeader("Cookie", this.cookie);
                    }

                    String body = "";
                    HttpWebResponse webR = (HttpWebResponse)webClient.GetResponse();

                    List<String> cookies = webR.Headers.GetValues("Set-Cookie").ToList();// headers.values("Set-Cookie");
                    this.cookie = Utils.getCookie(cookies);  //webR.Headers["Set-Cookie"].ToString();//

                    WriteLog("[*] 设置cookie [{0}]", this.cookie);

                    using (StreamReader sr = new StreamReader(webR.GetResponseStream()))
                    {
                        body = sr.ReadToEnd();
                    }

                    if (Utils.isBlank(body))
                    {
                        WriteLog(Const.LOG_MSG_FAIL);
                        continue;
                        //return false;
                    }
                    session.setSkey(Utils.match(body, "<skey>(\\S+)</skey>"));
                    session.setSid(Utils.match(body, "<wxsid>(\\S+)</wxsid>"));
                    session.setUin(Utils.match(body, "<wxuin>(\\S+)</wxuin>"));
                    session.setPassTicket(Utils.match(body, "<pass_ticket>(\\S+)</pass_ticket>"));

                    this.baseRequest = Utils.createMap(new object[]{"Uin", long.Parse(session.getUin()),
                        "Sid", session.getSid(), "Skey", session.getSkey(), "DeviceID", this.deviceId });

                    WriteLog(Const.LOG_MSG_SUCCESS);
                    break;
                    //return true;
                }
                catch (Exception e)
                {
                    WriteLog("[*] login 异常：{0}", e.Message);
                    continue;
                    //return false;
                }
            }
        }

        private String doGet(String url, Dictionary<String, Object>[] param)
        {
            return doGet(url, null, param);
        }

        private String doGet(String url, String cookie, Dictionary<String, Object>[] params1)
        {
            if (null != params1 && params1.Length > 0)
            {
                Dictionary<String, Object> param = params1[0];
                List<String> keys = param.Keys.ToList();
                StringBuilder sbuf = new StringBuilder(url);
                if (url.Contains("="))
                {
                    sbuf.Append("&");
                }
                else
                {
                    sbuf.Append("?");
                }
                foreach (String key in keys)
                {
                    sbuf.Append(key).Append('=').Append(param[key]).Append('&');
                }
                url = sbuf.ToString().Substring(0, sbuf.Length - 1);
            }
            try
            {
                HttpWebRequest webClient = (HttpWebRequest)HttpWebRequest.Create(url);

                if (null != cookie)
                {
                    webClient.Headers.Add("Cookie", this.cookie);
                }

                String body = "";
                using (StreamReader sr = new StreamReader(webClient.GetResponse().GetResponseStream()))
                {
                    body = sr.ReadToEnd();
                }

                //WriteLog("[*] 响应 => {0}", body);
                return body;
            }
            catch (Exception e)
            {
                WriteLog("doGet异常：{0}", e.StackTrace);
                return null;
            }
        }

        protected JObject doPost(String url, Object obj)
        {
            String bodyJson = "";

            HttpWebRequest webClient = (HttpWebRequest)HttpWebRequest.Create(url);
            //webClient.ContentType = "application/json";
            webClient.Method = "POST";
            if (null != cookie)
            {
                webClient.Headers.Add("Cookie", cookie);
                //requestBuilder.addHeader("Cookie", cookie);
            }

            if (null != obj)
            {
                bodyJson = Utils.toJson(obj);
                Stream requestStream = webClient.GetRequestStream();
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(bodyJson);
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            try
            {
                //Response response = client.newCall(request).execute();
                String body = "";
                using (StreamReader sr = new StreamReader(webClient.GetResponse().GetResponseStream()))
                {
                    body = sr.ReadToEnd();
                }
                if (null != body && body.Length <= 300)
                {
                    //WriteLog("[*] 响应 => {0}", body);
                }

                return JObject.Parse(body);  //JsonObject
            }
            catch (Exception e)
            {
                WriteError("doPost异常：{0}_{1}", e.Message, e.StackTrace);
                return null; // JObject.Parse("{}");
            }
        }

        /// <summary>
        /// 微信初始化，获取当前用户信息，以及最近会话列表(包括群，联系人等，会和后面的获取联系人结果有重复)
        /// </summary>
        /// <returns></returns>
        public void webwxinit()
        {
            while (true)
            {
                try
                {
                    WriteLog(Const.LOG_MSG_INIT);

                    String url = conf["API_webwxinit"] + "?pass_ticket={0}&r={1}";  //&skey={1}
                    url = String.Format(url, session.getPassTicket(), Utils.currentTimeMillis()); //session.getSkey(),

                    Dictionary<String, Object> param = Utils.createMap(new Object[] { "BaseRequest", this.baseRequest });

                    JObject responseJson = doPost(url, param);
                    if (null == responseJson)
                    {
                        continue;
                    }
                    this.sessionList = (JArray)responseJson["ContactList"];//最近会话列表

                    this.user = JsonToDictionary(responseJson["User"]);
                    this.makeSynckey(responseJson);

                    if (((JValue)responseJson["BaseResponse"]["Ret"]).Value.ToString() == "0")
                    {
                        WriteLog(Const.LOG_MSG_SUCCESS);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    WriteLog("webwxinit异常：{0}", ex.Message);
                    continue;
                }
            }
        }
        /// <summary>
        /// 得到Synckey
        /// </summary>
        /// <param name="dic"></param>
        protected void makeSynckey(JObject dic)
        {
            this.synckeyDic = dic["SyncKey"];
            StringBuilder synckey = new StringBuilder();
            JToken list = this.synckeyDic["List"];
            foreach (JContainer element in list)
            {
                synckey.Append("|" + ((JValue)element["Key"]).Value + "_" + ((JValue)element["Val"]).Value);
            }
            this.synckey = synckey.ToString().Substring(1);
        }

        /// <summary>
        /// 开启微信状态通知
        /// </summary>
        /// <returns></returns>
        public void openStatusNotify()
        {
            while (true)
            {
                try
                {
                    WriteLog(Const.LOG_MSG_STATUS_NOTIFY);

                    String url = conf["API_webwxstatusnotify"] + "?lang={0}&pass_ticket={1}";
                    url = String.Format(url, conf["LANG"], session.getPassTicket());

                    Dictionary<String, Object> params1 = new Dictionary<String, Object>();
                    params1.Add("BaseRequest", this.baseRequest);
                    params1.Add("Code", 3);
                    params1.Add("FromUserName", this.user["UserName"]);
                    params1.Add("ToUserName", this.user["UserName"]);
                    params1.Add("ClientMsgId", Utils.currentTimeMillis());

                    JObject response = doPost(url, params1);
                    if (null == response)
                    {
                        continue;
                        //return false;
                    }
                    if (((JValue)response["BaseResponse"]["Ret"]).Value.ToString() == "0")
                    {
                        WriteLog(Const.LOG_MSG_SUCCESS);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    WriteError("webwxinit异常：{0}", ex.Message);
                    continue;
                }
            }
        }
        /// <summary>
        /// 获取联系人
        /// "Uin": 0,
        ///"UserName": 用户名称，一个"@"为好友，两个"@"为群组
        ///"NickName": 昵称
        ///"HeadImgUrl":头像图片链接地址
        ///"ContactFlag": 1-好友， 2-群组， 3-公众号
        ///"MemberCount": 成员数量，只有在群组信息中才有效,
        ///"MemberList": 成员列表,
        ///"RemarkName": 备注名称
        ///"HideInputBarFlag": 0,
        ///"Sex": 性别，0-未设置（公众号、保密），1-男，2-女
        ///"Signature": 公众号的功能介绍 or 好友的个性签名
        ///"VerifyFlag": 0,
        ///"OwnerUin": 0,
        ///"PYInitial": 用户名拼音缩写
        ///"PYQuanPin": 用户名拼音全拼
        ///"RemarkPYInitial":备注拼音缩写
        ///"RemarkPYQuanPin": 备注拼音全拼
        ///"StarFriend": 是否为星标朋友  0-否  1-是
        ///"AppAccountFlag": 0,
        ///"Statues": 0,
        ///"AttrStatus": 119911,
        ///"Province": 省
        ///"City": 市
        ///"Alias": 
        ///"SnsFlag": 17,
        ///"UniFriend": 0,
        ///"DisplayName": "",
        ///"ChatRoomId": 0,
        ///"KeyWord": 
        ///"EncryChatRoomId": ""
        /// </summary>
        /// <returns></returns>
        public void getContact()
        {
            while (true)
            {
                try
                {
                    WriteLog(Const.LOG_MSG_GET_CONTACT);
                    String url = conf["API_webwxgetcontact"] + "?pass_ticket={0}&skey={1}&r={2}";
                    url = String.Format(url, session.getPassTicket(), session.getSkey(), Utils.currentTimeMillis());

                    HashSet<String> specialUsers = Const.API_SPECIAL_USER;

                    JObject response = doPost(url, null);

                    if (null == response)
                    {
                        WriteError("getContact  response为空！");
                        continue;
                    }

                    this.memberCount = int.Parse(((JValue)response["MemberCount"]).Value.ToString());
                    this.memberList = (JArray)response["MemberList"];
                    memberList.Add(sessionList.ToArray());

                    JArray ContactList = new JArray(memberList.ToArray());
                    //ContactList.Add(sessionList.ToArray());
                    //ContactList.Add((JArray)sessionList);

                    this.publicUsersList = new JArray();
                    this.groupList = new JArray();
                    this.specialUsersList = new JArray();

                    foreach (JObject contact in memberList)
                    {
                        string elementVerifyFlag = ((JValue)contact["VerifyFlag"]).Value.ToString();
                        string elementUserName = ((JValue)contact["UserName"]).Value.ToString();
                        if (elementVerifyFlag != "0") //公众号/服务号
                        {
                            //ContactList.Remove(contact);
                            this.publicUsersList.Add(contact);
                        }
                        else if (specialUsers.Contains(elementUserName)) //特殊账号
                        {
                            //ContactList.Remove(contact);
                            this.specialUsersList.Add(contact);
                        }
                        else if (elementUserName.Contains("@@")) // 微信群
                        {
                            //ContactList.Remove(contact);
                            this.groupList.Add(contact);
                        }
                        else if (elementUserName == this.user["UserName"].ToString()) //自己
                        {
                            ContactList.Remove(contact);
                        }
                    }
                    this.contactList = ContactList;

                    WriteLog(Const.LOG_MSG_CONTACT_COUNT, memberCount, memberList.Count);
                    WriteLog(Const.LOG_MSG_OTHER_CONTACT_COUNT, groupList.Count, contactList.Count, specialUsersList.Count, publicUsersList.Count);
                    break;
                }
                catch (Exception ex)
                {
                    WriteError("getContact异常：{0}", ex.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// 获取聊天会话列表
        /// 第一次请求得到最近一段时间内活跃的群组（但不知道腾讯是怎么定义这段时间的）
        /// 第二次请求得到剩下的（最近没有交流的）群组： 
        /// </summary>
        public void getGroup()
        {
            int i = 1;
            while (i <= 2)
            {
                try
                {
                    WriteLog("获取聊天会话列表，第{0}次", i);
                    String url = conf["API_webwxbatchgetcontact"] + "?type=ex&r={0}&pass_ticket={1}";
                    url = String.Format(url, Utils.currentTimeMillis(), session.getPassTicket());

                    JObject response = doPost(url, null);
                    if (null == response)
                    {
                        WriteError("getGroup  response为空！");
                        continue;
                    }

                    this.memberCount = int.Parse(((JValue)response["Count"]).Value.ToString());
                    if (memberCount == 0)
                    {
                        continue;
                    }
                    //this.memberList = (JArray)response["MemberList"];

                    //JArray ContactList = new JArray(memberList);

                    //foreach (JObject contact in memberList)
                    //{

                    //}
                    //this.contactList = ContactList;

                    WriteLog(Const.LOG_MSG_CONTACT_COUNT, memberCount, memberList.Count);
                    WriteLog(Const.LOG_MSG_OTHER_CONTACT_COUNT, groupList.Count, contactList.Count, specialUsersList.Count, publicUsersList.Count);
                    i++;
                }
                catch (Exception ex)
                {
                    WriteError("getGroup异常：{0}", ex.Message);
                    continue;
                }
                Thread.Sleep(3000);
            }
        }
        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        public Boolean snapshot()
        {
            WriteLog(Const.LOG_MSG_SNAPSHOT);
            return false;
        }

        /// <summary>
        /// 微信同步检查
        /// retcode: 0 正常，1100 失败/退出微信
        /// selector:0 正常，2 新的消息，7 进入/离开聊天界面
        /// </summary>
        /// <returns></returns>
        public int[] synccheck()
        {
            String url = conf["API_synccheck"];
            Dictionary<String, Object> params1 = new Dictionary<String, Object>();
            params1.Add("r", Utils.currentTimeMillis() + Utils.getRandomNumber(5));
            params1.Add("sid", session.getSid());
            params1.Add("uin", session.getUin());
            params1.Add("skey", session.getSkey());
            params1.Add("deviceid", this.deviceId);
            params1.Add("synckey", this.synckey);
            params1.Add("_", Utils.currentTimeMillis());

            String response = doGet(url, this.cookie, new Dictionary<string, object>[] { params1 });

            int[] arr = new int[] { -1, -1 };
            if (Utils.isBlank(response))
            {
                return arr;
            }
            String retcode = Utils.match(response, "retcode:\"(\\d+)\",");
            String selector = Utils.match(response, "selector:\"(\\d+)\"}");
            if (null != retcode && null != selector)
            {
                arr[0] = int.Parse(retcode);
                arr[1] = int.Parse(selector);
            }
            return arr;
        }

        /// <summary>
        /// 获取联系人头像
        ///     seq: 数字, 可为空
        ///     username: ID
        ///     skey: xxx
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public JObject GetContactHeadImg(String userName)
        {
            String url = conf["API_webwxgeticon"] + "?seq=637275253&username={0}&skey={1}";  //seq=637275253&
            url = String.Format(url, userName, session.getSkey());

            String clientMsgId = Utils.currentTimeMillis() + Utils.getRandomNumber(5);

            JObject response = doPost(url, null);
            if (null == response)
            {
                return null;
            }
            return response;
        }

        public void sendText(String msg, String uid)
        {
            this.wxSendMessage(msg, uid);
        }

        public JObject wxSendMessage(String msg, String to)
        {
            String url = conf["API_webwxsendmsg"] + "?pass_ticket={0}";
            url = String.Format(url, session.getPassTicket());

            String clientMsgId = Utils.currentTimeMillis() + Utils.getRandomNumber(5);
            Dictionary<String, Object> params1 = new Dictionary<String, Object>();
            params1.Add("BaseRequest", this.baseRequest);
            Dictionary<String, Object> Msg = new Dictionary<String, Object>();
            Msg.Add("Type", 1);
            Msg.Add("Content", Utils.unicodeToUtf8(msg));
            Msg.Add("FromUserName", this.user["UserName"]);
            Msg.Add("ToUserName", to);
            Msg.Add("LocalID", clientMsgId);
            Msg.Add("ClientMsgId", clientMsgId);
            params1.Add("Msg", Msg);

            JObject response = doPost(url, params1);
            if (null == response)
            {
                return null;
            }
            return response;
        }
        /**
         * 根据群id查询群信息
         *
         * @param groupId
         * @return
         */
        public Dictionary<String, String> getGroupById(String groupId)
        {
            String unknownGroup = Const.LOG_MSG_UNKNOWN_GROUP_NAME + groupId;
            Dictionary<String, String> group = new Dictionary<String, String>();
            group.Add("UserName", groupId);
            group.Add("NickName", "");
            group.Add("DisplayName", "");
            group.Add("ShowName", "");
            group.Add("OwnerUin", "");
            group.Add("MemberCount", "");

            foreach (JObject element in groupList)
            {
                //JsonObject member = element.getAsJsonObject();
                if (((JValue)element["UserName"]).Value.ToString() == groupId)
                {
                    group["NickName"] = ((JValue)element["NickName"]) == null ? "" : ((JValue)element["NickName"]).Value.ToString();// Utils.emptyOr(((JValue)element["NickName"]).Value.ToString(), "");
                    group["DisplayName"] = ((JValue)element["DisplayName"]) == null ? "" : ((JValue)element["DisplayName"]).Value.ToString(); //Utils.emptyOr(((JValue)element["DisplayName"]).Value.ToString(), "");
                    group["ShowName"] = ((JValue)element["ShowName"]) == null ? "" : ((JValue)element["ShowName"]).Value.ToString();//Utils.emptyOr(((JValue)element["ShowName"]).Value.ToString(), "");
                    group["OwnerUin"] = ((JValue)element["OwnerUin"]) == null ? "" : ((JValue)element["OwnerUin"]).Value.ToString(); //Utils.emptyOr(((JValue)element["OwnerUin"]).Value.ToString(), "");
                    group["MemberCount"] = ((JValue)element["MemberCount"]) == null ? "0" : ((JValue)element["MemberCount"]).Value.ToString();//Utils.emptyOr(((JValue)element["MemberCount"]).Value.ToString(), "0");
                    break;
                }
            }

            return group;
        }
        /**
         * 根据用户id和群id查询用户信息
         *
         * @param userId
         * @param groupId
         * @return
         */
        public Dictionary<String, String> getGroupUserById(String userId, String groupId)
        {
            String unknownPeople = Const.LOG_MSG_UNKNOWN_NAME + userId;
            Dictionary<String, String> user = new Dictionary<String, String>();
            // 微信动态ID
            user.Add("UserName", userId);
            // 微信昵称
            user.Add("NickName", "");
            // 群聊显示名称
            user.Add("DisplayName", "");
            // Log显示用的
            user.Add("ShowName", unknownPeople);
            // 群用户id
            user.Add("AttrStatus", unknownPeople);

            // 群友
            if (!groupMemeberList.ContainsKey(groupId))
            {
                return user;
            }

            JArray memebers = groupMemeberList[groupId];
            foreach (JObject element in memebers)
            {
                //JsonObject member = element.getAsJsonObject();
                if (((JValue)element["UserName"]).Value.ToString() == userId)
                {
                    String nickName = ((JValue)element["NickName"]) == null ? "" : ((JValue)element["NickName"]).Value.ToString(); //Utils.emptyOr(((JValue)element["NickName"]).Value.ToString(), "");
                    String displayName = ((JValue)element["DisplayName"]) == null ? "" : ((JValue)element["DisplayName"]).Value.ToString(); //Utils.emptyOr(((JValue)element["DisplayName"]).Value.ToString(), "");
                    user["NickName"] = nickName;
                    user["DisplayName"] = displayName;
                    user["AttrStatus"] = ((JValue)element["AttrStatus"]) == null ? "" : ((JValue)element["AttrStatus"]).Value.ToString(); //Utils.emptyOr(((JValue)element["AttrStatus"]).Value.ToString(), "");
                    user["ShowName"] = displayName ?? nickName;
                    break;
                }
            }
            return user;
        }
        /**
         * 根据用户id查询用户信息
         *
         * @param userId
         * @return
         */
        public Dictionary<String, String> getUserById(String userId)
        {
            String unknownPeople = Const.LOG_MSG_UNKNOWN_NAME + userId;
            Dictionary<String, String> user = new Dictionary<String, String>();
            user.Add("UserName", userId);
            user.Add("RemarkName", "");
            user.Add("NickName", "");
            user.Add("ShowName", unknownPeople);

            if (userId == this.user["UserName"].ToString())
            {
                user["RemarkName"] = this.user["RemarkName"].ToString();
                user["NickName"] = this.user["NickName"].ToString();
                user["ShowName"] = this.user["NickName"].ToString();
            }
            else
            {
                // 联系人
                foreach (JObject element in memberList)
                {
                    //JsonObject item = element.getAsJsonObject();
                    if (((JValue)element["UserName"]).Value.ToString() == userId)
                    {
                        user["RemarkName"] = ((JValue)element["RemarkName"]) == null ? "" : ((JValue)element["RemarkName"]).Value.ToString();// Utils.emptyOr(((JValue)element["RemarkName"]).Value.ToString(), "");
                        user["NickName"] = ((JValue)element["NickName"]) == null ? "" : ((JValue)element["NickName"]).Value.ToString();//Utils.emptyOr(((JValue)element["NickName"]).Value.ToString(), "");
                        if (string.IsNullOrEmpty(user["RemarkName"]) == false)
                        {
                            user["ShowName"] = user["RemarkName"];
                        }
                        else
                        {
                            user["ShowName"] = user["NickName"];
                        }
                        break;
                    }
                }
                // 特殊账号
                foreach (JObject element in specialUsersList)
                {
                    //JsonObject item = element.getAsJsonObject();
                    if (((JValue)element["UserName"]).Value.ToString() == userId)
                    {
                        user["RemarkName"] = userId;
                        user["NickName"] = userId;
                        user["ShowName"] = userId;
                        break;
                    }
                }
            }
            return user;
        }
        private Dictionary<string, object> JsonToDictionary(JToken jt)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (JProperty j in jt)
            {
                dict.Add(j.Name, j.Value);
            }
            return dict;
        }

        public void WriteLog(string formatString, params object[] parms)
        {
            Console.ForegroundColor = ConsoleColor.White;
            string msg = String.Format(formatString, parms);
            Console.WriteLine(DateTime.Now.ToString(" hh:mm:ss ") + msg);
        }
        public void WriteError(string formatString, params object[] parms)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            string msg = String.Format(formatString, parms);
            Console.WriteLine(DateTime.Now.ToString(" hh:mm:ss ") + msg);
        }
    }
}
