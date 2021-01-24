using System;
using PostgreSQL.ApplicationInsights.ConsoleApp.Services;

namespace PostgreSQL.ApplicationInsights.ConsoleApp
{
    public class Browser
    {
        private readonly IMvcService _mvcService;

        public Browser(IMvcService mvcService)
        {
            _mvcService = mvcService;
        }

        public void Show()
        {
            ShowIndex();
        }

        private void ShowIndex()
        {
            var response = _mvcService.Navigate("Index/Get");
            if(response == nameof(ConsoleKey.Escape))
                return;
            
            ShowCategory(response);
        }

        private void ShowCategory(string path)
        {
           var response = _mvcService.Navigate(path);
           if (response == nameof(ConsoleKey.Backspace))
               ShowIndex();
           else
               ShowProduct(path, response);

        }

        private void ShowProduct(string oldPath, string path)
        {
            var response = _mvcService.Navigate(path);
            if (response == nameof(ConsoleKey.Backspace))
                ShowCategory(oldPath);
            else
                ShowProduct(oldPath, response);
        }
    }
}
