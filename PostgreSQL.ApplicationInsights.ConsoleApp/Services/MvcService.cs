using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PostgreSQL.ApplicationInsights.ConsoleApp.Extensions;
using PostgreSQL.ApplicationInsights.ConsoleApp.Migrations.Seeds;
using PostgreSQL.ApplicationInsights.ConsoleApp.Views;
using static System.Reflection.BindingFlags;

namespace PostgreSQL.ApplicationInsights.ConsoleApp.Services
{
    public interface IMvcService
    {
        string Navigate(string url);
    }

    public class MvcService : IMvcService
    {
        private readonly IServiceProvider _provider;
        private Dictionary<string, Func<string,string>> _route;
        private Dictionary<Type, object> _views;

        public MvcService(IServiceProvider provider)
        {
            _provider = provider;
        }

        private void Init(IServiceProvider provider)
        {
            _views = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.GetInterface(nameof(IView)) != null)
                .ToDictionary(k => k, provider.GetService);
            _route = new Dictionary<string, Func<string,string>>();
            foreach (var view in _views)
            {
                var methods = view.Key
                    .GetMethods(DeclaredOnly | Public | Instance)
                    .Where(x => x.Name.StartsWith("Get")).ToList();

                foreach (var method in methods)
                {
                    var path = $"{view.Key.Name.Replace("View", "")}/{method.Name}/{method.GetParameters().Length}";

                    _route.Add(path, url =>
                    {
                        var parts = url.Split('/');
                        var parameters = new List<object>();
                        for (int i = 2; i < parts.Length; i++)
                        {
                            parameters.Add(int.Parse(parts[i]));
                        }

                        Console.Clear();
                        return method.Invoke(view.Value, parameters.ToArray<object>())?.ToString();
                    });
                }
            }
        }

        public string Navigate(string url)
        {
            if (_route == null)
                Init(_provider);

            var parts = url.Split('/');
            var controller = parts[0];
            var method = parts[1];
            var parameterCount = parts.Length - 2;
            var path = $"{controller}/{method}/{parameterCount}";
            var func = _route[path];
            return func.Invoke(url);
        }
    }
}