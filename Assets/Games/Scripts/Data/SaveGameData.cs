namespace _JigblockPuzzle
{
    public class SaveGameData
    {
        public int level;

        public SaveGameData LoadData()
        {
            return new SaveGameData();
        }

        public bool IsEndGame(int lv)
        {
            return false;
        }
    }
}