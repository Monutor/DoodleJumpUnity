using UnityEngine;
using UnityEditor;

/// <summary>
/// Редакторный скрипт для добавления тега "Player".
/// Использование: автоматически при запуске или через меню.
/// </summary>
[InitializeOnLoad]
public class PlayerTagSetup
{
    static PlayerTagSetup()
    {
        EnsurePlayerTagExists();
    }

    [MenuItem("Tools/Doodle Jump/Add Player Tag")]
    public static void AddPlayerTagManually()
    {
        EnsurePlayerTagExists();
        Debug.Log("Тег 'Player' проверен/добавлен");
    }

    private static void EnsurePlayerTagExists()
    {
        // Проверяем, существует ли тег "Player"
        string[] tags = new string[] { "Player" };
        bool playerTagExists = false;
        
        foreach (string tag in tags)
        {
            try
            {
                if (AssetDatabase.LoadAssetAtPath("ProjectSettings/TagManager.asset", typeof(UnityEngine.Object)) != null)
                {
                    // Проверяем через SerializedObject
                    SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAssetAtPath("ProjectSettings/TagManager.asset", typeof(UnityEngine.Object)));
                    SerializedProperty tagsProp = tagManager.FindProperty("tags");
                    
                    for (int i = 0; i < tagsProp.arraySize; i++)
                    {
                        SerializedProperty tagProp = tagsProp.GetArrayElementAtIndex(i);
                        if (tagProp.stringValue == "Player")
                        {
                            playerTagExists = true;
                            break;
                        }
                    }
                    
                    if (!playerTagExists)
                    {
                        // Добавляем тег "Player"
                        int newIndex = tagsProp.arraySize;
                        tagsProp.InsertArrayElementAtIndex(newIndex);
                        SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(newIndex);
                        newTag.stringValue = "Player";
                        tagManager.ApplyModifiedProperties();
                        Debug.Log("Тег 'Player' добавлен в Tag Manager");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Не удалось проверить/добавить тег Player: {e.Message}");
            }
        }
    }
}
