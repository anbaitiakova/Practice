using System;
using System.Collections.Generic;

namespace Task_3
{
    class Program
    {
        public sealed class ApplicationCache<T>
        {
            private Dictionary<string, (DateTime, T)> _cache;
            private TimeSpan _lifetime;
            private int _maxSize;

            public ApplicationCache(TimeSpan time, int capacity)
            {
                _cache = new Dictionary<string, (DateTime, T)>();
                _lifetime = time;
                _maxSize = capacity;
            }
            public void ControlTimeOfLife()
            {
                foreach (var it in _cache)
                {
                    TimeSpan tmp = DateTime.Now - it.Value.Item1;
                    if (tmp >= _lifetime)
                        _cache.Remove(it.Key);
                }    
            }
            
            public void Save(string key, T data)
            {
                if (_cache.TryGetValue(key, out _))
                    throw new ArgumentException(nameof(key));
                else if (_cache.Count == _maxSize)
                {
                    TimeSpan old = TimeSpan.Zero;
                    string tmpKey = null;
                    foreach (var it in _cache)
                    {
                        TimeSpan difference = DateTime.Now - it.Value.Item1;
                        if (difference > old)
                        {
                            old = difference;
                            tmpKey = it.Key;
                        }
                        _cache.Remove(tmpKey);
                    }
                    _cache.Add(key,(DateTime.Now, data));
                }
                else
                {
                    _cache.Add(key,(DateTime.Now, data));
                }
                ControlTimeOfLife();
            }
            
            public T Get(string key)
            {
                (DateTime, T) keyData;
                ControlTimeOfLife();
                if (_cache.TryGetValue(key, out keyData))
                {
                    ControlTimeOfLife();
                    return keyData.Item2;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
                    
            }
        }
        static void Main(string[] args)
        {
            ApplicationCache<string> cache = new ApplicationCache<string>(new TimeSpan(0, 0, 20), 3);
            string key;
            
            int choice = 0;
            while (choice != 3)
            {
                Console.WriteLine("{0}{1}{0}{2}{0}{3}{0}{4}", Environment.NewLine,"What do yo want to do?", "1. Add", "2. Get", "3. Exit");
                choice = Convert.ToInt16(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        try
                        {
                            Console.WriteLine("Enter key and information:");
                            key = Console.ReadLine();
                            string inf = Console.ReadLine();
                            cache.Save(key, inf);
                        }
                        catch (ArgumentException)
                        {
                            Console.WriteLine("Such a key already exists");
                        }
                        break;
                    case 2:
                        try
                        {
                            Console.WriteLine("Enter key:");
                            key = Console.ReadLine();
                            string received = cache.Get(key);
                            Console.WriteLine(received);
                        }
                        catch (KeyNotFoundException)
                        {
                            Console.WriteLine("There is no such key");
                        }
                        break;
                    case 3:
                        break;
                    default:
                        continue;
                        
                }
            }
            
        }
    }
}