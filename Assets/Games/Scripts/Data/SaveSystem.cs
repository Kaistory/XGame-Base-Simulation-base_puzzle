using System.IO;
using DevUlts.Assistants.DictionarySerialize;
using Newtonsoft.Json;
using UnityEngine;

public static class SaveSystem
{
    public static readonly string SAVE_PATH = Application.persistentDataPath;

    private static JsonSerializerSettings settings = new JsonSerializerSettings
    {
        MissingMemberHandling = MissingMemberHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Populate,
        ObjectCreationHandling = ObjectCreationHandling.Auto,
        NullValueHandling = NullValueHandling.Ignore
    };

    private static readonly object _lock = new object();

    public static void Initialize()
    {
        if (!Directory.Exists(SAVE_PATH))
        {
            Directory.CreateDirectory(SAVE_PATH);
            Debug.Log($"📁 Đã tạo folder mới: {SAVE_PATH}");
        }
    }

    public static void Save<T>(string key, T data)
    {
        string path = GetPath(key);
        string dir = Path.GetDirectoryName(path);

        if (!Directory.Exists(dir) && dir != null)
        {
            Directory.CreateDirectory(dir);
        }

        lock (_lock)
        {
            if (File.Exists(path))
            {
                string backupPath = Path.Combine(
                    Path.GetDirectoryName(path) ?? "",
                    Path.GetFileNameWithoutExtension(path) + ".bak"
                );
                File.Copy(path, backupPath, true);
            }

            string json = JsonConvert.SerializeObject(data, Formatting.Indented, settings);
            File.WriteAllText(path, json);

            Log($"✅ Đã lưu {key} thành công.");
        }
    }

    public static T Load<T>(string key, bool tryBackup = false) where T : new()
    {
        string path = GetPath(key);

        if (!File.Exists(path))
        {
            LogWarning($"⚠️ LoadSafe: Missing {path}. Return default (no write).");
            return new T(); // KHÔNG ghi file mới ở đây
        }

        var val = TryLoadFromPath<T>(path);
        if (val != null) return val;

        if (tryBackup)
        {
            string bak = System.IO.Path.ChangeExtension(path, ".bak");
            if (File.Exists(bak))
            {
                LogWarning($"⚠️ LoadSafe: main JSON invalid, try backup {bak}");
                val = TryLoadFromPath<T>(bak);
                if (val != null) return val;
            }
        }

        LogWarning("⚠️ LoadSafe: both main and backup invalid. Return default.");
        return new T(); // vẫn không ghi đè file
    }

