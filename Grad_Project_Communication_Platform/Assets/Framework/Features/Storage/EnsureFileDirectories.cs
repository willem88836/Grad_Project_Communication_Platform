using Framework.ScriptableObjects.Variables;
using System;

namespace Framework.Storage
{
	[Serializable]
	public class EnsureFileDirectories
	{
		public SharedString[] Directories;
		public SharedString[] Files;


		public void Invoke()
		{
			foreach(SharedString dir in Directories)
			{
				SaveLoad.EnsureDirectoryExistence(dir.Value);
			}
			foreach(SharedString path in Files)
			{
				SaveLoad.EnsureDirectoryExistenceOfFile(path.Value);
			}
		}
	}
}
