using System;

namespace Projects.Scripts.Data
{
    [Serializable]
    public class UserRanking
    {
        public UserRanking()
        {
            var rd = new Random();
            id = rd.Next(100000, 999999);
            userName = "Player" + id;
        }
        
        public int id, rank, trophy, win, lose;
        public string userName;
    }
}