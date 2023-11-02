using AI;
using Services;
using UnityEngine;

namespace Cores
{
    public class PlayModeConfig : Service
    {
        private bool hasInited = false;
        public AIMode randomModeDifficulty = AIMode.Easy;

        protected override void Awake()
        {
            singleInAllScene = true;
            if (!hasInited)
            {
                hasInited = true;
                base.Awake();
            }
        }

        protected override void Start()
        {
            base.Start();
            DontDestroyOnLoad(this);
        }

        public bool IfAI { private set; get; }

        public AI.AIMode AIMode { private set; get; }

        public bool IfRandom { private set; get; }

        public void SetPlayModeConfig(bool ifAI, AIMode aiMode = AIMode.Easy, bool ifRandom = false)
        {
            IfRandom = ifRandom;
            IfAI = ifAI;
            AIMode = aiMode;
        }

        public void SetWinEvent(EventSystem eventSystem)
        {
            eventSystem.AddListener<Team>(EEvent.OnGameEnd,OnGameEnd);
        }

        public void OnGameEnd(Team winTeam)
        {
            if (winTeam == Team.A)
                randomModeDifficulty =
                    randomModeDifficulty != AIMode.ExtremeHard
                        ? (AI.AIMode)(randomModeDifficulty + 1)
                        : AIMode.ExtremeHard;
            Debug.Log(randomModeDifficulty);
        }
    }
}