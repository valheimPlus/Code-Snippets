public static T GetChildComponentByName<T>(string name, GameObject objected, bool caseSensitive = true) where T : Component
{
   foreach (T component in objected.GetComponentsInChildren<T>(true))
   {
       bool comparison = false;
       if (!caseSensitive)
           comparison = component.gameObject.name.ToLower() == name.ToLower();
       else 
           comparison = component.gameObject.name == name;
       if (comparison)
           return component;
    }
    return null;
}
