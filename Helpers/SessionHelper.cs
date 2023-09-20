using Newtonsoft.Json;
namespace OnlineShop.Helpers;

public static class SessionHelper
{
    /// <summary>
    /// 設定 Session 內容
    /// </summary>
    /// <param name="session"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        session.SetString(key, JsonConvert.SerializeObject(value, settings));
    }

    /// <summary>
    /// 取得 Session 內容
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="session"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T GetObjectFromJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }

    /// <summary>
    /// 移除 Session 
    /// </summary>
    /// <param name="session"></param>
    /// <param name="key"></param>
    public static void Remove(this ISession session, string key)
    {
        session.Remove(key);
    }
}
