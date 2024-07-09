namespace ParanormalUniverse
{
    public class Achievements : AchievementsParent
    {
        private int[] _neededPurchasedMemeClips = { 1, 5, 10, 15, 20, 27 };

        public override void Init()
        {
            NeededPurchasedMemeClips = _neededPurchasedMemeClips;
            base.Init();
        }
    }
}