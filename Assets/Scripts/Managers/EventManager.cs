namespace Game
{
    public class EventManager : Singleton<EventManager>
    {
        public GameEvent OnDamageTaken;
        public GameEvent OnLevelCompleted;
        public GameEvent OnLevelFailed;
        public GameEvent OnLevelLoaded;
        public GameEvent OnOutOfArrow;
    }
}
