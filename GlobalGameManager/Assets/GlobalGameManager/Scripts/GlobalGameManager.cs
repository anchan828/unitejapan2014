using GlobalGameManager;
using UnityEngine;

public class GlobalGameManager<T> : GlobalGameManagerObject where T : GlobalGameManagerObject
{
    private static T m_instance;

    public static T instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = Resources.Load<T>(string.Format("GlobalGameManager/{0}", typeof(T).Name));
                
                if (m_instance == null)
                {
                    m_instance = Creator.GlobalGameManager<T>();
                }
            }

            return m_instance;
        }
    }
}
