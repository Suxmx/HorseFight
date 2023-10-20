using Services;

namespace AI
{
    public class GameAI
    {
        public GameAI(Team aiTeam, EventSystem eventSystem)
        {
            this.aiTeam = aiTeam;
            this.eventSystem = eventSystem;
            eventSystem.AddListener<Team,int>(EEvent.OnNextRound,Buy);
        }

        public Team aiTeam;
        protected EventSystem eventSystem;

        protected void Buy(Team team,int round)
        {
            if (team != aiTeam) return;
        }
    }
}