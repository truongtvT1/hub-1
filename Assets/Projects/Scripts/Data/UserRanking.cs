using System;

namespace Projects.Scripts.Data
{
    [Serializable]
    public class UserRanking
    {
        public int rank, trophy, win, lose;
        public string userName;
    }
}