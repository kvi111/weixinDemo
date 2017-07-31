using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace weixinDemo
{
    public class Session //: Serializable
    {

        private String uuid;
        private String skey;
        private String sid;
        private String uin;
        private String passTicket;

        public String getUuid()
        {
            return uuid;
        }

        public void setUuid(String uuid)
        {
            this.uuid = uuid;
        }

        public String getSkey()
        {
            return skey;
        }

        public void setSkey(String skey)
        {
            this.skey = skey;
        }

        public String getSid()
        {
            return sid;
        }

        public void setSid(String sid)
        {
            this.sid = sid;
        }

        public String getUin()
        {
            return uin;
        }

        public void setUin(String uin)
        {
            this.uin = uin;
        }

        public String getPassTicket()
        {
            return passTicket;
        }

        public void setPassTicket(String passTicket)
        {
            this.passTicket = passTicket;
        }


        public String toString()
        {
            return "Session(" +
                    "uuid='" + uuid + '\'' +
                    ", skey='" + skey + '\'' +
                    ", sid='" + sid + '\'' +
                    ", uin='" + uin + '\'' +
                    ", passTicket='" + passTicket + '\'' +
                    ')';
        }
    }
}
