namespace Pereklichka.Database
{
    public abstract class DataHelper
    {
        private static Model1 _context = new Model1();

        public static Model1 GetContext()
        {
            return _context;
        }
    }
}
