using UnityEngine;
using System.Collections;

namespace GameModule
{
	public class Singleton<T> where T : class, new()
	{
		private static object _syncobj = new object();
		private static T _instance = null;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_syncobj)
					{
						if (_instance == null)
						{
							_instance = new T();
						}
					}
				}
				return _instance;
			}
		}
		public static T GetInstance()
        {
			return _instance;
        }
		public Singleton()
		{
		}
	}
}