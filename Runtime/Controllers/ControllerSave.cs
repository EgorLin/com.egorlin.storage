using System.Collections.Generic;
using EgorLin.Storage.Services;

namespace EgorLin.Storage.Controllers
{
	public class ControllerSave : IControllerSave, ISchedulerSave
	{
		private readonly IEnumerable<IServiceSave> _serviceSaves;

		public ControllerSave(IEnumerable<IServiceSave> serviceSaves)
		{
			_serviceSaves = serviceSaves;
		}

		public void Initialize()
		{
			foreach (var serviceSave in _serviceSaves)
			{
				serviceSave.Initialize();
			}
		}

		public void Save(IStateHandle handle)
		{
			foreach (var serviceSave in _serviceSaves)
			{
				if (serviceSave.Domain == handle.Domain)
				{
					serviceSave.Save(handle);
					break;
				}
			}
		}

		public void Load(IStateHandle handle)
		{
			foreach (var serviceSave in _serviceSaves)
			{
				if (serviceSave.Domain == handle.Domain)
				{
					serviceSave.Load(handle);
					break;
				}
			}
		}
	}
}
