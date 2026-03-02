using UnityEngine;
using UnityEditor;

/// <summary>
/// Редакторный скрипт для создания префаба игрока.
/// Использование: Tools → Doodle Jump → Create Doodler Prefab
/// </summary>
public class DoodlerPrefabCreator : EditorWindow
{
    private string prefabName = "Doodler";
    private float spriteSize = 1f;

    [MenuItem("Tools/Doodle Jump/Create Doodler Prefab")]
    public static void ShowWindow()
    {
        GetWindow<DoodlerPrefabCreator>("Создание Doodler");
    }

    private void OnGUI()
    {
        GUILayout.Label("Создание префаба игрока", EditorStyles.boldLabel);

        prefabName = EditorGUILayout.TextField("Имя префаба", prefabName);
        spriteSize = EditorGUILayout.FloatField("Размер спрайта", spriteSize);

        if (GUILayout.Button("Создать префаб"))
        {
            CreateDoodlerPrefab();
        }

        if (GUILayout.Button("Создать спрайт-квадрат"))
        {
            CreateDoodlerSprite();
        }
    }

    private void CreateDoodlerSprite()
    {
        // Создаём текстуру программно
        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];

        for (int i = 0; i < pixels.Length; i++)
        {
            // Зелёный цвет для Doodler
            pixels[i] = new Color(0.2f, 0.8f, 0.2f, 1f);
        }

        texture.SetPixels(pixels);
        texture.Apply();

        // Сохраняем как PNG
        byte[] pngData = texture.EncodeToPNG();
        string path = "Assets/Sprites/DoodlerSprite.png";
        
        // Создаём папку если нет
        System.IO.Directory.CreateDirectory("Assets/Sprites");
        System.IO.File.WriteAllBytes(path, pngData);
        
        AssetDatabase.Refresh();
        Debug.Log($"Спрайт создан: {path}");
    }

    private void CreateDoodlerPrefab()
    {
        // Создаём GameObject
        GameObject doodler = new GameObject(prefabName);
        
        // Добавляем SpriteRenderer
        SpriteRenderer spriteRenderer = doodler.AddComponent<SpriteRenderer>();
        
        // Пытаемся найти спрайт
        string spritePath = "Assets/Sprites/DoodlerSprite.png";
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"Спрайт не найден: {spritePath}. Создайте его сначала.");
        }
        
        // Добавляем Rigidbody2D
        Rigidbody2D rigidbody = doodler.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 1f;
        rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // Добавляем BoxCollider2D
        BoxCollider2D collider = doodler.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.8f, 0.8f);
        collider.offset = new Vector2(0f, 0f);
        
        // Добавляем скрипт контроллера
        doodler.AddComponent<DoodlerController>();
        
        // Создаём папку для префабов
        string prefabsFolder = "Assets/Prefabs";
        if (!System.IO.Directory.Exists(prefabsFolder))
        {
            System.IO.Directory.CreateDirectory(prefabsFolder);
            AssetDatabase.Refresh();
        }
        
        // Создаём префаб
        string prefabPath = $"{prefabsFolder}/{prefabName}.prefab";
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (existingPrefab != null)
        {
            PrefabUtility.SaveAsPrefabAsset(doodler, prefabPath);
        }
        else
        {
            PrefabUtility.SaveAsPrefabAsset(doodler, prefabPath);
        }
        
        // Удаляем объект из сцены
        DestroyImmediate(doodler);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Префаб создан: {prefabPath}");
        
        // Показываем в проектном окне
        UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);
    }
}
