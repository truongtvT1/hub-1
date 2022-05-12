namespace MiniGame
{
    public interface IPlayerRanking
    {
        public void InitRank(RankIngame rankInfo);
        public void Score(int score);
        public void Finish();
    }
}