    private static T TryLoadFromPath<T>(string path) where T : new()
    {
        try
        {
            string json = System.IO.File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json) || json == "null") return default;
            var loaded = JsonConvert.DeserializeObject<T>(json, settings);
            if (loaded == null) return default;
            var fb = new T();
            MergeFallbackFields(loaded, fb);
            return loaded;
        }
        catch (System.Exception ex)
        {
            LogWarning($"TryLoadFromPath error {path}: {ex.Message}");
            return default;
        }
    }


    public static bool Exists(string key)
    {
        return File.Exists(GetPath(key));
    }

    public static string GetPath(string fileName)
    {
        return GetFileAtPath($"", fileName);
    }

    public static string GetFileAtPath(string folder, string fileName)
    {
        EnsureFolderExists(folder);
        return Path.Combine(SAVE_PATH, folder, fileName);
    }

    public static void EnsureFolderExists(string relativeFolder)
    {
        string fullPath = Path.Combine(SAVE_PATH, relativeFolder);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
            Log($"📁 Đã tạo folder mới: {fullPath}");
        }
    }

    public static bool ExistsFolder(string key)
    {
        var path = GetPath(key);
        return Directory.Exists(path);
    }

    public static void Delete(string key)
    {
        string path = GetPath(key);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static void ClearAllData()
    {
        string path = Application.persistentDataPath;

        if (Directory.Exists(path))
        {
            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            Log("✅ All data in persistentDataPath cleared.");
        }
        else
        {
            LogWarning("❗ No persistentDataPath found.");
        }
    }

    public static void CopyData<TOld, TNew>(string oldKey, string newKey)
        where TOld : new()
        where TNew : class, new()
    {
        string oldPath = GetPath(oldKey);
        string newPath = GetPath(newKey);

        if (!File.Exists(oldPath))
        {
            LogWarning($"❗ Không tìm thấy file cũ tại: {oldPath}");
            return;
        }

        try
        {
            string oldJson = File.ReadAllText(oldPath);
            TOld oldData = JsonConvert.DeserializeObject<TOld>(oldJson, settings);

            string tempJson = JsonConvert.SerializeObject(oldData, Formatting.Indented, settings);
            TNew newData = JsonConvert.DeserializeObject<TNew>(tempJson, settings);

            string newJson = JsonConvert.SerializeObject(newData, Formatting.Indented);
            File.WriteAllText(newPath, newJson);

            Log($"✅ Đã copy data từ '{oldKey}' sang '{newKey}' thành công.");
        }
        catch (System.Exception ex)
        {
            LogError($"❌ Lỗi khi copy data từ '{oldKey}' sang '{newKey}': {ex}");
        }
    }

    public static void CopyAllData(string fromFolder, string toFolder, string[] extensions = null)
    {
        string fromPath = Path.Combine(SAVE_PATH, fromFolder);
        string toPath = Path.Combine(SAVE_PATH, toFolder);

        if (!Directory.Exists(fromPath))
        {
            LogWarning($"❗ Không tìm thấy folder gốc: {fromPath}");
            return;
        }

        if (!Directory.Exists(toPath))
        {
            Directory.CreateDirectory(toPath);
        }

        string[] files = Directory.GetFiles(fromPath, "*", SearchOption.AllDirectories);
        int copiedCount = 0;

        foreach (string file in files)
        {
            string ext = Path.GetExtension(file).ToLower();
            if (extensions != null && extensions.Length > 0 &&
                !System.Array.Exists(extensions, e => e.ToLower() == ext))
                continue;

            try
            {
                string relativePath = file.Substring(fromPath.Length + 1);
                string targetPath = Path.Combine(toPath, relativePath);
                string targetDir = Path.GetDirectoryName(targetPath);

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                File.Copy(file, targetPath, true);
                copiedCount++;
            }
            catch (System.Exception ex)
            {
                LogError($"❌ Lỗi khi copy file '{file}': {ex.Message}");
            }
        }

        Log($"✅ Đã copy {copiedCount} file từ '{fromFolder}' sang '{toFolder}'.");
    }

    public static void DeleteFilesWithExtension(string folder, string extension)
    {
        string targetPath = Path.Combine(SAVE_PATH, folder ?? "");

        if (!Directory.Exists(targetPath))
        {
            LogWarning($"❗ Không tìm thấy folder: {targetPath}");
            return;
        }

        string[] files = Directory.GetFiles(targetPath, $"*{extension}", SearchOption.AllDirectories);
        int deletedCount = 0;

        foreach (string file in files)
        {
            try
            {
                File.Delete(file);
                deletedCount++;
            }
            catch (System.Exception ex)
            {
                LogError($"❌ Lỗi khi xoá file '{file}': {ex.Message}");
            }
        }

        Log($"🗑️ Đã xoá {deletedCount} file có đuôi '{extension}' trong folder '{folder}'.");
    }

    private static void MergeFallbackFields<T>(T target, T fallback)
    {
        foreach (var prop in typeof(T).GetProperties())
        {
            if (!prop.CanWrite || !prop.CanRead) continue;
            var value = prop.GetValue(target);
            if (value == null)
            {
                LogWarning($"⚠️ Field {prop.Name} is null, using fallback.");
                prop.SetValue(target, prop.GetValue(fallback));
            }
        }

        foreach (var field in typeof(T).GetFields())
        {
            var value = field.GetValue(target);
            if (value == null)
            {
                LogWarning($"⚠️ Field {field.Name} is null, using fallback.");
                field.SetValue(target, field.GetValue(fallback));
            }
        }
    }

    #region Log API

#if UNITY_EDITOR
    private static readonly bool ENABLE_LOGGING = false;
#else
    private static readonly bool ENABLE_LOGGING = false;
#endif

    private static readonly string LogRegion = $"{nameof(SaveSystem)}";

    private static void Log(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.Log($"[{LogRegion}] Log: {message}");
        }
    }

    private static void LogError(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogError($"[{LogRegion}] LogError: {message}");
        }
    }

    private static void LogWarning(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogWarning($"[{LogRegion}] LogWarning: {message}");
        }
    }

    #endregion
}