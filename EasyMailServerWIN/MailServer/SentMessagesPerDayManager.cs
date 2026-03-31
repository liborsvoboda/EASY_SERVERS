using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Authenticated user sent messages per ay counter.
    /// </summary>
    public class SentMessagesPerDayManager
    {
        private object                 m_pLock = new object();
        private Dictionary<string,int> m_pList = null;
        private DateTime               m_Time;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SentMessagesPerDayManager()
        {
            m_pList = new Dictionary<string,int>();
            m_Time  = DateTime.Today;
        }


        #region method GetUserSentMessagesCount

        /// <summary>
        /// Gets how many messages user has sent today.
        /// </summary>
        /// <param name="userName">User login name.</param>
        /// <returns>Returns how many messages user has sent today.</returns>
        public int GetUserSentMessagesCount(string userName)
        {
            if(string.IsNullOrEmpty(userName)){
                throw new ArgumentNullException("userName");
            }

            lock(m_pLock){
                // New date arrived, reset list.
                if((DateTime.Now - m_Time).TotalHours > 24){
                    m_Time = DateTime.Today;
                    m_pList.Clear();
                }

                if(m_pList.ContainsKey(userName)){
                    return m_pList[userName];
                }
                else{
                    return 0;
                }
            }
        }

        #endregion

        #region method IncreaseUserSentMessagesCout

        /// <summary>
        /// Increases the specified user messages sent today value by 1.
        /// </summary>
        /// <param name="userName">User login name.</param>
        public void IncreaseUserSentMessagesCout(string userName)
        {
            if(string.IsNullOrEmpty(userName)){
                throw new ArgumentNullException("userName");
            }

            lock(m_pLock){
                if(m_pList.ContainsKey(userName)){
                    m_pList[userName]++;
                }
                else{
                    m_pList[userName] = 1;
                }
            }
        }

        #endregion
    }
}
