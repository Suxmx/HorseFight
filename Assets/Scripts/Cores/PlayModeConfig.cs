using AI;
using Services;

namespace Cores
{
    public class PlayModeConfig : Service
    {
        private bool hasInited = false;
        private AIMode randomModeDifficulty=AIMode.Easy;

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
    }
}