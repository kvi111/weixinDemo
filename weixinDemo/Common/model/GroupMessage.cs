﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weixinDemo
{
    public class GroupMessage : UserMessage
    {
        private String groupId;
        private String group_owner_uin;
        private String group_name;
        private String group_count;
        private String user_attrstatus;
        private String user_display_name;
        private String user_nickname;
        private String recall_msg_id;
        private String sys_notif;
        private String time;
        private String timestamp;
        private String log;

        public GroupMessage(WechatApi wechatApi) : base(wechatApi)
        {
        }

        public String getGroupId()
        {
            return groupId;
        }

        public void setGroupId(String groupId)
        {
            this.groupId = groupId;
        }

        public String getGroup_owner_uin()
        {
            return group_owner_uin;
        }

        public void setGroup_owner_uin(String group_owner_uin)
        {
            this.group_owner_uin = group_owner_uin;
        }

        public String getGroup_name()
        {
            return group_name;
        }

        public void setGroup_name(String group_name)
        {
            this.group_name = group_name;
        }

        public String getGroup_count()
        {
            return group_count;
        }

        public void setGroup_count(String group_count)
        {
            this.group_count = group_count;
        }

        public String getUser_attrstatus()
        {
            return user_attrstatus;
        }

        public void setUser_attrstatus(String user_attrstatus)
        {
            this.user_attrstatus = user_attrstatus;
        }

        public String getUser_display_name()
        {
            return user_display_name;
        }

        public void setUser_display_name(String user_display_name)
        {
            this.user_display_name = user_display_name;
        }

        public String getUser_nickname()
        {
            return user_nickname;
        }

        public void setUser_nickname(String user_nickname)
        {
            this.user_nickname = user_nickname;
        }

        public String getRecall_msg_id()
        {
            return recall_msg_id;
        }

        public void setRecall_msg_id(String recall_msg_id)
        {
            this.recall_msg_id = recall_msg_id;
        }

        public String getSys_notif()
        {
            return sys_notif;
        }

        public void setSys_notif(String sys_notif)
        {
            this.sys_notif = sys_notif;
        }

        public String getTime()
        {
            return time;
        }

        public void setTime(String time)
        {
            this.time = time;
        }

        public String getTimestamp()
        {
            return timestamp;
        }

        public void setTimestamp(String timestamp)
        {
            this.timestamp = timestamp;
        }


        public override String getLog()
        {
            return log;
        }


        public override void setLog(String log)
        {
            this.log = log;
        }

        override
        public String ToString()
        {
            return "GroupMessage(" +
                    "groupId='" + groupId + '\'' +
                    ", group_owner_uin='" + group_owner_uin + '\'' +
                    ", group_name='" + group_name + '\'' +
                    ", group_count='" + group_count + '\'' +
                    ", user_attrstatus='" + user_attrstatus + '\'' +
                    ", user_display_name='" + user_display_name + '\'' +
                    ", user_nickname='" + user_nickname + '\'' +
                    ", recall_msg_id='" + recall_msg_id + '\'' +
                    ", sys_notif='" + sys_notif + '\'' +
                    ", time='" + time + '\'' +
                    ", timestamp='" + timestamp + '\'' +
                    ", log='" + log + '\'' +
                    ')';
        }
    }
}