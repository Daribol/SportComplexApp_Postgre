using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SportComplexApp.Services;

public class GlobalTranslationFilter : IAsyncResultFilter
{
    private readonly TranslationService _translationService;

    public GlobalTranslationFilter(TranslationService translationService)
    {
        _translationService = translationService;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var currentCulture = System.Globalization.CultureInfo.CurrentUICulture.Name;

        if (currentCulture.StartsWith("bg"))
        {
            if (context.Result is ViewResult viewResult && viewResult.Model != null)
            {
                var visitedObjects = new HashSet<object>();
                await TranslateObjectAsync(viewResult.Model, visitedObjects);
            }
        }

        await next();
    }

    private async Task TranslateObjectAsync(object obj, HashSet<object> visited)
    {
        if (obj == null) return;

        var type = obj.GetType();
        if (type.IsClass && type != typeof(string))
        {
            if (visited.Contains(obj)) return;
            visited.Add(obj);
        }

        if (obj is IList<string> stringList)
        {
            for (int i = 0; i < stringList.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(stringList[i]))
                {
                    stringList[i] = await _translationService.TranslateToBgAsync(stringList[i]);
                }
            }
            return;
        }

        if (obj is IEnumerable EnumerableObj && !(obj is string))
        {
            foreach (var item in EnumerableObj)
            {
                await TranslateObjectAsync(item, visited);
            }
            return;
        }

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (!prop.CanRead) continue;

            if (prop.Name.Contains("Id") || prop.Name.Contains("Url") ||
                prop.Name.Contains("Image") || prop.Name.Contains("Password") ||
                prop.Name.Contains("Email"))
            {
                continue;
            }

            if (typeof(IList<string>).IsAssignableFrom(prop.PropertyType))
            {
                var list = prop.GetValue(obj) as IList<string>;
                if (list != null)
                {
                    await TranslateObjectAsync(list, visited);
                }
                continue;
            }

            if (!prop.CanWrite) continue;

            if (prop.PropertyType == typeof(string))
            {
                var currentVal = (string)prop.GetValue(obj);
                if (!string.IsNullOrWhiteSpace(currentVal))
                {
                    var translatedVal = await _translationService.TranslateToBgAsync(currentVal);
                    prop.SetValue(obj, translatedVal);
                }
            }
            else if ((prop.PropertyType.IsClass || prop.PropertyType.IsInterface) && prop.PropertyType != typeof(string))
            {
                var subObj = prop.GetValue(obj);
                if (subObj != null)
                {
                    await TranslateObjectAsync(subObj, visited);
                }
            }
        }
    }
}