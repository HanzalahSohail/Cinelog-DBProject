using CineLog.Data.Models;

namespace CineLog.BLL.Services
{
    public static class CineLogServiceFactory
    {
        private static string _mode = "EF"; // default mode

        public static void SetMode(string mode)
        {
            _mode = mode.ToUpper() == "SP" ? "SP" : "EF";
        }

        public static string GetMode()
        {
            return _mode;
        }

        public static ICineLogService Create(CineLogContext context)
        {
            if (_mode == "SP")
                return new SpCineLogService(context);

            return new EfCineLogService(context);
        }
    }
}
