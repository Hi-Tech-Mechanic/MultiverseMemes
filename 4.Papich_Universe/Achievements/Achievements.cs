namespace PapichUniverse
{
    public class Achievements : AchievementsParent
    {
        private int[] _neededPurchasedMemeClips = { 1, 5, 10, 16 };

        public override void Init()
        {
            NeededPurchasedMemeClips = _neededPurchasedMemeClips;
            base.Init();
        }
    }
}