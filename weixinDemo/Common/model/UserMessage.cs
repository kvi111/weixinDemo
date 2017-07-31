using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weixinDemo
{
    public class UserMessage
    {
        private JObject rawMsg;
        private String location;
        private String msgId;
        private String msgType;
        private String log;
        private String text;
        private String fromUserName;
        private String toUserName;

        private WechatApi wechatApi;

        public UserMessage(WechatApi wechatApi)
        {
            this.wechatApi = wechatApi;
        }
        public JObject getRawMsg()
        {
            return rawMsg;
        }

        public void setRawMsg(JObject rawMsg)
        {
            this.rawMsg = rawMsg;
        }

        public String getLocation()
        {
            return location;
        }

        public void setLocation(String location)
        {
            this.location = location;
        }

        public virtual String getLog()
        {
            return log;
        }

        public virtual void setLog(String log)
        {
            this.log = log;
        }

        public String getText()
        {
            return text;
        }

        public void setText(String text)
        {
            this.text = text;
        }

        public WechatApi getWechatApi()
        {
            return wechatApi;
        }

        public void setWechatApi(WechatApi wechatApi)
        {
            this.wechatApi = wechatApi;
        }


        override
        public String ToString()
        {
            return "UserMessage(" +
                    "location='" + location + '\'' +
                    ", log='" + log + '\'' +
                    ", text='" + text + '\'' +
                    ')';
        }

        public String getMsgId()
        {
            return msgId;
        }

        public void setMsgId(String msgId)
        {
            this.msgId = msgId;
        }

        public String getFromUserName()
        {
            return fromUserName;
        }

        public void setFromUserName(String fromUserName)
        {
            this.fromUserName = fromUserName;
        }

        public String getToUserName()
        {
            return toUserName;
        }

        public void setToUserName(String toUserName)
        {
            this.toUserName = toUserName;
        }

        public String getMsgType()
        {
            return msgType;
        }

        public void setMsgType(String msgType)
        {
            this.msgType = msgType;
        }

        public Boolean isEmpty()
        {
            return null == text || null == rawMsg;
        }

        public void sendText(String msg, String uid)
        {
            wechatApi.sendText(msg, uid);
        }
    }
}
