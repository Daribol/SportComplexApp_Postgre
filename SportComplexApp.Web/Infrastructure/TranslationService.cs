using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using GTranslate.Translators;
using Microsoft.AspNetCore.Hosting;

namespace SportComplexApp.Services
{
    public class TranslationService
    {
        private readonly GoogleTranslator _translator;
        private readonly string _cacheFilePath;
        private Dictionary<string, string> _fileCache;

        public TranslationService(IWebHostEnvironment env)
        {
            _translator = new GoogleTranslator();

            _cacheFilePath = Path.Combine(env.ContentRootPath, "translations_cache.json");

            LoadCache();
        }

        private void LoadCache()
        {
            if (File.Exists(_cacheFilePath))
            {
                var json = File.ReadAllText(_cacheFilePath);
                _fileCache = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
            else
            {
                _fileCache = new Dictionary<string, string>();
            }
        }

        private void SaveCache()
        {
            var json = JsonSerializer.Serialize(_fileCache, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_cacheFilePath, json);
        }

        public async Task<string> TranslateToBgAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            string key = text.Trim();

            if (_fileCache.TryGetValue(key, out string translatedText))
            {
                return translatedText;
            }

            try
            {
                await Task.Delay(200);

                var result = await _translator.TranslateAsync(key, "bg", "en");
                translatedText = result.Translation;

                _fileCache[key] = translatedText;
                SaveCache();

                return translatedText;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Translation Error for '{key}']: {ex.Message}");
                return text;
            }
        }
    }
}